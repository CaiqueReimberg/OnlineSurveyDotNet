using Infnet.OnlineSurveys.Domain.Entities;

namespace Infnet.OnlineSurveys.Domain.Repositories;

public interface IOptionRepository : IRepository<Option>
{
    Task<IEnumerable<Option>> GetByQuestionIdAsync(Guid questionId);
}
