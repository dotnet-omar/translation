using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Translation.Models;

[Table("translation_files")]
[Microsoft.EntityFrameworkCore.Index(nameof(FilePath), IsUnique = true)]
public class TranslationFile
{
    [Key] [Column("id")] public Guid Id { get; set; }
    [Column("file_path")] public string FilePath { get; set; } = null!;
    [Column("file_hash")] public string FileHash { get; set; } = null!;
    
    public (string languageCode, string module) GetFileInfo()
    {
        return (
            languageCode: Path.GetFileNameWithoutExtension(FilePath),
            module: Path.GetFileName(Path.GetDirectoryName(FilePath))!
        );
    }
}