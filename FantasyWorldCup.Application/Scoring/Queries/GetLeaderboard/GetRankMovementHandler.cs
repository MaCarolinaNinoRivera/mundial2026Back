using FantasyWorldCup.Application.Scoring.Interfaces;
using FantasyWorldCup.Application.Auth.Interfaces;
namespace FantasyWorldCup.Application.Scoring.Queries.GetLeaderboard;

public class GetRankMovementHandler
{
    private readonly IScoringRepository _scoringRepo;
    private readonly IUserRepository _userRepo;

    public GetRankMovementHandler(IScoringRepository scoringRepo, IUserRepository userRepo)
    {
        _scoringRepo = scoringRepo;
        _userRepo = userRepo;
    }

    public async Task<List<RankMovementDto>> Handle()
    {
        // 1. Obtener los snapshots (las "fotos") de la tabla de historial
        // Equivalente a tus CTEs 'today' y 'yesterday'
        var todayHistory = await _scoringRepo.GetLatestRankHistoryAsync();
        var previousHistory = await _scoringRepo.GetPreviousRankHistoryAsync();

        // Traemos los usuarios para tener el Alias/Username
        var users = await _userRepo.GetAllActivePlayersAsync();

        // 2. Realizar el LEFT JOIN que tienes en tu SQL
        var movement = todayHistory.Select(t =>
        {
            var y = previousHistory.FirstOrDefault(prev => prev.UserId == t.UserId);
            var u = users.FirstOrDefault(user => user.Id == t.UserId);

            return new RankMovementDto
            {
                UserId = t.UserId,
                Alias = u?.Alias ?? u?.Username ?? "Desconocido",

                // Datos de Hoy
                CurrentPosition = t.RankPosition,
                TotalPoints = t.TotalPoints,

                // Datos de Ayer (COALESCE en tu SQL)
                PreviousPosition = y?.RankPosition ?? 0,

                // rank_change (t.rank_position - y.rank_position)
                // Nota: En SQL un cambio negativo (1 - 2 = -1) significa que MEJORÓ posición.
                // Para el DTO lo guardamos tal cual tu SQL lo pide:
                PointsDifference = t.TotalPoints - (y?.TotalPoints ?? 0)
            };
        })
        .OrderBy(x => x.CurrentPosition)
        .ToList();

        return movement;
    }
}
