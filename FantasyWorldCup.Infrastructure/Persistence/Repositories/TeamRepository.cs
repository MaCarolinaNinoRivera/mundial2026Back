using Microsoft.EntityFrameworkCore;
using FantasyWorldCup.Application.Teams.Interfaces;
using FantasyWorldCup.Domain.Teams.Entities;
using FantasyWorldCup.Domain.Countries.Entities;

namespace FantasyWorldCup.Infrastructure.Persistence.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly AppDbContext _context;

    public TeamRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserTeam?> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserTeams
            .Include(t => t.TeamPlayers)
                .ThenInclude(tp => tp.Player)
                    .ThenInclude(p => p.Country) // Asegúrate de que esté este Include
            .FirstOrDefaultAsync(t => t.UserId == userId);
    }

    public async Task<List<UserTeamPlayer>> GetTeamPlayersAsync(Guid userTeamId)
        => await _context.UserTeamPlayers
            .Where(tp => tp.UserTeamId == userTeamId)
            .ToListAsync();

    public async Task<FootballPlayer?> GetPlayerByIdAsync(Guid playerId)
    {
        return await _context.FootballPlayers.FindAsync(playerId);
    }

    public async Task AddPlayerToTeamAsync(UserTeamPlayer teamPlayer)
    {
        await _context.UserTeamPlayers.AddAsync(teamPlayer);
    }

    public async Task AddTeamAsync(UserTeam team)
    {
        await _context.UserTeams.AddAsync(team);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<UserTeam?> GetTeamByUserIdAsync(Guid userId)
    {
        return await _context.UserTeams.FirstOrDefaultAsync(t => t.UserId == userId);
    }

    public async Task<UserTeamPlayer?> GetPlayerInTeamAsync(Guid teamId, Guid footballPlayerId)
    {
        return await _context.UserTeamPlayers
            .FirstOrDefaultAsync(p => p.UserTeamId == teamId && p.FootballPlayerId == footballPlayerId);
    }

    public void RemovePlayerFromTeam(UserTeamPlayer playerInTeam)
    {
        _context.UserTeamPlayers.Remove(playerInTeam);
    }

    public async Task LockAllTeamsAsync()
    {
        // Esto se traduce a un solo UPDATE en SQL, muy eficiente
        await _context.UserTeams
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.Locked, true));
    }

    public async Task ClearAndSetLineupAsync(Guid userTeamId, Guid matchId, List<Guid> playerIds)
    {
        // Usamos una transacción para asegurar que no borremos y nos quedemos sin nada si algo falla
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Eliminar alineación existente para ese partido
            var existingLineup = _context.UserMatchLineups
                .Where(l => l.UserTeamId == userTeamId && l.MatchId == matchId);

            _context.UserMatchLineups.RemoveRange(existingLineup);

            // 2. Insertar los nuevos 11 titulares
            // Buscamos los IDs de la tabla user_team_players que corresponden a los FootballPlayers elegidos
            var teamPlayers = await _context.UserTeamPlayers
                .Where(tp => tp.UserTeamId == userTeamId && playerIds.Contains(tp.FootballPlayerId))
                .ToListAsync();

            var newLineups = teamPlayers.Select(tp => new UserMatchLineup
            {
                Id = Guid.NewGuid(),
                UserTeamId = userTeamId,
                MatchId = matchId,
                UserTeamPlayerId = tp.Id,
                IsStarter = true,
                IsSubstituted = false,
                CreatedAt = DateTime.UtcNow
            });

            await _context.UserMatchLineups.AddRangeAsync(newLineups);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<UserMatchLineup>> GetMatchLineupAsync(Guid matchId, Guid userId)
    {
        return await _context.UserMatchLineups
            .Include(l => l.UserTeam) // Para los colores del equipo
            .Include(l => l.UserTeamPlayer)
                .ThenInclude(utp => utp.Player) // Cargamos el jugador (donde está el ShirtNumber)
                    .ThenInclude(p => p.Country) // <--- AGREGAR ESTO para traer los datos del país
            .Where(l => l.MatchId == matchId && l.UserTeam.UserId == userId)
            .ToListAsync();
    }

    public async Task<int> GetSubstitutionCountAsync(Guid userId, Guid matchId)
    {
        // Buscamos directamente por el ID del equipo asociado al usuario
        var team = await _context.UserTeams.FirstOrDefaultAsync(t => t.UserId == userId);
        if (team == null) return 0;

        return await _context.UserMatchSubstitutions
            .CountAsync(s => s.MatchId == matchId && s.UserTeamId == team.Id);
    }

    public async Task ExecuteSubstitutionAsync(Guid userId, Guid matchId, Guid footballPlayerOutId, Guid footballPlayerInId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
            {
                var now = DateTime.UtcNow;
                var team = await GetByUserIdAsync(userId);
                if (team == null) throw new InvalidOperationException("Equipo no encontrado.");

            // 1. Buscamos al jugador que SALE usando su FootballPlayerId a través de la relación
            var lineupOut = await _context.UserMatchLineups
                .Include(l => l.UserTeamPlayer)
                .FirstOrDefaultAsync(l => l.UserTeamId == team.Id
                                     && l.MatchId == matchId
                                     && l.UserTeamPlayer.FootballPlayerId == footballPlayerOutId
                                     && l.IsStarter);

            if (lineupOut == null)
                throw new InvalidOperationException("El jugador a salir no está en la alineación titular.");

            // 2. Buscamos al jugador que ENTRA en la banca del usuario
            var playerIn = await _context.UserTeamPlayers
                .FirstOrDefaultAsync(tp => tp.UserTeamId == team.Id
                                     && tp.FootballPlayerId == footballPlayerInId);

            if (playerIn == null)
                throw new InvalidOperationException("El jugador suplente no pertenece a tu equipo.");

            // 3. Validar que el que entra no esté ya jugando
            var isAlreadyPlaying = await _context.UserMatchLineups
                .AnyAsync(l => l.MatchId == matchId && l.UserTeamPlayerId == playerIn.Id);

            if (isAlreadyPlaying)
                throw new InvalidOperationException("El jugador suplente ya está en el campo.");

            // --- LÓGICA DE SUSTITUCIÓN ---

            // A. Crear la nueva entrada en el Lineup para el que entra
            var lineupIn = new UserMatchLineup
            {
                Id = Guid.NewGuid(),
                UserTeamId = team.Id,
                MatchId = matchId,
                UserTeamPlayerId = playerIn.Id,
                IsStarter = false, // Entra como suplente
                IsSubstituted = false,
                CreatedAt = now
            };

            // B. Actualizar el que sale
            lineupOut.IsSubstituted = true;
            lineupOut.SubstitutedByLineupId = lineupIn.Id;

            _context.UserMatchLineups.Add(lineupIn);

            // C. Registrar en la tabla de sustituciones para el motor de puntos
            var subRecord = new UserMatchSubstitution
            {
                Id = Guid.NewGuid(),
                UserTeamId = team.Id,
                MatchId = matchId,
                LineupPlayerOutId = lineupOut.Id,
                LineupPlayerInId = lineupIn.Id,
                SubstitutionTime = now,
                SubstitutionMinute = 60 // Aquí deberías recibir el minuto real del partido o calcularlo
            };

            _context.UserMatchSubstitutions.Add(subRecord);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<UserMatchSubstitution>> GetMatchSubstitutionsAsync(Guid userTeamId, Guid matchId)
    {
        return await _context.UserMatchSubstitutions
            .Where(s => s.UserTeamId == userTeamId && s.MatchId == matchId)
            .ToListAsync();
    }

    public async Task<UserTeam?> GetFullTeamByUserIdAsync(Guid userId)
    {
        return await _context.UserTeams
            .Include(t => t.TeamPlayers)
                .ThenInclude(tp => tp.Player)
                    .ThenInclude(p => p.Country)
            .FirstOrDefaultAsync(t => t.UserId == userId);
    }

    public async Task SwapLineupPlayersBeforeMatchAsync(Guid userId, Guid matchId, Guid footballPlayerIdOut, Guid footballPlayerIdIn)
    {
        // 1. Buscamos el equipo del usuario directamente usando el método de esta misma clase
        var team = await GetByUserIdAsync(userId);
        if (team == null) throw new InvalidOperationException("Equipo no encontrado.");

        // 2. Buscamos la fila de la alineación titular que contiene al futbolista que sale
        var lineupEntry = await _context.UserMatchLineups
            .Include(l => l.UserTeamPlayer)
            .FirstOrDefaultAsync(l => l.UserTeamId == team.Id
                                     && l.MatchId == matchId
                                     && l.UserTeamPlayer.FootballPlayerId == footballPlayerIdOut);

        if (lineupEntry == null)
            throw new InvalidOperationException("La posición que intentas cambiar no existe en tu alineación.");

        // 3. Buscamos al jugador que ENTRA (usando el ID del equipo que obtuvimos en el paso 1)
        var playerInMyTeam = await _context.UserTeamPlayers
            .FirstOrDefaultAsync(tp => tp.FootballPlayerId == footballPlayerIdIn && tp.UserTeamId == team.Id);

        if (playerInMyTeam == null)
            throw new InvalidOperationException("El futbolista seleccionado no ha sido comprado o no pertenece a tu equipo.");

        // 4. Validar que el futbolista que intenta entrar no esté ya jugando en este partido
        var isAlreadyStarter = await _context.UserMatchLineups
            .AnyAsync(l => l.MatchId == matchId && l.UserTeamPlayerId == playerInMyTeam.Id);

        if (isAlreadyStarter)
            throw new InvalidOperationException("Este jugador ya está en la alineación titular.");

        // 5. Hacer el intercambio
        lineupEntry.UserTeamPlayerId = playerInMyTeam.Id;

        await _context.SaveChangesAsync();
    }

    public async Task<List<UserTeamPlayer>> GetAvailableBenchAsync(Guid userTeamId, Guid matchId)
    {
        // Obtenemos los IDs de los jugadores que YA están ocupando un lugar en el partido
        var playersInLineupIds = await _context.UserMatchLineups
            .Where(l => l.UserTeamId == userTeamId && l.MatchId == matchId)
            .Select(l => l.UserTeamPlayerId)
            .ToListAsync();

        // Retornamos los de la nómina (15) que NO estén en esa lista (los 4 restantes)
        return await _context.UserTeamPlayers
            .Include(tp => tp.Player)
                .ThenInclude(p => p.Country)
            .Where(tp => tp.UserTeamId == userTeamId && !playersInLineupIds.Contains(tp.Id))
            .ToListAsync();
    }
}