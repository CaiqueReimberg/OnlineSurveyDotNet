using Infnet.OnlineSurveys.Domain.Enums;

namespace Infnet.OnlineSurveys.Domain.Entities
{
    public class Survey
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public SurveyStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly List<Question> _questions = new();
        public IReadOnlyCollection<Question> Questions => _questions;

        protected Survey() { }

        public Survey(string title)
        {
            Id = Guid.NewGuid();
            Title = title;
            Status = SurveyStatus.Draft;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddQuestion(Question question)
        {
            if (Status != SurveyStatus.Draft)
                throw new InvalidOperationException("Cannot add questions after publication.");

            _questions.Add(question);
        }

        public void Publish()
        {

            if (!_questions.Any())
                throw new InvalidOperationException("Survey must have at least one question.");

            if (_questions.Any(q => q.Options.Count < 2))
                throw new InvalidOperationException("Each question must have at least two options.");

            Status = SurveyStatus.Published;
        }

        public void Close()
        {
            Status = SurveyStatus.Closed;
        }
    }
}
