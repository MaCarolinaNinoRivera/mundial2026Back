using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FantasyWorldCup.Domain.Trivias.Entities;

namespace FantasyWorldCup.Infrastructure.Persistence.Configurations;

public class UserTriviaAnswerConfiguration : IEntityTypeConfiguration<UserTriviaAnswer>
{
    public void Configure(EntityTypeBuilder<UserTriviaAnswer> builder)
    {
        builder.ToTable("user_trivia_answers", "public");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
        builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(e => e.TriviaQuestionId).HasColumnName("trivia_question_id").IsRequired();
        builder.Property(e => e.SelectedAnswer).HasColumnName("selected_answer").IsRequired(false);
        builder.Property(e => e.AnsweredAt).HasColumnName("answered_at").HasDefaultValueSql("now()");
        builder.Property(e => e.PointsEarned).HasColumnName("points_earned").HasDefaultValue(0);
    }
}
