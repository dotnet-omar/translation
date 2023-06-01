using Translation.Data;
using Translation.Models;

namespace Translation.Repositories;

public class TranslationFileRepository
{
    private readonly TranslationContext _dbContext;

    public TranslationFileRepository(TranslationContext dbContext) => _dbContext = dbContext;

    public List<TranslationFile> ListAll() => _dbContext.TranslationFiles.ToList();

    public void AddRange(IEnumerable<TranslationFile> translationFiles)
    {
        _dbContext.TranslationFiles.AddRange(translationFiles);
        _dbContext.SaveChanges();
    }

    public void RemoveRange(IEnumerable<TranslationFile> translationFiles)
    {
        _dbContext.TranslationFiles.RemoveRange(translationFiles);
        _dbContext.SaveChanges();
    }
}