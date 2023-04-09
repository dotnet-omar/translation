using System.Linq.Expressions;

namespace Translation.Dto.Language;

public class LanguageCodeDto
{
    public Guid Id { get; init; }
    public string Code { get; set; } = null!;

    public static Expression<Func<Models.Language, LanguageCodeDto>> Mapper()
    {
        return item => new LanguageCodeDto
        {
            Id = item.Id,
            Code = item.Code,
        };
    }
}