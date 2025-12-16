using Infnet.OnlineSurveys.Domain.Entities;

namespace Infnet.OnlineSurveys.Domain.Repositories;

public interface IResponseRepository : IRepository<Response>
{
    Task<IEnumerable<Response>> GetBySurveyIdAsync(Guid surveyId);
    Task<Response?> GetByIdWithAnswersAsync(Guid id);
    Task<IEnumerable<Response>> GetByEmailAsync(string email);
}
