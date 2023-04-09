using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Translation.Data;
using Translation.Repositories;
using Translation.Services;

namespace Translation.Extensions;

public static class ServiceExtension
{
    public static void AddTranslation(this IServiceCollection services, string connectionString)
    {
        // Services
        services.AddScoped<TranslationService>();
        services.AddScoped<LanguageRepository>();
        services.AddScoped<TranslationKeyRepository>();
        services.AddScoped<TranslationValueRepository>();
        services.AddScoped<TranslationFileRepository>();
        services.AddScoped<TranslationRepository>();

        services.AddDbContext<TranslationContext>(options =>
            options.UseSqlServer(connectionString)
        );
    }
}