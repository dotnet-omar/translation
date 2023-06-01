using LinqKit;
using Microsoft.EntityFrameworkCore;
using Translation.Classes;
using Translation.Data;
using Translation.Dto.TranslationKey;
using Translation.Models;

namespace Translation.Repositories;

public class TranslationKeyRepository
{
    private readonly TranslationContext _dbContext;

    public TranslationKeyRepository(TranslationContext dbContext) => _dbContext = dbContext;

    public List<TranslationKey> ListAll() => _dbContext
        .TranslationKeys
        .Include(x => x.Values)
        .ToList();

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

    public void AddRange(IEnumerable<TranslationKey> translationKeys)
    {
        _dbContext.TranslationKeys.AddRange(translationKeys);
        _dbContext.SaveChanges();
    }

    public void RemoveRange(IEnumerable<TranslationKey> translationKeys)
    {
        _dbContext.TranslationKeys.RemoveRange(translationKeys);
        _dbContext.SaveChanges();
    }
}