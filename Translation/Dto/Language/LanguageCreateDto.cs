namespace Translation.Dto.Language;

public class LanguageCreateDto
{
    public string Label { get; set; } = null!;
    public string Code { get; set; } = null!;
    public bool IsRtl { get; set; }
    public int? Order { get; set; }
}