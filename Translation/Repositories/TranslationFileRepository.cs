using Translation.Data;
using Translation.Models;

namespace Translation.Repositories;

public class TranslationFileRepository
{
    private readonly TranslationContext _dbContext;

    public TranslationFileRepository(TranslationContext dbContext) => _dbContext = dbContext;

    public List<TranslationFile> List()
    {
        return _dbContext
            .TranslationFiles
            .ToList();
    }

    public TranslationFile? Item(string? filePath)
    {
        return _dbContext
            .TranslationFiles
            .FirstOrDefault(x => x.FilePath == filePath);
    }

    public void CreateOrUpdate(TranslationFile translationFile)
    {
        var existingTranslationFile = Item(translationFile.FilePath);

        if (existingTranslationFile == null) _dbContext.TranslationFiles.Add(translationFile);
        else
        {
            existingTranslationFile.FileHash = translationFile.FileHash;
            _dbContext.TranslationFiles.Update(existingTranslationFile);
        }

        _dbContext.SaveChanges();
    }

    public void RemoveRange(IEnumerable<TranslationFile> translationFiles)
    {
        _dbContext.TranslationFiles.RemoveRange(translationFiles);
        _dbContext.SaveChanges();
    }
}