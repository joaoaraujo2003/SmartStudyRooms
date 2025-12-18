using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using SmartStudyRooms.Data.Repositories;
using System.Threading;
using System;
using SmartStudyRooms.Data.Models;

namespace SmartStudyRooms.API.Services
{
 
    public class SalaCleanUpService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SalaCleanUpService> _logger;

        public SalaCleanUpService
        (
            IServiceScopeFactory scopeFactory,
            ILogger<SalaCleanUpService> logger
        )
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SalaCleanUpService iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var salaRepo = scope.ServiceProvider.GetRequiredService<SalaRepository>();
                    var sensorRepo = scope.ServiceProvider.GetRequiredService<SensorRepository>();
                    var reservaRepo = scope.ServiceProvider.GetRequiredService<ReservaRepository>();

                    // REGRA A — fim da reserva
                    int libertadasPorTempo = salaRepo.LibertarSalasPorFimReserva();

                    // REGRA B — sensor inativo 15 min
                    var salasInativas = sensorRepo.SalasInativasHaMaisDe15Min();
                    foreach (var salaId in salasInativas)
                    {
                        salaRepo.LibertarSala(salaId);
                    }

                    // histórico    
                    reservaRepo.ReservasExpiradas();
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

    }
}

