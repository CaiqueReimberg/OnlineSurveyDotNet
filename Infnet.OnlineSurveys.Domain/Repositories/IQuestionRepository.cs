using Infnet.OnlineSurveys.Domain.Entities;

namespace Infnet.OnlineSurveys.Domain.Repositories;

public interface IQuestionRepository : IRepository<Question>
{
    Task<IEnumerable<Question>> GetBySurveyIdAsync(Guid surveyId);
    Task<Question?> GetByIdWithOptionsAsync(Guid id);
}
