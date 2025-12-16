namespace Infnet.OnlineSurveys.Domain.Entities;

public class Answer
{
    public Guid Id { get; private set; }
    public Guid QuestionId { get; private set; }
    public Guid OptionId { get; private set; }

    protected Answer() { }

    public Answer(Guid questionId, Guid optionId)
    {
        Id = Guid.NewGuid();
        QuestionId = questionId;
        OptionId = optionId;
    }
}
