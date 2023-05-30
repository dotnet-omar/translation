using Microsoft.EntityFrameworkCore;
using Translation.Data;

namespace Translation.Repositories;

public class TranslationRepository
{
    private readonly TranslationContext _dbContext;

    public TranslationRepository(TranslationContext dbContext) => _dbContext = dbContext;

    public Dictionary<string, string> GetTranslations(string module, string languageCode) => _dbContext
        .TranslationValues
        .Include(x => x.TranslationKey)
        .Where(x => x.Language.Code == languageCode)
        .Where(x => x.TranslationKey.Module == module)
        .ToDictionary(
            x => x.TranslationKey.Key,
            x => x.OverriddenValue ?? x.DefaultValue ?? ""
        );
}