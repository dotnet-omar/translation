using System.ComponentModel.DataAnnotations.Schema;

namespace Translation.Models;

[Table("translation_values")]
public class TranslationValue
{
    [Column("default_value")] public string? DefaultValue { get; set; }
    [Column("overridden_value")] public string? OverriddenValue { get; set; }

    [Column("language_id")]
    [ForeignKey("Language")]
    public Guid LanguageId { get; set; }

    public Language Language { get; set; } = null!;

    [Column("translation_key_id")]
    [ForeignKey("TranslationKey")]
    public Guid TranslationKeyId { get; set; }

    public TranslationKey TranslationKey { get; set; } = null!;
}