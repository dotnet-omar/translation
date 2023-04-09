using Microsoft.AspNetCore.Mvc;
using Translation.Extensions;
using Translation.Interfaces;
using Translation.Repositories;

namespace Translation.Controllers;

[ApiController]
[Route("api/v1/translations")]
public class TranslationController : ControllerBase
{
    private readonly TranslationRepository _repository;

    public TranslationController(TranslationRepository repository) => _repository = repository;

    [HttpGet("{module}/{languageCode}")]
    public IApiResponse<Dictionary<string, string>> List(string module, string languageCode)
    {
        var list = _repository.GetTranslations(module, languageCode);
        return this.ApiResponse(200, "Items fetched successfully", list);
    }
}