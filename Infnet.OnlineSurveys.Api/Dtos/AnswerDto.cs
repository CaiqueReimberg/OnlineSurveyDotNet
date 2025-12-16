namespace Infnet.OnlineSurveys.Api.DTOs;

public class AnswerDto
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public Guid OptionId { get; set; }
    public string? QuestionText { get; set; }
    public string? OptionText { get; set; }
}

public class CreateAnswerDto
{
    public Guid QuestionId { get; set; }
    public Guid OptionId { get; set; }
}
