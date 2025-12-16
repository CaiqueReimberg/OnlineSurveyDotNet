namespace Infnet.OnlineSurveys.Domain.Entities;

public class Response
{
    public Guid Id { get; private set; }
    public Guid SurveyId { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public DateTime SubmittedAt { get; private set; }

    private readonly List<Answer> _answers = new();
    public IReadOnlyCollection<Answer> Answers => _answers;

    protected Response() { }

    public Response(Guid surveyId, string name, string email)
    {
        Id = Guid.NewGuid();
        SurveyId = surveyId;
        Name = name;
        Email = email;
        SubmittedAt = DateTime.UtcNow;
    }

    public void AddAnswer(Answer answer)
    {
        _answers.Add(answer);
    }
}
