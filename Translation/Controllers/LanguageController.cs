using Microsoft.AspNetCore.Mvc;
using Translation.Classes;
using Translation.Dto.Language;
using Translation.Extensions;
using Translation.Interfaces;
using Translation.Repositories;

namespace Translation.Controllers;

[ApiController]
[Route("api/v1/languages")]
public class LanguageController : ControllerBase
{
    private readonly LanguageRepository _repository;

    public LanguageController(LanguageRepository repository) => _repository = repository;

    [HttpGet]
    public IApiResponse<Pagination<LanguageListDto>> List(
        string? keyword,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        var list = _repository.List(keyword, pageNumber, pageSize);
        return this.ApiResponse(200, "Items fetched successfully", list);
    }

    [HttpGet("{id:Guid}")]
    public IApiResponse<LanguageGetDto> Item(Guid id)
    {
        var language = _repository.Item(id, null);

        return language == null
            ? this.ApiResponse<LanguageGetDto>(404, "Language Not Found")
            : this.ApiResponse(200, "Item fetched successfully", language);
    }

    [HttpPost]
    public IApiResponse<LanguageGetDto> Create(LanguageCreateDto languageCreateDto)
    {
        var language = _repository.Create(languageCreateDto);

        return language == null
            ? this.ApiResponse<LanguageGetDto>(404, "Cannot create language")
            : this.ApiResponse(201, "Item created successfully", language);
    }


    [HttpPut("{id:Guid}")]
    public IApiResponse<LanguageGetDto> Update(Guid id, LanguageUpdateDto languageUpdateDto)
    {
        var language = _repository.GetItemAsQueryable(id, null).FirstOrDefault();

        if (language == null)
            return this.ApiResponse<LanguageGetDto>(404, "Item not found");

        if (language.IsLocked)
            return this.ApiResponse<LanguageGetDto>(400, "Cannot update default languages");

        var updatedLanguage = _repository.Update(language, languageUpdateDto);

        return this.ApiResponse(200, "Item updated successfully", updatedLanguage);
    }

    [HttpDelete("{id:Guid}")]
    public IApiResponse<string> Delete(Guid id)
    {
        var result = _repository.Delete(id);

        return result
            ? this.ApiResponse<string>(204, "Item Deleted Successfully")
            : this.ApiResponse<string>(404, "Language not found");
    }
}