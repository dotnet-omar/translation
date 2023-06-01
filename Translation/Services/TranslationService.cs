using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using Translation.Classes;
using Translation.Dto.TranslationResult;
using Translation.Models;
using Translation.Repositories;

namespace Translation.Services;

public class TranslationService
{
    private readonly LanguageRepository _languageRepository;
    private readonly TranslationFileRepository _translationFileRepository;
    private readonly TranslationKeyRepository _translationKeyRepository;
    private readonly TranslationValueRepository _translationValueRepository;

    public TranslationService(
        LanguageRepository languageRepository,
        TranslationFileRepository translationFileRepository,
        TranslationKeyRepository translationKeyRepository,
        TranslationValueRepository translationValueRepository
    )
    {
        _languageRepository = languageRepository;
        _translationFileRepository = translationFileRepository;
        _translationKeyRepository = translationKeyRepository;
        _translationValueRepository = translationValueRepository;
    }

    public void Sync(string folderPath, string languagesFilePath)
    {
        SyncLanguages(languagesFilePath);
        SyncTranslations(folderPath);
    }

    private void SyncLanguages(string languagesFilePath)
    {
        var savedLanguages = _languageRepository.ListAll();
        var currentLanguages = GetCurrentLanguages(languagesFilePath);
        _languageRepository.ResetAllIsLockedLanguages(savedLanguages);
        _languageRepository.CreateOrUpdate(savedLanguages, currentLanguages);
        _languageRepository.Commit();
    }

    private void SyncTranslations(string folderPath)
    {
        var savedLanguages = _languageRepository.ListAll();
        var savedTranslationFiles = _translationFileRepository.ListAll();
        var savedTranslationsKey = _translationKeyRepository.ListAll();
        var savedTranslationsValue = _translationValueRepository.ListAll();
        var currentTranslationFilesPath = GetCurrentFilesPath(folderPath).ToList();

        var translationResult = UpdateThenGetTranslationSyncResult(
            currentTranslationFilesPath,
            savedTranslationFiles,
            savedLanguages,
            savedTranslationsKey,
            savedTranslationsValue
        );

        _translationFileRepository.AddRange(translationResult.AllNewTranslationsFile);

        _translationKeyRepository.AddRange(translationResult.AllNewTranslationsKey);

        _translationValueRepository.AddRange(translationResult.AllNewTranslationsValue);

        _translationValueRepository.RemoveRange(translationResult.AllUnusedTranslationsValue);

        _translationKeyRepository.RemoveRange(translationResult.AllUnusedTranslationsKey);

        _translationFileRepository.RemoveRange(translationResult.AllUnusedTranslationsFile);
    }

    private static TranslationSyncResult UpdateThenGetTranslationSyncResult(
        List<string> currentTranslationFilesPath,
        IReadOnlyCollection<TranslationFile> savedTranslationFiles,
        IReadOnlyCollection<Language> savedLanguages,
        IReadOnlyCollection<TranslationKey> savedTranslationsKey,
        IReadOnlyCollection<TranslationValue> savedTranslationsValue
    )
    {
        var translationSyncResult = new TranslationSyncResult();

        foreach (var filePath in currentTranslationFilesPath)
        {
            var fileContent = File.ReadAllText(filePath);
            var fileHash = GetHash(fileContent);

            var translationFile = savedTranslationFiles.FirstOrDefault(x => x.FilePath == filePath);

            if (translationFile != null && translationFile.FileHash == fileHash) continue;

            translationFile ??= new TranslationFile();

            translationFile.FilePath = filePath;
            translationFile.FileHash = fileHash;

            var (languageCode, module) = translationFile.GetFileInfo();

            var language = savedLanguages.First(l => l.Code == languageCode)!;

            var savedModuleTranslationsKey = savedTranslationsKey
                .Concat(translationSyncResult.AllNewTranslationsKey)
                .Where(x => x.Module == module)
                .ToList();

            var savedModuleTranslationsValue = savedTranslationsValue
                .Where(x => x.TranslationKey.Module == module && x.LanguageId == language.Id)
                .ToList();

            var translationResults = GetFileTranslations(fileContent);
            var currentTranslationsKey = translationResults
                .Select(x => new TranslationKey() { Key = x.Key, Module = module });

            var newTranslationsKey = currentTranslationsKey
                .Where(x => savedModuleTranslationsKey.All(y => y.Key != x.Key))
                .ToList();

            translationSyncResult.AllNewTranslationsKey.AddRange(newTranslationsKey);

            var allTranslationsKey = savedModuleTranslationsKey.Concat(newTranslationsKey).ToList();

            var newTranslationsValue = new List<TranslationValue>();

            foreach (var result in translationResults)
            {
                var translationKey = allTranslationsKey.First(x => x.Key == result.Key);

                var translationValue = savedModuleTranslationsValue.FirstOrDefault(x =>
                    translationKey.Id != Guid.Empty && x.TranslationKeyId == translationKey.Id
                );

                if (translationValue != null && translationValue.DefaultValue == result.Value) continue;

                if (translationValue == null)
                {
                    translationValue ??= new TranslationValue
                    {
                        LanguageId = language.Id,
                        DefaultValue = result.Value,
                        TranslationKey = translationKey
                    };

                    newTranslationsValue.Add(translationValue);
                }
                else if (translationValue.DefaultValue != result.Value)
                {
                    translationValue.DefaultValue = result.Value;
                }
            }

            translationSyncResult.AllNewTranslationsValue.AddRange(newTranslationsValue);

            var allTranslationsValue = savedModuleTranslationsValue
                .Concat(newTranslationsValue)
                .ToList();

            var translationsValueDeletedKey = allTranslationsValue
                .Where(x => translationResults.All(y => y.Key != x.TranslationKey.Key))
                .ToList();

            translationSyncResult.AllUnusedTranslationsValue.AddRange(translationsValueDeletedKey);

            if (translationFile.Id == Guid.Empty) translationSyncResult.AllNewTranslationsFile.Add(translationFile);
        }

        translationSyncResult.AllUnusedTranslationsFile = savedTranslationFiles
            .Where(x => currentTranslationFilesPath.All(y => y != x.FilePath))
            .ToList();

        var allUnusedTranslationsFileInfo =
            translationSyncResult.AllUnusedTranslationsFile.Select(x => x.GetFileInfo()).ToList();

        var translationsValueDeletedFile = savedTranslationsValue.Where(x =>
            allUnusedTranslationsFileInfo.Any(y =>
                y.languageCode == x.Language.Code && y.module == x.TranslationKey.Module
            )
        );

        translationSyncResult.AllUnusedTranslationsValue.AddRange(translationsValueDeletedFile);

        translationSyncResult.AllUnusedTranslationsKey = savedTranslationsKey
            .Where(x => !x
                .Values
                .Any(y =>
                    y.DefaultValue != null &&
                    !translationSyncResult.AllUnusedTranslationsValue.Any(z =>
                        z.TranslationKeyId == y.TranslationKeyId && z.LanguageId == y.LanguageId
                    )
                )
            )
            .ToList();

        return translationSyncResult;
    }

    private static IEnumerable<string> GetCurrentFilesPath(string folderPath)
    {
        return Directory
            .GetFiles(folderPath, "*.json", SearchOption.AllDirectories)
            .ToList();
    }

    private static List<Language> GetCurrentLanguages(string languagesFilePath)
    {
        var languagesList = new List<Language>();

        var fileContent = File.ReadAllText(languagesFilePath);

        var jsonObject = JObject.Parse(fileContent);

        var languagesProperty = jsonObject["languages"];

        if (languagesProperty?.Type is JTokenType.Array)
        {
            languagesList.AddRange(
                ((JArray)languagesProperty).Select(item =>
                    new Language()
                    {
                        Label = item["label"]?.ToString() ?? "",
                        Code = item["code"]?.ToString() ?? "",
                        IsRtl = (bool)(item["isRtl"] ?? false),
                        Order = (int)(item["order"] ?? 0),
                        IsLocked = true,
                    }
                )
            );
        }

        return languagesList;
    }

    private static List<TranslationResultDto> GetFileTranslations(string fileContent)
    {
        var translations = new List<TranslationResultDto>();

        var jsonObject = JObject.Parse(fileContent);

        foreach (var item in jsonObject)
        {
            if (item.Value?.Type != JTokenType.String) continue;

            translations.Add(new()
                {
                    Key = item.Key,
                    Value = item.Value?.ToString() ?? "",
                }
            );
        }

        return translations;
    }

    private static string GetHash(string text)
    {
        var textBytes = Encoding.UTF8.GetBytes(text);
        var hash = SHA256.HashData(textBytes);
        return BitConverter.ToString(hash);
    }
}