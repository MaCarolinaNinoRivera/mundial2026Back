using FantasyWorldCup.Application.Scoring.UseCases.RecordRankSnapshot;

namespace FantasyWorldCup.Api.BackgroundServices;

public class RankSnapshotWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RankSnapshotWorker> _logger;
    private const int ExecutionHourColombia = 17; // 5 PM

    public RankSnapshotWorker(IServiceProvider serviceProvider, ILogger<RankSnapshotWorker> _logger)
    {
        _serviceProvider = serviceProvider;
        this._logger = _logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de Snapshot de Ranking iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            // Obtener hora actual en Colombia (UTC-5)
            var colombiaTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                DateTimeOffset.UtcNow, "SA Pacific Standard Time");

            // Si es la hora exacta (17:00) y no se ha ejecutado hoy
            if (colombiaTime.Hour == ExecutionHourColombia && colombiaTime.Minute == 0)
            {
                _logger.LogInformation("Iniciando snapshot diario de ranking (5 PM Colombia)...");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var handler = scope.ServiceProvider.GetRequiredService<RecordRankSnapshotHandler>();
                    await handler.ExecuteAsync();
                }

                _logger.LogInformation("Snapshot completado con éxito.");

                // Esperar un minuto para no ejecutarlo varias veces en el mismo minuto
                await Task.Delay(TimeSpan.FromMinutes(1.1), stoppingToken);
            }

            // Revisar cada minuto
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
