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

    private void SyncTranslations(string folderPath)
    {
        var savedLanguages = _languageRepository.GetAllLanguages();
        var savedTranslationFiles = _translationFileRepository.List();
        var currentTranslationFilesPath = GetCurrentFilesPath(folderPath).ToList();

        foreach (var filePath in currentTranslationFilesPath)
        {
            var fileContent = File.ReadAllText(filePath);
            var fileHash = GetHash(fileContent);

            var savedTranslationFile = savedTranslationFiles.Find(x => x.FilePath == filePath);

            if (savedTranslationFile != null && savedTranslationFile.FileHash == fileHash) continue;

            var translationFile = new TranslationFile
            {
                FilePath = filePath,
                FileHash = fileHash
            };

            var (languageCode, module) = translationFile.GetFileInfo();

            var language = savedLanguages.Find(l => l.Code == languageCode)!;
            var translationResults = GetFileTranslations(fileContent);
            var savedTranslationsKey = _translationKeyRepository.GetAllBy(module);
            var savedTranslationsValue = _translationValueRepository.GetAll(module, language.Id);

            foreach (var result in translationResults)
            {
                var translationKey = savedTranslationsKey.FirstOrDefault(x => x.Key == result.Key);

                if (translationKey == null)
                {
                    translationKey = _translationKeyRepository.CreateItemFromResult(result.Key, module)!;
                }

                var translationValue = savedTranslationsValue
                    .FirstOrDefault(x => x.TranslationKeyId == translationKey.Id);

                if (translationValue == null)
                {
                    _translationValueRepository.CreateItemFromResult(
                        translationKey.Id,
                        language.Id,
                        result.Value
                    );
                }
                else if (translationValue.DefaultValue != result.Value)
                {
                    _translationValueRepository.UpdateDefaultValue(translationValue, result.Value);
                }
            }

            var unusedSavedTranslationValues = _translationValueRepository
                .GetAll(module, language.Id)
                .Where(x =>
                    translationResults.All(y => y.Key != x.TranslationKey.Key)
                )
                .ToList();

            _translationValueRepository.DeleteRange(unusedSavedTranslationValues);

            _translationFileRepository.CreateOrUpdate(translationFile);
        }

        var unusedSavedTranslationFiles = savedTranslationFiles
            .Where(x => currentTranslationFilesPath.All(y => y != x.FilePath))
            .ToList();

        _translationValueRepository.DeleteAllByFiles(unusedSavedTranslationFiles);

        _translationFileRepository.RemoveRange(unusedSavedTranslationFiles);

        _translationKeyRepository.RemoveAllUnusedKeys();
    }

    private void SyncLanguages(string languagesFilePath)
    {
        var currentLanguages = GetCurrentLanguages(languagesFilePath);
        _languageRepository.ResetAllIsLockedLanguages();
        _languageRepository.CreateOrUpdate(currentLanguages);
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