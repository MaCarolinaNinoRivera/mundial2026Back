// En src/FantasyWorldCup.Application/Teams/UseCases/MakeSubstitution/MakeSubstitutionCommand.cs
public record MakeSubstitutionCommand(Guid MatchId, Guid PlayerOutId, Guid PlayerInId);