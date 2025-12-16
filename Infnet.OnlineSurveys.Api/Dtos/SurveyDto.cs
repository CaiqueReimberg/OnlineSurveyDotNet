namespace Infnet.OnlineSurveys.Api.DTOs;

public class SurveyDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<QuestionDto> Questions { get; set; } = new();
}

public class CreateSurveyDto
{
    public string Title { get; set; } = string.Empty;
}

public class UpdateSurveyDto
{
    public string Title { get; set; } = string.Empty;
}
