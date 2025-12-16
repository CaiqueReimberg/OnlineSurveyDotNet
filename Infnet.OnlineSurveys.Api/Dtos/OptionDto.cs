namespace Infnet.OnlineSurveys.Api.DTOs;

public class OptionDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class CreateOptionDto
{
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class UpdateOptionDto
{
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
}
