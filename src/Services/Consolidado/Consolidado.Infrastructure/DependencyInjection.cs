using Shared.Messaging.Configuration;
using Shared.Messaging.Interfaces;
using Shared.Messaging.Serialization;
using Consolidado.Application.Interfaces;
using Consolidado.Application.UseCases.ObterConsolidadoPorData;
using Consolidado.Application.UseCases.ProcessarLancamentoCriado;
using Consolidado.Infrastructure.BackgroundServices;
using Consolidado.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Consolidado.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddConsolidado(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

        services.AddDbContext<ConsolidadoDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IConsolidadoRepository, ConsolidadoRepository>();
        services.AddScoped<IProcessedMessageRepository, ProcessedMessageRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ObterConsolidadoPorDataHandler>();
        services.AddScoped<ProcessarLancamentoCriadoHandler>();

        services.AddSingleton<IMessageSerializer, SystemTextJsonMessageSerializer>();
        services.AddHostedService<LancamentoCriadoConsumerBackgroundService>();
        services.AddSingleton<IMessageSerializer, SystemTextJsonMessageSerializer>();

        return services;
    }
}
