using Infnet.OnlineSurveys.Domain.Entities;

namespace Infnet.OnlineSurveys.Domain.Repositories;

public interface IAnswerRepository : IRepository<Answer>
{
    Task<IEnumerable<Answer>> GetByResponseIdAsync(Guid responseId);
    Task<IEnumerable<Answer>> GetByQuestionIdAsync(Guid questionId);
}
