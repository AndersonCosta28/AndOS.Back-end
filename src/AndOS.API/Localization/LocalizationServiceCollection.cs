using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace AndOS.API.Localization;

public static class LocalizationServiceCollection
{
    public static IServiceCollection AddLocalizationService(this IServiceCollection services) =>
        services.AddLocalization()
                .Configure<RequestLocalizationOptions>(options =>
        {
            string[] supportedCultures = ["en-US", "pt-BR"];
            options.DefaultRequestCulture = new RequestCulture("en-US");
            options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
            options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
            options.FallBackToParentCultures = true;
            options.FallBackToParentUICultures = true;
        });
}
