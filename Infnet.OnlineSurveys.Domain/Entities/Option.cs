namespace Infnet.OnlineSurveys.Domain.Entities;

public class Option
{
    public Guid Id { get; private set; }
    public string Text { get; private set; }
    public int Order { get; private set; }

    protected Option() { }

    public Option(string text, int order)
    {
        Id = Guid.NewGuid();
        Text = text;
        Order = order;
    }
}
