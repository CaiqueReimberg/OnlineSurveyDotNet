namespace Infnet.OnlineSurveys.Api.DTOs;

public class QuestionDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<OptionDto> Options { get; set; } = new();
}

public class CreateQuestionDto
{
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<CreateOptionDto> Options { get; set; } = new();
}

public class UpdateQuestionDto
{
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
}
