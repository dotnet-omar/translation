using System.Linq.Expressions;
using LinqKit;
using Translation.Dto.Language;

namespace Translation.Dto.TranslationValue;

public class TranslationValueWithLanguageDto
{
    public string? DefaultValue { get; set; }
    public string? OverriddenValue { get; set; }
    public LanguageLabelWithCodeDto Language { get; set; } = null!;

    public static Expression<Func<Models.TranslationValue, TranslationValueWithLanguageDto>> Mapper()
    {
        return item => new TranslationValueWithLanguageDto
        {
            DefaultValue = item.DefaultValue,
            OverriddenValue = item.OverriddenValue,
            Language = LanguageLabelWithCodeDto.Mapper().Invoke(item.Language),
        };
    }
}