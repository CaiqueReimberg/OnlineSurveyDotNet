using Infnet.OnlineSurveys.Domain.Entities;

namespace Infnet.OnlineSurveys.Domain.Repositories;

public interface ISurveyRepository : IRepository<Survey>
{
    Task<IEnumerable<Survey>> GetByStatusAsync(Enums.SurveyStatus status);
    Task<Survey?> GetByIdWithQuestionsAsync(Guid id);
    Task AddQuestionToSurveyAsync(Guid surveyId, Question question);
}
