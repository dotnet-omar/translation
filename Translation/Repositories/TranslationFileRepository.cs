using Translation.Data;
using Translation.Models;

namespace Translation.Repositories;

public class TranslationFileRepository
{
    private readonly TranslationContext _dbContext;

    public TranslationFileRepository(TranslationContext dbContext) => _dbContext = dbContext;

    public List<TranslationFile> List() => _dbContext.TranslationFiles.ToList();

    public void Create(TranslationFile translationFile) => _dbContext.TranslationFiles.Add(translationFile);


    public void Update(TranslationFile translationFile) => _dbContext.TranslationFiles.Update(translationFile);

    public void RemoveRange(IEnumerable<TranslationFile> translationFiles) =>
        _dbContext.TranslationFiles.RemoveRange(translationFiles);

    public void Commit() => _dbContext.SaveChanges();
}