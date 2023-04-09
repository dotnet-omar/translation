using Translation.Classes;
using Translation.Data;
using Translation.Dto.Language;
using Translation.Models;

namespace Translation.Repositories;

public class LanguageRepository
{
    private readonly TranslationContext _dbContext;

    public LanguageRepository(TranslationContext dbContext) => _dbContext = dbContext;

    public Pagination<LanguageListDto> List(
        string? keyword,
        int pageNumber,
        int pageSize
    )
    {
        IQueryable<Language> query = _dbContext
            .Languages
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Code);


        if (!string.IsNullOrEmpty(keyword))
        {
            keyword = keyword.Trim();
            query = query.Where(x => x.Label.Contains(keyword));
        }

        var total = query.Count();

        if (pageSize > 0)
        {
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        var list = new Pagination<LanguageListDto>()
        {
            Items = query
                .Select(LanguageListDto.Mapper())
                .ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            Total = total
        };

        return list;
    }

    public LanguageGetDto? Item(Guid? id, string? code)
    {
        return GetItemAsQueryable(id, code)
            .Select(LanguageGetDto.Mapper())
            .FirstOrDefault();
    }

    public LanguageGetDto? Create(LanguageCreateDto languageCreateDto)
    {
        var language = new Language()
        {
            Label = languageCreateDto.Label,
            Code = languageCreateDto.Code,
            IsRtl = languageCreateDto.IsRtl,
            Order = languageCreateDto.Order,
        };

        _dbContext.Languages.Add(language);
        _dbContext.SaveChanges();

        return Item(language.Id, null);
    }

    public LanguageGetDto? Update(Language language, LanguageUpdateDto languageUpdateDto)
    {
        language.Label = languageUpdateDto.Label;
        if (!language.IsLocked) language.Code = languageUpdateDto.Code;
        language.IsRtl = languageUpdateDto.IsRtl;
        language.Order = languageUpdateDto.Order;

        _dbContext.SaveChanges();

        return Item(language.Id, null);
    }

    public bool Delete(Guid id)
    {
        var language = _dbContext.Languages.FirstOrDefault(x => x.Id == id);

        if (language == null || language.IsLocked) return false;

        _dbContext.Languages.Remove(language);
        _dbContext.SaveChanges();

        return true;
    }

    public void CreateOrUpdate(List<Language> languages)
    {
        foreach (var language in languages)
        {
            var existingLanguage = _dbContext.Languages.FirstOrDefault(x => x.Code == language.Code);

            if (existingLanguage == null)
            {
                existingLanguage = new Language()
                {
                    Label = language.Label,
                    Code = language.Code,
                    IsRtl = language.IsRtl,
                    Order = language.Order,
                    IsLocked = language.IsLocked,
                };
                _dbContext.Languages.Add(existingLanguage);
            }
            else
            {
                existingLanguage.Label = language.Label;
                existingLanguage.IsRtl = language.IsRtl;
                existingLanguage.Order = language.Order;
                existingLanguage.IsLocked = language.IsLocked;
            }
        }

        _dbContext.SaveChanges();
    }

    public List<LanguageCodeDto> GetAllLanguages()
    {
        return _dbContext
            .Languages
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Code)
            .Select(LanguageCodeDto.Mapper())
            .ToList();
    }

    public IQueryable<Language> GetItemAsQueryable(Guid? id, string? code)
    {
        return _dbContext
            .Languages
            .Where(x =>
                id.HasValue ? x.Id == id : (!string.IsNullOrEmpty(code) && x.Code == code)
            );
    }

    public void ResetAllIsLockedLanguages()
    {
        var languages = _dbContext
            .Languages
            .ToList();

        foreach (var language in languages)
        {
            language.IsLocked = false;
        }

        _dbContext.Languages.UpdateRange(languages);
        _dbContext.SaveChanges();
    }
}