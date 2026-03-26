using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FantasyWorldCup.Domain.Trivias.Entities;

namespace FantasyWorldCup.Infrastructure.Persistence.Configurations;

public class TriviaQuestionConfiguration : IEntityTypeConfiguration<TriviaQuestion>
{
    public void Configure(EntityTypeBuilder<TriviaQuestion> builder)
    {
        builder.ToTable("trivia_questions", "public");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
        builder.Property(e => e.QuestionText).HasColumnName("question").IsRequired();
        builder.Property(e => e.OptionA).HasColumnName("option_a").IsRequired();
        builder.Property(e => e.OptionB).HasColumnName("option_b").IsRequired();
        builder.Property(e => e.OptionC).HasColumnName("option_c").IsRequired();
        builder.Property(e => e.CorrectOption).HasColumnName("correct_answer").IsRequired();
        builder.Property(e => e.PointsValue).HasColumnName("points").HasDefaultValue(5);
    }
}
