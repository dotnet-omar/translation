using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Translation.Constants;
using Translation.Data;
using Translation.Repositories;
using Translation.Services;

namespace Translation.Extensions;

public static class ServiceExtension
{
    public static void AddTranslation(
        this IServiceCollection services,
        string connectionString
    )
    {
        // Services
        services.AddScoped<TranslationService>();
        services.AddScoped<LanguageRepository>();
        services.AddScoped<TranslationKeyRepository>();
        services.AddScoped<TranslationValueRepository>();
        services.AddScoped<TranslationFileRepository>();
        services.AddScoped<TranslationRepository>();

        var migrationsAssembly = typeof(TranslationService).GetTypeInfo().Assembly.GetName().Name;

        services.AddDbContext<TranslationContext>((options) =>
            options.UseSqlServer(
                connectionString,
                x =>
                {
                    x.MigrationsAssembly(migrationsAssembly);
                    x.MigrationsHistoryTable("__MyMigrationsHistory", DbOptions.Schema);
                }
            )
        );
    }
}