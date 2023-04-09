using System.Linq.Expressions;

namespace Translation.Dto.TranslationKey;

public class TranslationKeyListDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = null!;
    public string Module { get; set; } = null!;
    public string? DefaultValue { get; set; }
    public string? OverriddenValue { get; set; }
    public int ValuesCount { get; set; }

    public static Expression<Func<Models.TranslationKey, TranslationKeyListDto>> Mapper(Guid languageId)
    {
        return item => new TranslationKeyListDto
        {
            Id = item.Id,
            Key = item.Key,
            Module = item.Module,
            DefaultValue = item.Values
                .Where(x => x.Language.Id == languageId)
                .Select(x => x.DefaultValue)
                .FirstOrDefault(),
            OverriddenValue = item.Values
                .Where(x => x.Language.Id == languageId)
                .Select(x => x.OverriddenValue)
                .FirstOrDefault(),
            ValuesCount = item.Values.Count
        };
    }
}