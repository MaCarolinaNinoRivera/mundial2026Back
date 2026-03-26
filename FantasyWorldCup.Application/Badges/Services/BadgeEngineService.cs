using FantasyWorldCup.Application.Badges.Interfaces;
using FantasyWorldCup.Domain.Badges.Entities;
using FantasyWorldCup.Domain.Scoring.Entities;

namespace FantasyWorldCup.Application.Badges.Services;

public class BadgeEngineService
{
    private readonly IBadgeRepository _badgeRepository;

    public BadgeEngineService(IBadgeRepository badgeRepository)
    {
        _badgeRepository = badgeRepository;
    }

    public async Task<bool> CheckAndAwardEarlyBirdAsync(Guid userId, DateTime predictionTime, DateTime matchDate)
    {
        // 1. Verificar si es madrugador (24h antes)
        // CAMBIO: return false en lugar de return solo
        if ((matchDate - predictionTime).TotalHours < 24) return false;

        // 2. Obtener la medalla de la DB
        var badge = await _badgeRepository.GetBadgeByCriteriaAsync("EarlyBird");
        // CAMBIO: return false
        if (badge == null) return false;

        // 3. Verificar si ya la tiene
        // CAMBIO: return false
        if (await _badgeRepository.UserHasBadgeAsync(userId, badge.Id)) return false;

        // 4. Crear el registro de la medalla
        var userBadgeId = Guid.NewGuid();
        var userBadge = new UserBadge
        {
            Id = userBadgeId,
            UserId = userId,
            BadgeId = badge.Id,
            AwardedAt = DateTime.UtcNow
        };

        // 5. Crear el punto extra en el Ledger
        var pointEntry = new UserPointsLedger
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Points = 1,
            SourceType = "Badge",
            SourceId = userBadgeId,
            CreatedAt = DateTime.UtcNow
        };

        await _badgeRepository.AwardBadgeAsync(userBadge, pointEntry);

        // 6. ¡Éxito! Devolvemos true
        return true;
    }

    // En src/FantasyWorldCup.Application/Badges/Services/BadgeEngineService.cs

    public async Task<bool> CheckAndAwardTriviaExpertAsync(Guid userId, int currentStreak)
    {
        // 1. ¿Llegó a la racha de 10?
        if (currentStreak < 10) return false;

        // 2. Buscar la medalla en la DB
        var badge = await _badgeRepository.GetBadgeByCriteriaAsync("TriviaExpert");
        if (badge == null) return false;

        // 3. ¿Ya la tiene? (Las medallas normalmente se ganan una sola vez)
        if (await _badgeRepository.UserHasBadgeAsync(userId, badge.Id)) return false;

        // 4. Crear registros (Medalla + 1 Punto extra por el logro)
        var userBadgeId = Guid.NewGuid();
        var userBadge = new UserBadge
        {
            Id = userBadgeId,
            UserId = userId,
            BadgeId = badge.Id,
            AwardedAt = DateTime.UtcNow
        };

        var pointEntry = new UserPointsLedger
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Points = 1, // Tu regla de 1 punto extra
            SourceType = "Badge",
            SourceId = userBadgeId,
            CreatedAt = DateTime.UtcNow
        };

        await _badgeRepository.AwardBadgeAsync(userBadge, pointEntry);
        return true;
    }
}
