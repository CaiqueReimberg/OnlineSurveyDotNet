using Infnet.OnlineSurveys.Domain.Entities;
using Infnet.OnlineSurveys.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infnet.OnlineSurveys.Infrastructure.Data.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly OnlineSurveysDbContext _context;

    public QuestionRepository(OnlineSurveysDbContext context)
    {
        _context = context;
    }

    public async Task<Question?> GetByIdAsync(Guid id)
    {
        return await _context.Questions.FindAsync(id);
    }

    public async Task<IEnumerable<Question>> GetAllAsync()
    {
        return await _context.Questions.ToListAsync();
    }

    public async Task AddAsync(Question entity)
    {
        await _context.Questions.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Question entity)
    {
        _context.Questions.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var question = await GetByIdAsync(id);
        if (question != null)
        {
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Question>> GetBySurveyIdAsync(Guid surveyId)
    {
        return await _context.Questions
            .Where(q => EF.Property<Guid>(q, "SurveyId") == surveyId)
            .ToListAsync();
    }

    public async Task<Question?> GetByIdWithOptionsAsync(Guid id)
    {
        return await _context.Questions
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == id);
    }
}
