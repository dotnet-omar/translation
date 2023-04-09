using Microsoft.AspNetCore.Mvc;
using Translation.Classes;
using Translation.Dto.TranslationKey;
using Translation.Extensions;
using Translation.Interfaces;
using Translation.Repositories;

namespace Translation.Controllers;

[ApiController]
[Route("api/v1/translation-keys")]
public class TranslationKeyController : ControllerBase
{
    private readonly TranslationKeyRepository _repository;

    public TranslationKeyController(TranslationKeyRepository repository) => _repository = repository;

    [HttpGet]
    public IApiResponse<Pagination<TranslationKeyListDto>> List(
        Guid languageId,
        string? key,
        string? value,
        string? module,
        bool? isChangedOnly,
        bool? isEqualToValue,
        bool? hasNoValue,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        var list = _repository.List(
            languageId,
            key,
            value,
            module,
            isChangedOnly,
            isEqualToValue,
            hasNoValue,
            pageNumber,
            pageSize
        );

        return this.ApiResponse(200, "Items fetched successfully", list);
    }

    [HttpGet("{id:Guid}")]
    public IApiResponse<TranslationKeyGetDto> Item(Guid id)
    {
        var translationKey = _repository.Item(id);

        return translationKey == null
            ? this.ApiResponse<TranslationKeyGetDto>(404, "Translation key Not Found")
            : this.ApiResponse(200, "Item fetched successfully", translationKey);
    }
}