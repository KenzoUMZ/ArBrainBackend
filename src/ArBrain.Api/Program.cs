using ArBrain.Application;
using ArBrain.Api.Extensions;
using ArBrain.Infrastructure;
using ArBrain.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? ["http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await DbInitializer.InitializeAsync(context);
    }
    catch (Exception ex) when (ex is Npgsql.NpgsqlException or TimeoutException)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("""
            ERRO: não foi possível conectar ao PostgreSQL em localhost:5432.

            Causa provável: Docker Desktop não está rodando ou o container do banco não foi iniciado.

            Passos:
              1. Abra o Docker Desktop e aguarde ficar "Running"
              2. Na pasta ArBrainBackend: docker compose up -d
              3. Verifique: docker compose ps
              4. Rode novamente: dotnet run --project src/ArBrain.Api

            """);
        Console.ResetColor();
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseArBrainMiddleware();
app.MapArBrainEndpoints();

app.Run();
