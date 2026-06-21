using ArBrain.Api.Middleware;

namespace ArBrain.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseArBrainMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseCors("Frontend");
        app.UseHttpsRedirection();
        return app;
    }
}
