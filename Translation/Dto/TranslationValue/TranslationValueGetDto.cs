using System.Linq.Expressions;
using LinqKit;
using Translation.Dto.Language;
using Translation.Dto.TranslationKey;

namespace Translation.Dto.TranslationValue;

public class TranslationValueGetDto
{
    public string? DefaultValue { get; set; }
    public string? OverriddenValue { get; set; }
    public LanguageLabelWithCodeDto Language { get; init; } = null!;
    public TranslationKeyWithoutValuesDto TranslationKey { get; init; } = null!;

    public static Expression<Func<Models.TranslationValue, TranslationValueGetDto>> Mapper()
    {
        return item => new TranslationValueGetDto
        {
            DefaultValue = item.DefaultValue,
            OverriddenValue = item.OverriddenValue,
            Language = LanguageLabelWithCodeDto.Mapper().Invoke(item.Language),
            TranslationKey = TranslationKeyWithoutValuesDto.Mapper().Invoke(item.TranslationKey),
        };
    }
}