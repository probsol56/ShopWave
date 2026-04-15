using Scalar.AspNetCore;

namespace ShopWave.API.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiWithJwt(this IServiceCollection services)
    {
        services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer((document, context, ct) =>
            {
                document.Info = new()
                {
                    Title = "ShopWave API",
                    Version = "v1",
                    Description = "Multi-tenant Shop & Inventory Management API"
                };
                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static WebApplication UseOpenApiWithScalar(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = "ShopWave API";
            options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });
        return app;
    }
}
