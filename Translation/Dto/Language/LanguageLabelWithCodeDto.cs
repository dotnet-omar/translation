using System.Linq.Expressions;

namespace Translation.Dto.Language;

public class LanguageLabelWithCodeDto
{
    public Guid Id { get; init; }
    public string Label { get; set; } = null!;
    public string Code { get; set; } = null!;

    public static Expression<Func<Models.Language, LanguageLabelWithCodeDto>> Mapper()
    {
        return item => new LanguageLabelWithCodeDto
        {
            Id = item.Id,
            Label = item.Label,
            Code = item.Code
        };
    }
}