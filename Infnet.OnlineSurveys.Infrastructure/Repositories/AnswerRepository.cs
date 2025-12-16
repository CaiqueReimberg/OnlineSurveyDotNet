using Infnet.OnlineSurveys.Domain.Entities;
using Infnet.OnlineSurveys.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infnet.OnlineSurveys.Infrastructure.Data.Repositories;

public class AnswerRepository : IAnswerRepository
{
    private readonly OnlineSurveysDbContext _context;

    public AnswerRepository(OnlineSurveysDbContext context)
    {
        _context = context;
    }

    public async Task<Answer?> GetByIdAsync(Guid id)
    {
        return await _context.Answers.FindAsync(id);
    }

    public async Task<IEnumerable<Answer>> GetAllAsync()
    {
        return await _context.Answers.ToListAsync();
    }

    public async Task AddAsync(Answer entity)
    {
        await _context.Answers.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Answer entity)
    {
        _context.Answers.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var answer = await GetByIdAsync(id);
        if (answer != null)
        {
            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Answer>> GetByResponseIdAsync(Guid responseId)
    {
        return await _context.Answers
            .Where(a => EF.Property<Guid>(a, "ResponseId") == responseId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Answer>> GetByQuestionIdAsync(Guid questionId)
    {
        return await _context.Answers
            .Where(a => a.QuestionId == questionId)
            .ToListAsync();
    }
}
