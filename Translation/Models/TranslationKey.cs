using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Translation.Models;

[Table("translation_keys")]
public class TranslationKey
{
    [Key] [Column("id")] public Guid Id { get; set; }
    [Column("key")] public string Key { get; set; } = null!;
    [Column("module")] public string Module { get; set; } = null!;
    [InverseProperty("TranslationKey")] public ICollection<TranslationValue> Values { get; set; } = null!;
}