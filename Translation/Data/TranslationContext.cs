using Microsoft.EntityFrameworkCore;
using Translation.Models;

namespace Translation.Data;

public class TranslationContext : DbContext
{
    public TranslationContext(DbContextOptions<TranslationContext> options) : base(options)
    {
    }

    public DbSet<Language> Languages { get; set; } = null!;
    public DbSet<TranslationKey> TranslationKeys { get; set; } = null!;
    public DbSet<TranslationValue> TranslationValues { get; set; } = null!;
    public DbSet<TranslationFile> TranslationFiles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<TranslationValue>()
            .HasKey(x => new { x.TranslationKeyId, x.LanguageId });
    }
}