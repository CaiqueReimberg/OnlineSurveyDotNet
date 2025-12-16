using Infnet.OnlineSurveys.Domain.Entities;
using Infnet.OnlineSurveys.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infnet.OnlineSurveys.Infrastructure.Data.Repositories;

public class ResponseRepository : IResponseRepository
{
    private readonly OnlineSurveysDbContext _context;

    public ResponseRepository(OnlineSurveysDbContext context)
    {
        _context = context;
    }

    public async Task<Response?> GetByIdAsync(Guid id)
    {
        return await _context.Responses.FindAsync(id);
    }

    public async Task<IEnumerable<Response>> GetAllAsync()
    {
        return await _context.Responses
            .Include(r => r.Answers)
            .ToListAsync();
    }

    public async Task AddAsync(Response entity)
    {
        await _context.Responses.AddAsync(entity);
        
        // Define a shadow property ResponseId para cada Answer
        foreach (var answer in entity.Answers)
        {
            _context.Entry(answer).Property("ResponseId").CurrentValue = entity.Id;
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Response entity)
    {
        _context.Responses.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var response = await GetByIdAsync(id);
        if (response != null)
        {
            _context.Responses.Remove(response);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Response>> GetBySurveyIdAsync(Guid surveyId)
    {
        return await _context.Responses
            .Include(r => r.Answers)
            .Where(r => r.SurveyId == surveyId)
            .ToListAsync();
    }

    public async Task<Response?> GetByIdWithAnswersAsync(Guid id)
    {
        return await _context.Responses
            .Include(r => r.Answers)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Response>> GetByEmailAsync(string email)
    {
        return await _context.Responses
            .Include(r => r.Answers)
            .Where(r => r.Email == email)
            .ToListAsync();
    }
}
