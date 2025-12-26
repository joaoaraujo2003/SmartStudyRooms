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

                    // Fim da reserva
                    var salasReservaExpirada = salaRepo.SalasComReservaExpirada();
                    foreach (var salaId in salasReservaExpirada)
                        salaRepo.LibertarSala(salaId);

                    // Sem movimento há +10 min
                    var salasSemMovimento = sensorRepo.SalasSemMovimentoHaMaisDe10Min();
                    foreach (var salaId in salasSemMovimento)
                        salaRepo.LibertarSala(salaId);

                    // Histórico 
                    reservaRepo.ReservasExpiradas();
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

    }
}

