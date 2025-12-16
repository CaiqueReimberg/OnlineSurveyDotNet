namespace Infnet.OnlineSurveys.Domain.Entities;

public class Question
{
    public Guid Id { get; private set; }
    public string Text { get; private set; }
    public int Order { get; private set; }

    private readonly List<Option> _options = new();
    public IReadOnlyCollection<Option> Options => _options;

    protected Question() { }

    public Question(string text, int order)
    {
        Id = Guid.NewGuid();
        Text = text;
        Order = order;
    }

    public void AddOption(Option option)
    {
        _options.Add(option);
    }
}
