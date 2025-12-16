using Infnet.OnlineSurveys.Domain.Entities;
using Infnet.OnlineSurveys.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infnet.OnlineSurveys.Infrastructure.Data.Repositories;

public class SurveyRepository : ISurveyRepository
{
    private readonly OnlineSurveysDbContext _context;

    public SurveyRepository(OnlineSurveysDbContext context)
    {
        _context = context;
    }

    public async Task<Survey?> GetByIdAsync(Guid id)
    {
        return await _context.Surveys.FindAsync(id);
    }

    public async Task<IEnumerable<Survey>> GetAllAsync()
    {
        return await _context.Surveys.ToListAsync();
    }

    public async Task AddAsync(Survey entity)
    {
        await _context.Surveys.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Survey entity)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var survey = await GetByIdAsync(id);
        if (survey != null)
        {
            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Survey>> GetByStatusAsync(Domain.Enums.SurveyStatus status)
    {
        return await _context.Surveys
            .Where(s => s.Status == status)
            .ToListAsync();
    }

    public async Task<Survey?> GetByIdWithQuestionsAsync(Guid id)
    {
        return await _context.Surveys
            .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task AddQuestionToSurveyAsync(Guid surveyId, Question question)
    {
        var survey = await _context.Surveys
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == surveyId);

        if (survey == null)
        {
            throw new InvalidOperationException($"Survey with ID {surveyId} not found.");
        }

        if (survey.Status != Domain.Enums.SurveyStatus.Draft)
        {
            throw new InvalidOperationException("Cannot add questions after publication.");
        }

        // Adiciona a questão ao contexto
        var questionEntry = await _context.Questions.AddAsync(question);
        
        // Define a shadow property SurveyId
        _context.Entry(question).Property("SurveyId").CurrentValue = surveyId;

        // As opções já estão na coleção Options da Question
        // O EF vai detectá-las automaticamente e adicionar
        
        await _context.SaveChangesAsync();
    }
}
