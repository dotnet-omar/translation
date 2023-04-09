namespace Translation.Dto.TranslationValue;

public class TranslationValueSetDto
{
    public Guid TranslationKeyId { get; init; }
    public Guid LanguageId { get; init; }
    public string? Value { get; init; }
}