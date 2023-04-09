using Microsoft.EntityFrameworkCore;
using Translation.Constants;
using Translation.Models;

namespace Translation.Data;

public class TranslationContext : DbContext
{
    public DbSet<Language> Languages { get; set; } = null!;
    public DbSet<TranslationKey> TranslationKeys { get; set; } = null!;
    public DbSet<TranslationValue> TranslationValues { get; set; } = null!;
    public DbSet<TranslationFile> TranslationFiles { get; set; } = null!;

    public TranslationContext(DbContextOptions<TranslationContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DbOptions.Schema);

        modelBuilder
            .Entity<TranslationValue>()
            .HasKey(x => new { x.TranslationKeyId, x.LanguageId });
    }
}