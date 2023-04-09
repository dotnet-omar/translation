using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Translation.Data;
using Translation.Services;

namespace Translation.Extensions;

public static class ApplicationBuilderExtension
{
    public static void InitTranslations(this IApplicationBuilder app, string folderPath, string languagesFilePath)
    {
        var scopeFactoryService = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

        using var scope = scopeFactoryService.CreateScope();
        // Run Migrations
        var translationContextService = scope.ServiceProvider.GetRequiredService<TranslationContext>();
        translationContextService.Database.Migrate();

        var translationService = scope.ServiceProvider.GetRequiredService<TranslationService>();
        translationService.Sync(folderPath, languagesFilePath);
    }
}