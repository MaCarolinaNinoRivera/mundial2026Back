using Microsoft.EntityFrameworkCore;
using FantasyWorldCup.Domain.Auth.Entities;
using FantasyWorldCup.Domain.Matches.Entities;
using FantasyWorldCup.Domain.Countries.Entities;
using FantasyWorldCup.Domain.Predictions.Entities;
using FantasyWorldCup.Domain.Trivias.Entities;
using FantasyWorldCup.Domain.Scoring.Entities;
using FantasyWorldCup.Domain.Teams.Entities;
using FantasyWorldCup.Application.Scoring.Queries.GetLeaderboard;
using FantasyWorldCup.Domain.Badges.Entities;

namespace FantasyWorldCup.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Match> Matches { get; set; } = default!;
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<MatchPrediction> MatchPredictions => Set<MatchPrediction>();
    public DbSet<TriviaQuestion> TriviaQuestions => Set<TriviaQuestion>();
    public DbSet<UserTriviaAnswer> UserTriviaAnswers => Set<UserTriviaAnswer>();
    public DbSet<UserTeam> UserTeams => Set<UserTeam>();
    public DbSet<UserTeamPlayer> UserTeamPlayers => Set<UserTeamPlayer>();
    public DbSet<FootballPlayer> FootballPlayers => Set<FootballPlayer>();
    public DbSet<UserPointsLedger> UserPointsLedger => Set<UserPointsLedger>();
    public DbSet<PlayerMatchStat> PlayerMatchStats => Set<PlayerMatchStat>();
    public DbSet<UserRankHistory> UserRankHistories => Set<UserRankHistory>();
    public DbSet<Badge> Badges => Set<Badge>();
    public DbSet<UserBadge> UserBadges => Set<UserBadge>();
    public DbSet<UserMatchLineup> UserMatchLineups => Set<UserMatchLineup>();
    public DbSet<UserMatchSubstitution> UserMatchSubstitutions => Set<UserMatchSubstitution>();
    public DbSet<MatchEvent> MatchEvents => Set<MatchEvent>();
    public DbSet<Group> Groups => Set<Group>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
