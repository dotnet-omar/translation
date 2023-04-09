using System.Linq.Expressions;

namespace Translation.Dto.TranslationKey;

public class TranslationKeyWithoutValuesDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = null!;
    public string Module { get; set; } = null!;

    public static Expression<Func<Models.TranslationKey, TranslationKeyWithoutValuesDto>> Mapper()
    {
        return item => new TranslationKeyWithoutValuesDto
        {
            Id = item.Id,
            Key = item.Key,
            Module = item.Module,
        };
    }
}