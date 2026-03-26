using FantasyWorldCup.Application.Auth.UseCases.Register;
using FantasyWorldCup.Application.Auth.UseCases.Login;
using FantasyWorldCup.Application.Auth.UseCases.Logout;
using FantasyWorldCup.Application.Auth.Interfaces;
using FantasyWorldCup.Application.Countries.Interfaces;
using FantasyWorldCup.Infrastructure.Persistence;
using FantasyWorldCup.Infrastructure.Persistence.Repositories;
using FantasyWorldCup.Infrastructure.Security.Hashing;
using FantasyWorldCup.Infrastructure.Security.Jwt;
using FantasyWorldCup.Application.Matches.Interfaces;
using FantasyWorldCup.Application.Matches.UseCases.CreateMatch;
using FantasyWorldCup.Application.Matches.UseCases.UpdateMatch;
using FantasyWorldCup.Api.Middleware;
using FantasyWorldCup.Application.Auth.UseCases.RefreshToken;
using FantasyWorldCup.Application.Predictions.Interfaces;
using FantasyWorldCup.Application.Predictions.UseCases.UpsertPrediction;
using FantasyWorldCup.Application.Predictions.UseCases.GetMyPredictions;
using FantasyWorldCup.Application.Predictions.UseCases.CalculatePoints;
using FantasyWorldCup.Application.Trivias.Interfaces;
using FantasyWorldCup.Application.Trivias.UseCases.CreateQuestion;
using FantasyWorldCup.Application.Trivias.UseCases.UpdateQuestion;
using FantasyWorldCup.Application.Trivias.UseCases.AnswerQuestion;
using FantasyWorldCup.Application.Trivias.UseCases.GetNextQuestion;
using FantasyWorldCup.Application.Teams.Interfaces;
using FantasyWorldCup.Application.Teams.Services;
using FantasyWorldCup.Application.Teams.UseCases.BuyPlayer;
using FantasyWorldCup.Application.Teams.UseCases.SellPlayer;
using FantasyWorldCup.Application.Teams.UseCases.LockAllTeams;
using FantasyWorldCup.Application.Teams.UseCases.MakeSubstitution;
using FantasyWorldCup.Application.Teams.UseCases.SetLineup;
using FantasyWorldCup.Application.Teams.UseCases.UpdateTeamKit;
using FantasyWorldCup.Application.Matches.UseCases.DistributePoints;
using FantasyWorldCup.Application.Matches.UseCases.ProcessMatchPoints;
using FantasyWorldCup.Application.Matches.UseCases.RecordMatchEvent;
using FantasyWorldCup.Application.Scoring.Interfaces;
using FantasyWorldCup.Application.Scoring.Services;
using FantasyWorldCup.Application.Matches.UseCases.RecordMatchStats;
using FantasyWorldCup.Application.Matches.Validators;
using FantasyWorldCup.Application.Scoring.Queries.GetLeaderboard;
using FantasyWorldCup.Application.Scoring.Queries.GetScoringRules;
using FantasyWorldCup.Application.Scoring.UseCases.RecordRankSnapshot;
using FantasyWorldCup.Api.BackgroundServices;
using FantasyWorldCup.Application.Badges.Interfaces;
using FantasyWorldCup.Application.Badges.Services;
using FantasyWorldCup.Application.Teams.Queries.GetAvailableBench;
using FantasyWorldCup.Application.Teams.Queries.GetMatchLineup;
using FantasyWorldCup.Application.Teams.Queries.GetMyTeam;
using FantasyWorldCup.Application.Matches.Queries.GetMatches;
using FantasyWorldCup.Application.Matches.Queries.GetMatchPlayers;
using FantasyWorldCup.Application.Badges.Queries.GetBadges;
using FantasyWorldCup.Application.Countries.Queries.GetCountriesAndGroups;
using FantasyWorldCup.Application.Countries.Queries.GetFilters;
using FantasyWorldCup.Application.Matches.Queries.GetTodayMatches;

using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "FantasyWorldCup API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingresa el token JWT así: Bearer {tu token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Use Cases
builder.Services.AddScoped<RegisterUserHandler>();
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<CreateMatchHandler>();
builder.Services.AddScoped<UpdateMatchHandler>();
builder.Services.AddScoped<RefreshTokenHandler>();
builder.Services.AddScoped<LogoutHandler>();
builder.Services.AddScoped<UpsertPredictionHandler>();
builder.Services.AddScoped<GetMyPredictionsHandler>();
builder.Services.AddScoped<CalculatePointsHandler>();
builder.Services.AddScoped<CreateQuestionHandler>();
builder.Services.AddScoped<UpdateQuestionHandler>();
builder.Services.AddScoped<GetNextQuestionHandler>();
builder.Services.AddScoped<AnswerQuestionHandler>();
builder.Services.AddScoped<TeamValidator>();
builder.Services.AddScoped<BuyPlayerHandler>();
builder.Services.AddScoped<ProcessMatchPointsHandler>();
builder.Services.AddScoped<DistributePointsHandler>();
builder.Services.AddScoped<RecordMatchStatsHandler>();
builder.Services.AddScoped<GetLeaderboardHandler>();
builder.Services.AddScoped<GetScoringRulesHandler>();
builder.Services.AddScoped<SellPlayerHandler>();
builder.Services.AddScoped<LockAllTeamsHandler>();
builder.Services.AddScoped<RecordRankSnapshotHandler>();
builder.Services.AddHostedService<RankSnapshotWorker>();
builder.Services.AddScoped<GetBadgesLeaderboardHandler>();
builder.Services.AddScoped<SetLineupHandler>();
builder.Services.AddScoped<MakeSubstitutionHandler>();
builder.Services.AddScoped<GetMyTeamHandler>();
builder.Services.AddScoped<GetMatchLineupHandler>();
builder.Services.AddScoped<GetAvailableBenchHandler>();
builder.Services.AddScoped<RecordMatchEventHandler>();
builder.Services.AddScoped<GetMatchPlayersHandler>();
builder.Services.AddScoped<GetBadgesHandler>();
builder.Services.AddScoped<GetCountriesAndGroupsHandler>();
builder.Services.AddScoped<GetFilteredPlayersHandler>();
builder.Services.AddScoped<GetTodayMatchesHandler>();
builder.Services.AddScoped<GetRankMovementHandler>();
builder.Services.AddScoped<GetMatchResultsHandler>();
builder.Services.AddScoped<UpdateTeamKitHandler>();

// Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IPredictionRepository, PredictionRepository>();
builder.Services.AddScoped<ITriviaRepository, TriviaRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IScoringRepository, ScoringRepository>();
builder.Services.AddScoped<IPointsRepository, PointsRepository>();
builder.Services.AddScoped<IBadgeRepository, BadgeRepository>();
builder.Services.AddScoped<ScoringService>();
builder.Services.AddScoped<BadgeEngineService>();

// Security
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();

// Validators
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateMatchValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpsertPredictionValidator>();

// n8n
builder.Services.AddHttpClient("n8n", client =>
{
    client.BaseAddress = new Uri("TU_URL_DE_N8N");
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Esto asegura que los JSON de salida siempre lleven el formato correcto
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Agrega esto para PostgreSQL específicamente
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// JWT CONFIGURATION CORRECTA
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtOptions = jwtSection.Get<JwtOptions>();

builder.Services.Configure<JwtOptions>(jwtSection);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions!.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.Secret))
        };
    });

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowReact");

// Puedes comentar esto si molesta
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
