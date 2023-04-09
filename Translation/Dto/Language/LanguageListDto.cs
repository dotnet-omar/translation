using System.Linq.Expressions;

namespace Translation.Dto.Language;

public class LanguageListDto
{
    public Guid Id { get; init; }
    public string Label { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int? Order { get; set; }
    public bool IsRtl { get; set; }
    public bool IsLocked { get; set; }
    public int TranslationValuesCount { get; set; }

    public static Expression<Func<Models.Language, LanguageListDto>> Mapper()
    {
        return item => new LanguageListDto
        {
            Id = item.Id,
            Label = item.Label,
            Code = item.Code,
            Order = item.Order,
            IsRtl = item.IsRtl,
            IsLocked = item.IsLocked,
            TranslationValuesCount = item.TranslationValues.Count
        };
    }
}