namespace Infnet.OnlineSurveys.Api.DTOs;

public class ResponseDto
{
    public Guid Id { get; set; }
    public Guid SurveyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public List<AnswerDto> Answers { get; set; } = new();
}

public class CreateResponseDto
{
    public Guid SurveyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<CreateAnswerDto> Answers { get; set; } = new();
}
