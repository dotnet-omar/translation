using Microsoft.AspNetCore.Mvc;
using Translation.Classes;
using Translation.Dto.TranslationValue;
using Translation.Extensions;
using Translation.Interfaces;
using Translation.Repositories;

namespace Translation.Controllers;

[ApiController]
[Route("api/v1/translation-values")]
public class TranslationValueController : ControllerBase
{
    private readonly TranslationValueRepository _repository;

    public TranslationValueController(TranslationValueRepository repository) => _repository = repository;

    [HttpGet]
    public IApiResponse<Pagination<TranslationValueGetDto>> List(
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        var list = _repository.List(pageNumber, pageSize);
        return this.ApiResponse(200, "Items fetched successfully", list);
    }

    [HttpGet("{languageId:guid}/{translationId:guid}")]
    public IApiResponse<TranslationValueGetDto> Item(Guid languageId, Guid translationId)
    {
        var translationValue = _repository.Item(languageId, translationId);

        return translationValue == null
            ? this.ApiResponse<TranslationValueGetDto>(404, "Translation value Not Found")
            : this.ApiResponse(200, "Item fetched successfully", translationValue);
    }

    [HttpPut]
    public IApiResponse<TranslationValueGetDto> Set(
        TranslationValueSetDto translationValueSetDto
    )
    {
        var value = translationValueSetDto.Value?.Trim();
        value = string.IsNullOrEmpty(value) ? null : value;

        var translationValue = _repository
            .GetItemAsDefault(translationValueSetDto.LanguageId, translationValueSetDto.TranslationKeyId);

        var status = GetSetStatus(
            translationValue != null,
            value != null,
            translationValue?.DefaultValue != null
        );

        TranslationValueGetDto? translationValueGetDto = null;

        switch (status)
        {
            case "create":
                translationValueGetDto = _repository.Create(
                    translationValueSetDto.TranslationKeyId,
                    translationValueSetDto.LanguageId,
                    value!
                );
                break;

            case "update":
                translationValueGetDto = _repository.Update(translationValue!, value);
                break;

            case "delete":
                _repository.Delete(translationValue!);
                break;

            default:
                return this.ApiResponse<TranslationValueGetDto>(400, "Bad Request");
        }

        return this.ApiResponse(200, "Updated Successfully", translationValueGetDto);
    }

    private static string GetSetStatus(
        bool isTranslationExist,
        bool isValueExist,
        bool isDefaultValueExist
    ) => (isTranslationExist, isValueExist, isDefaultValueExist) switch
    {
        (false, true, _) => "create",
        (true, false, false) => "delete",
        (true, _, _) => "update",
        _ => "null",
    };
}