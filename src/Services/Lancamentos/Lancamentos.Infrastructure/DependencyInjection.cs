using Shared.Messaging.Configuration;
using Shared.Messaging.Interfaces;
using Shared.Messaging.Serialization;
using Lancamentos.Application.Intefaces;
using Lancamentos.Application.UseCases.CriarLancamento;
using Lancamentos.Application.UseCases.ListarLancamentos;
using Lancamentos.Application.UseCases.ObterLancamentoPorId;
using Lancamentos.Infrastructure.BackgroundServices;
using Lancamentos.Infrastructure.Messaging;
using Lancamentos.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lancamentos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddLancamentos(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

        services.AddDbContext<LancamentosDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ILancamentoRepository, LancamentoRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<CriarLancamentoHandler>();
        services.AddScoped<ObterLancamentoPorIdHandler>();
        services.AddScoped<ListarLancamentosHandler>();

        services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
        services.AddHostedService<OutboxPublisherBackgroundService>();
        services.AddSingleton<IMessageSerializer, SystemTextJsonMessageSerializer>();

        return services;
    }
}
