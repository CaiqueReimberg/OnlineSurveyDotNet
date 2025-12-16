using Infnet.OnlineSurveys.Domain.Entities;
using Infnet.OnlineSurveys.Infrastructure.Data;
using Infnet.OnlineSurveys.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infnet.OnlineSurveys.Infrastructure.Data.Tests.Repositories;

public class ResponseRepositoryTests : IDisposable
{
    private readonly OnlineSurveysDbContext _context;
    private readonly ResponseRepository _repository;
    private readonly Guid _surveyId;

    public ResponseRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<OnlineSurveysDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new OnlineSurveysDbContext(options);
        _repository = new ResponseRepository(_context);
        _surveyId = Guid.NewGuid();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task AddAsync_ShouldAddResponseToDatabase()
    {
        // Arrange
        var response = new Response(_surveyId, "João Silva", "joao@example.com");
        var answer = new Answer(Guid.NewGuid(), Guid.NewGuid());
        response.AddAnswer(answer);

        // Act
        await _repository.AddAsync(response);

        // Assert
        var savedResponse = await _context.Responses
            .Include(r => r.Answers)
            .FirstOrDefaultAsync(r => r.Id == response.Id);
        
        Assert.NotNull(savedResponse);
        Assert.Equal("João Silva", savedResponse.Name);
        Assert.Equal("joao@example.com", savedResponse.Email);
        Assert.Single(savedResponse.Answers);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnResponse()
    {
        // Arrange
        var response = new Response(_surveyId, "João Silva", "joao@example.com");
        await _repository.AddAsync(response);

        // Act
        var result = await _repository.GetByIdAsync(response.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(response.Id, result.Id);
        Assert.Equal("João Silva", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllResponsesWithAnswers()
    {
        // Arrange
        var response1 = new Response(_surveyId, "João Silva", "joao@example.com");
        var response2 = new Response(_surveyId, "Maria Santos", "maria@example.com");
        var response3 = new Response(_surveyId, "Pedro Oliveira", "pedro@example.com");

        response1.AddAnswer(new Answer(Guid.NewGuid(), Guid.NewGuid()));
        response2.AddAnswer(new Answer(Guid.NewGuid(), Guid.NewGuid()));
        response3.AddAnswer(new Answer(Guid.NewGuid(), Guid.NewGuid()));

        await _repository.AddAsync(response1);
        await _repository.AddAsync(response2);
        await _repository.AddAsync(response3);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count());
        Assert.All(result, r => Assert.NotEmpty(r.Answers));
    }

    [Fact]
    public async Task GetBySurveyIdAsync_ShouldReturnResponsesForSpecificSurvey()
    {
        // Arrange
        var survey1Id = Guid.NewGuid();
        var survey2Id = Guid.NewGuid();

        var response1 = new Response(survey1Id, "João Silva", "joao@example.com");
        var response2 = new Response(survey1Id, "Maria Santos", "maria@example.com");
        var response3 = new Response(survey2Id, "Pedro Oliveira", "pedro@example.com");

        response1.AddAnswer(new Answer(Guid.NewGuid(), Guid.NewGuid()));
        response2.AddAnswer(new Answer(Guid.NewGuid(), Guid.NewGuid()));
        response3.AddAnswer(new Answer(Guid.NewGuid(), Guid.NewGuid()));

        await _repository.AddAsync(response1);
        await _repository.AddAsync(response2);
        await _repository.AddAsync(response3);

        // Act
        var results = await _repository.GetBySurveyIdAsync(survey1Id);

        // Assert
        Assert.Equal(2, results.Count());
        Assert.All(results, r => Assert.Equal(survey1Id, r.SurveyId));
        Assert.All(results, r => Assert.NotEmpty(r.Answers));
    }

    [Fact]
    public async Task GetByIdWithAnswersAsync_ShouldReturnResponseWithAnswers()
    {
        // Arrange
        var response = new Response(_surveyId, "João Silva", "joao@example.com");
        var answer1 = new Answer(Guid.NewGuid(), Guid.NewGuid());
        var answer2 = new Answer(Guid.NewGuid(), Guid.NewGuid());
        response.AddAnswer(answer1);
        response.AddAnswer(answer2);

        await _repository.AddAsync(response);

        // Act
        var result = await _repository.GetByIdWithAnswersAsync(response.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Answers.Count);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnResponsesForSpecificEmail()
    {
        // Arrange
        var email = "joao@example.com";
        var response1 = new Response(_surveyId, "João Silva", email);
        var response2 = new Response(_surveyId, "João Silva", email);
        var response3 = new Response(_surveyId, "Maria Santos", "maria@example.com");

        response1.AddAnswer(new Answer(Guid.NewGuid(), Guid.NewGuid()));
        response2.AddAnswer(new Answer(Guid.NewGuid(), Guid.NewGuid()));
        response3.AddAnswer(new Answer(Guid.NewGuid(), Guid.NewGuid()));

        await _repository.AddAsync(response1);
        await _repository.AddAsync(response2);
        await _repository.AddAsync(response3);

        // Act
        var results = await _repository.GetByEmailAsync(email);

        // Assert
        Assert.Equal(2, results.Count());
        Assert.All(results, r => Assert.Equal(email, r.Email));
        Assert.All(results, r => Assert.NotEmpty(r.Answers));
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveResponseFromDatabase()
    {
        // Arrange
        var response = new Response(_surveyId, "João Silva", "joao@example.com");
        await _repository.AddAsync(response);

        // Act
        await _repository.DeleteAsync(response.Id);

        // Assert
        var deletedResponse = await _context.Responses.FindAsync(response.Id);
        Assert.Null(deletedResponse);
    }

    [Fact]
    public async Task AddAsync_WithMultipleAnswers_ShouldSaveAllAnswers()
    {
        // Arrange
        var response = new Response(_surveyId, "João Silva", "joao@example.com");
        var questionId1 = Guid.NewGuid();
        var questionId2 = Guid.NewGuid();
        var questionId3 = Guid.NewGuid();

        response.AddAnswer(new Answer(questionId1, Guid.NewGuid()));
        response.AddAnswer(new Answer(questionId2, Guid.NewGuid()));
        response.AddAnswer(new Answer(questionId3, Guid.NewGuid()));

        // Act
        await _repository.AddAsync(response);

        // Assert
        var savedResponse = await _repository.GetByIdWithAnswersAsync(response.Id);
        Assert.NotNull(savedResponse);
        Assert.Equal(3, savedResponse.Answers.Count);
    }

    [Fact]
    public async Task GetBySurveyIdAsync_WithNoResponses_ShouldReturnEmptyList()
    {
        // Act
        var results = await _repository.GetBySurveyIdAsync(Guid.NewGuid());

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public async Task GetByEmailAsync_WithNoResponses_ShouldReturnEmptyList()
    {
        // Act
        var results = await _repository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.Empty(results);
    }
}
