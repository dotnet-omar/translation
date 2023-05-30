using LinqKit;
using Microsoft.EntityFrameworkCore;
using Translation.Classes;
using Translation.Data;
using Translation.Dto.TranslationValue;
using Translation.Models;

namespace Translation.Repositories;

public class TranslationValueRepository
{
    private readonly TranslationContext _dbContext;

    public TranslationValueRepository(TranslationContext dbContext) => _dbContext = dbContext;

    public List<TranslationValue> ListAll() =>
        _dbContext
            .TranslationValues
            .Include(x => x.TranslationKey)
            .Include(x => x.Language)
            .ToList();

    public Pagination<TranslationValueGetDto> List(
        int pageNumber,
        int pageSize
    )
    {
        IQueryable<TranslationValue> query = _dbContext
            .TranslationValues
            .OrderBy(x => x.OverriddenValue)
            .ThenBy(x => x.DefaultValue);


        var total = query.Count();

        if (pageSize > 0)
        {
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        var list = new Pagination<TranslationValueGetDto>()
        {
            Items = query
                .AsExpandable()
                .Select(TranslationValueGetDto.Mapper())
                .ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            Total = total
        };

        return list;
    }

    public TranslationValueGetDto? Item(Guid languageId, Guid translationId) => _dbContext
        .TranslationValues
        .AsExpandable()
        .Where(x => x.LanguageId == languageId)
        .Where(x => x.TranslationKeyId == translationId)
        .Select(TranslationValueGetDto.Mapper())
        .FirstOrDefault();

    public TranslationValueGetDto? Create(Guid translationKeyId, Guid languageId, string value)
    {
        var translationValue = new TranslationValue()
        {
            TranslationKeyId = translationKeyId,
            LanguageId = languageId,
            OverriddenValue = value
        };
        _dbContext.TranslationValues.Add(translationValue);
        _dbContext.SaveChanges();

        return Item(languageId, translationKeyId);
    }

    public TranslationValueGetDto? Update(TranslationValue translationValue, string? value)
    {
        translationValue.OverriddenValue = value == translationValue.DefaultValue ? null : value;
        _dbContext.TranslationValues.Update(translationValue);
        _dbContext.SaveChanges();
        return Item(translationValue.LanguageId, translationValue.TranslationKeyId);
    }

    public void Delete(TranslationValue translationValue)
    {
        _dbContext.TranslationValues.Remove(translationValue);
        _dbContext.SaveChanges();
    }

    public TranslationValue? GetItemAsDefault(Guid languageId, Guid translationId) => _dbContext
        .TranslationValues
        .Include(x => x.TranslationKey)
        .Where(x => x.LanguageId == languageId)
        .FirstOrDefault(x => x.TranslationKeyId == translationId);

    public void AddRange(IEnumerable<TranslationValue> translationValues) =>
        _dbContext.TranslationValues.AddRange(translationValues);

    public void UpdateRange(IEnumerable<TranslationValue> translationValues) =>
        _dbContext.TranslationValues.UpdateRange(translationValues);

    public void DeleteRange(IEnumerable<TranslationValue> translationValues) =>
        _dbContext.TranslationValues.RemoveRange(translationValues);

    public void DeleteAllByFiles(
        IEnumerable<TranslationFile> translationFiles,
        IEnumerable<TranslationValue> savedTranslationsValue
    )
    {
        var items = translationFiles.Select(x => x.GetFileInfo()).ToList();

        var translationValuesQuery = savedTranslationsValue
            .Where(x =>
                items.Any(y => y.languageCode == x.Language.Code && y.module == x.TranslationKey.Module)
            )
            .ToList();

        _dbContext.TranslationValues.RemoveRange(translationValuesQuery);
    }

    public void Commit() => _dbContext.SaveChanges();
}