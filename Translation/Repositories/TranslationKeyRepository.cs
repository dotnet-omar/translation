using LinqKit;
using Translation.Classes;
using Translation.Data;
using Translation.Dto.TranslationKey;
using Translation.Models;

namespace Translation.Repositories;

public class TranslationKeyRepository
{
    private readonly TranslationContext _dbContext;

    public TranslationKeyRepository(TranslationContext dbContext) => _dbContext = dbContext;

    public Pagination<TranslationKeyListDto> List(
        Guid languageId,
        string? key = null,
        string? value = null,
        string? module = null,
        bool? isChangedOnly = null,
        bool? isEqualToValue = null,
        bool? hasNoValue = null,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        var query = _dbContext
            .TranslationKeys
            .Where(x => module == null || x.Module == module);

        if (!string.IsNullOrEmpty(key))
        {
            key = key.Trim();
            query = query.Where(x =>
                isEqualToValue == true ? x.Key.Equals(key) : x.Key.Contains(key)
            );
        }

        if (!string.IsNullOrEmpty(value))
        {
            value = value.Trim();

            query = query
                .Where(x =>
                    x.Values.Any(y =>
                        y.LanguageId == languageId &&
                        (
                            y.DefaultValue != null &&
                            (isEqualToValue == true
                                ? y.DefaultValue.Equals(value)
                                : y.DefaultValue.Contains(value))
                        )
                        ||
                        (
                            y.OverriddenValue != null &&
                            (isEqualToValue == true
                                ? y.OverriddenValue.Equals(value)
                                : y.OverriddenValue.Contains(value))
                        )
                    )
                );
        }

        if (isChangedOnly == true)
        {
            query = query
                .Where(x =>
                    x.Values.Any(y =>
                        y.LanguageId == languageId &&
                        y.DefaultValue != null &&
                        y.OverriddenValue != null
                    )
                );
        }

        if (hasNoValue == true)
        {
            query = query
                .Where(x =>
                    x.Values.All(y => y.LanguageId != languageId)
                );
        }

        var total = query.Count();

        if (pageSize > 0)
        {
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        var list = new Pagination<TranslationKeyListDto>()
        {
            Items = query
                .AsExpandable()
                .Select(TranslationKeyListDto.Mapper(languageId))
                .OrderBy(x => x.OverriddenValue)
                .ThenBy(x => x.DefaultValue)
                .ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            Total = total,
        };

        return list;
    }

    public TranslationKeyGetDto? Item(Guid id)
    {
        return _dbContext
            .TranslationKeys
            .AsExpandable()
            .Where(x => x.Id == id)
            .Select(TranslationKeyGetDto.Mapper())
            .FirstOrDefault();
    }

    public TranslationKey? GetItemBy(string key, string module)
    {
        return _dbContext
            .TranslationKeys
            .FirstOrDefault(x => x.Key == key && x.Module == module);
    }


    public List<TranslationKey> GetAllBy(string module)
    {
        return _dbContext
            .TranslationKeys
            .Where(x => x.Module == module)
            .ToList();
    }

    public TranslationKey CreateItemFromResult(string key, string module)
    {
        var translationKey = new TranslationKey()
        {
            Key = key,
            Module = module
        };

        _dbContext.TranslationKeys.Add(translationKey);
        _dbContext.SaveChanges();
        return translationKey;
    }


    public void AddRange(IEnumerable<TranslationKey> translationKeys)
    {
        _dbContext.TranslationKeys.AddRange(translationKeys);
        _dbContext.SaveChanges();
    }

    public void RemoveAllUnusedKeys()
    {
        var unusedTranslationKeys = _dbContext
            .TranslationKeys
            .Where(x => !x.Values.Any() || x.Values.All(y => y.DefaultValue == null));

        _dbContext.TranslationKeys.RemoveRange(unusedTranslationKeys);
        _dbContext.SaveChanges();
    }
}