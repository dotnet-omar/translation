using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Translation.Models;

[Table("languages")]
[Microsoft.EntityFrameworkCore.Index(nameof(Code), IsUnique = true)]
public class Language
{
    [Key] [Column("id")]  public Guid Id { get; init; }
    [Column("label")] public string Label { get; set; } = null!;
    [Column("code")] [MaxLength(10)]  public string Code { get; set; } = null!;
    [Column("is_rtl")] public bool IsRtl { get; set; }
    [Column("is_locked")] public bool IsLocked { get; set; }
    [Column("order")] public int? Order { get; set; }
    [InverseProperty("Language")] public ICollection<TranslationValue> TranslationValues { get; set; } = null!;
}