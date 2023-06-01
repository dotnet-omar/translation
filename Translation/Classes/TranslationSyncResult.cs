using Translation.Models;

namespace Translation.Classes;

public class TranslationSyncResult
{
    public List<TranslationValue> AllNewTranslationsValue { get; } = new();
    public List<TranslationKey> AllNewTranslationsKey { get; } = new();
    public List<TranslationFile> AllNewTranslationsFile { get; } = new();
    public List<TranslationValue> AllUnusedTranslationsValue { get; } = new();
    public List<TranslationKey> AllUnusedTranslationsKey { get; set; } = new();
    public List<TranslationFile> AllUnusedTranslationsFile { get; set; } = new();
}