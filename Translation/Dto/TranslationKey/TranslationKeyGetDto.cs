using System.Linq.Expressions;
using LinqKit;
using Translation.Dto.TranslationValue;

namespace Translation.Dto.TranslationKey;

public class TranslationKeyGetDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = null!;
    public string Module { get; set; } = null!;
    public int ValuesCount { get; set; }
    public IEnumerable<TranslationValueWithLanguageDto> Values { get; set; } = null!;

    public static Expression<Func<Models.TranslationKey, TranslationKeyGetDto>> Mapper()
    {
        return item => new TranslationKeyGetDto
        {
            Id = item.Id,
            Key = item.Key,
            Module = item.Module,
            ValuesCount = item.Values.Count,
            Values = item.Values
                .Select(x => TranslationValueWithLanguageDto.Mapper().Invoke(x))
                .ToList(),
        };
    }
}