using Shared.Security.Extensions;
using Consolidado.Infrastructure;
using Consolidado.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerJwt();
builder.Services.AddJwtSecurity(builder.Configuration);

builder.Services.AddConsolidado(builder.Configuration);

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<ConsolidadoDbContext>("sqlserver");

var app = builder.Build();

await EnsureDatabaseReadyAsync(app.Services, app.Logger);

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

static async Task EnsureDatabaseReadyAsync(IServiceProvider services, ILogger logger)
{
    using var scope = services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ConsolidadoDbContext>();

    for (var attempt = 1; attempt <= 30; attempt++)
    {
        try
        {
            await dbContext.Database.EnsureCreatedAsync();
            logger.LogInformation("Banco de Consolidado inicializado com sucesso.");
            return;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Tentativa {Attempt} de inicializar o banco de Consolidado falhou.", attempt);
            await Task.Delay(TimeSpan.FromSeconds(4));
        }
    }

    throw new InvalidOperationException("Não foi possível inicializar o banco de Consolidado.");
}

public partial class Program;
