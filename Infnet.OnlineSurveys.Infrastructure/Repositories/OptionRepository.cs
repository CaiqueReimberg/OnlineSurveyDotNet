using Infnet.OnlineSurveys.Domain.Entities;
using Infnet.OnlineSurveys.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infnet.OnlineSurveys.Infrastructure.Data.Repositories;

public class OptionRepository : IOptionRepository
{
    private readonly OnlineSurveysDbContext _context;

    public OptionRepository(OnlineSurveysDbContext context)
    {
        _context = context;
    }

    public async Task<Option?> GetByIdAsync(Guid id)
    {
        return await _context.Options.FindAsync(id);
    }

    public async Task<IEnumerable<Option>> GetAllAsync()
    {
        return await _context.Options.ToListAsync();
    }

    public async Task AddAsync(Option entity)
    {
        await _context.Options.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Option entity)
    {
        _context.Options.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var option = await GetByIdAsync(id);
        if (option != null)
        {
            _context.Options.Remove(option);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Option>> GetByQuestionIdAsync(Guid questionId)
    {
        return await _context.Options
            .Where(o => EF.Property<Guid>(o, "QuestionId") == questionId)
            .ToListAsync();
    }
}
