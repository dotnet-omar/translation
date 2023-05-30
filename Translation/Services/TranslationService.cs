using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
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
        var savedTranslationFiles = _translationFileRepository.List();
        var savedTranslationsKey = _translationKeyRepository.ListAll();
        var savedTranslationsValue = _translationValueRepository.ListAll();
        var currentTranslationFilesPath = GetCurrentFilesPath(folderPath).ToList();

        foreach (var filePath in currentTranslationFilesPath)
        {
            var fileContent = File.ReadAllText(filePath);
            var fileHash = GetHash(fileContent);

            var savedTranslationFile = savedTranslationFiles.Find(x => x.FilePath == filePath);

            if (savedTranslationFile != null && savedTranslationFile.FileHash == fileHash) continue;

            TranslationFile translationFile;

            if (savedTranslationFile != null)
            {
                translationFile = savedTranslationFile;
                translationFile.FileHash = fileHash;
            }
            else
            {
                translationFile = new TranslationFile
                {
                    FilePath = filePath,
                    FileHash = fileHash
                };
            }

            var (languageCode, module) = translationFile.GetFileInfo();

            var language = savedLanguages.Find(l => l.Code == languageCode)!;

            var savedModuleTranslationsKey = savedTranslationsKey.Where(x => x.Module == module).ToList();
            var savedModuleTranslationsValue = savedTranslationsValue
                .Where(x => x.TranslationKey.Module == module && x.LanguageId == language.Id)
                .ToList();

            var translationResults = GetFileTranslations(fileContent);
            var currentTranslationsKey = translationResults
                .Select(x => new TranslationKey() { Key = x.Key, Module = module });

            var newTranslationsKey = currentTranslationsKey
                .Where(x => savedModuleTranslationsKey.All(y => y.Key != x.Key))
                .ToList();

            _translationKeyRepository.AddRange(newTranslationsKey);

            var allTranslationsKey = savedModuleTranslationsKey.Concat(newTranslationsKey).ToList();

            var newTranslationsValue = new List<TranslationValue>();
            var updatedTranslationsValue = new List<TranslationValue>();

            foreach (var result in translationResults)
            {
                var translationKey = allTranslationsKey.First(x => x.Key == result.Key);

                var translationValue = savedModuleTranslationsValue
                    .FirstOrDefault(x => x.TranslationKeyId == translationKey.Id);

                if (translationValue == null)
                {
                    var newTranslationValue = new TranslationValue
                    {
                        TranslationKeyId = translationKey.Id,
                        LanguageId = language.Id,
                        DefaultValue = result.Value,
                    };

                    newTranslationsValue.Add(newTranslationValue);
                }
                else if (translationValue.DefaultValue != result.Value)
                {
                    translationValue.DefaultValue = result.Value;
                    updatedTranslationsValue.Add(translationValue);
                }
            }

            _translationValueRepository.AddRange(newTranslationsValue);
            _translationValueRepository.UpdateRange(updatedTranslationsValue);

            var allTranslationsValue = savedModuleTranslationsValue
                .Concat(newTranslationsValue)
                .Concat(updatedTranslationsValue)
                .ToList();

            var unusedSavedTranslationValues = allTranslationsValue
                .Where(x => translationResults.All(y => y.Key != x.TranslationKey.Key))
                .ToList();

            _translationValueRepository.DeleteRange(unusedSavedTranslationValues);

            if (translationFile.Id.Equals(null)) _translationFileRepository.Create(translationFile);
            else _translationFileRepository.Update(translationFile);
        }

        var unusedSavedTranslationFiles = savedTranslationFiles
            .Where(x => currentTranslationFilesPath.All(y => y != x.FilePath))
            .ToList();

        _translationValueRepository.DeleteAllByFiles(unusedSavedTranslationFiles, savedTranslationsValue);

        _translationFileRepository.RemoveRange(unusedSavedTranslationFiles);

        _translationKeyRepository.RemoveAllUnusedKeys();

        _translationKeyRepository.Commit();

        _translationValueRepository.Commit();

        _translationFileRepository.Commit();
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