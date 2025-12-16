using Infnet.OnlineSurveys.Domain.Entities;
using Infnet.OnlineSurveys.Domain.Enums;
using Infnet.OnlineSurveys.Infrastructure.Data;
using Infnet.OnlineSurveys.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infnet.OnlineSurveys.Infrastructure.Data.Tests.Repositories;

public class SurveyRepositoryTests : IDisposable
{
    private readonly OnlineSurveysDbContext _context;
    private readonly SurveyRepository _repository;

    public SurveyRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<OnlineSurveysDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new OnlineSurveysDbContext(options);
        _repository = new SurveyRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task AddAsync_ShouldAddSurveyToDatabase()
    {
        // Arrange
        var survey = new Survey("Test Survey");

        // Act
        await _repository.AddAsync(survey);

        // Assert
        var savedSurvey = await _context.Surveys.FindAsync(survey.Id);
        Assert.NotNull(savedSurvey);
        Assert.Equal("Test Survey", savedSurvey.Title);
        Assert.Equal(SurveyStatus.Draft, savedSurvey.Status);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnSurvey()
    {
        // Arrange
        var survey = new Survey("Test Survey");
        await _repository.AddAsync(survey);

        // Act
        var result = await _repository.GetByIdAsync(survey.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(survey.Id, result.Id);
        Assert.Equal("Test Survey", result.Title);
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
    public async Task GetAllAsync_ShouldReturnAllSurveys()
    {
        // Arrange
        var survey1 = new Survey("Survey 1");
        var survey2 = new Survey("Survey 2");
        var survey3 = new Survey("Survey 3");

        await _repository.AddAsync(survey1);
        await _repository.AddAsync(survey2);
        await _repository.AddAsync(survey3);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetByStatusAsync_ShouldReturnSurveysWithMatchingStatus()
    {
        // Arrange
        var draftSurvey = new Survey("Draft Survey");
        var publishedSurvey = new Survey("Published Survey");
        
        var question = new Question("Test Question", 1);
        question.AddOption(new Option("Option 1", 1));
        question.AddOption(new Option("Option 2", 2));
        publishedSurvey.AddQuestion(question);
        publishedSurvey.Publish();

        await _repository.AddAsync(draftSurvey);
        await _repository.AddAsync(publishedSurvey);

        // Act
        var draftResults = await _repository.GetByStatusAsync(SurveyStatus.Draft);
        var publishedResults = await _repository.GetByStatusAsync(SurveyStatus.Published);

        // Assert
        Assert.Single(draftResults);
        Assert.Single(publishedResults);
        Assert.Equal("Draft Survey", draftResults.First().Title);
        Assert.Equal("Published Survey", publishedResults.First().Title);
    }

    [Fact]
    public async Task GetByIdWithQuestionsAsync_ShouldReturnSurveyWithQuestions()
    {
        // Arrange
        var survey = new Survey("Test Survey");
        var question = new Question("Test Question", 1);
        question.AddOption(new Option("Option 1", 1));
        question.AddOption(new Option("Option 2", 2));
        survey.AddQuestion(question);

        await _repository.AddAsync(survey);

        // Act
        var result = await _repository.GetByIdWithQuestionsAsync(survey.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Questions);
        Assert.Equal("Test Question", result.Questions.First().Text);
        Assert.Equal(2, result.Questions.First().Options.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldSaveChanges()
    {
        // Arrange
        var survey = new Survey("Original Title");
        await _repository.AddAsync(survey);

        var loadedSurvey = await _repository.GetByIdAsync(survey.Id);
        Assert.NotNull(loadedSurvey);

        // Act
        await _repository.UpdateAsync(loadedSurvey);

        // Assert
        var updatedSurvey = await _repository.GetByIdAsync(survey.Id);
        Assert.NotNull(updatedSurvey);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveSurveyFromDatabase()
    {
        // Arrange
        var survey = new Survey("Test Survey");
        await _repository.AddAsync(survey);

        // Act
        await _repository.DeleteAsync(survey.Id);

        // Assert
        var deletedSurvey = await _context.Surveys.FindAsync(survey.Id);
        Assert.Null(deletedSurvey);
    }

    [Fact]
    public async Task AddQuestionToSurveyAsync_ShouldAddQuestionSuccessfully()
    {
        // Arrange
        var survey = new Survey("Test Survey");
        await _repository.AddAsync(survey);

        var question = new Question("New Question", 1);
        question.AddOption(new Option("Option 1", 1));
        question.AddOption(new Option("Option 2", 2));

        // Act
        await _repository.AddQuestionToSurveyAsync(survey.Id, question);

        // Assert
        var updatedSurvey = await _repository.GetByIdWithQuestionsAsync(survey.Id);
        Assert.NotNull(updatedSurvey);
        Assert.Single(updatedSurvey.Questions);
        Assert.Equal("New Question", updatedSurvey.Questions.First().Text);
        Assert.Equal(2, updatedSurvey.Questions.First().Options.Count);
    }

    [Fact]
    public async Task AddQuestionToSurveyAsync_WithInvalidSurveyId_ShouldThrowException()
    {
        // Arrange
        var question = new Question("Test Question", 1);
        question.AddOption(new Option("Option 1", 1));
        question.AddOption(new Option("Option 2", 2));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _repository.AddQuestionToSurveyAsync(Guid.NewGuid(), question));
    }

    [Fact]
    public async Task AddQuestionToSurveyAsync_ToPublishedSurvey_ShouldThrowException()
    {
        // Arrange
        var survey = new Survey("Test Survey");
        var initialQuestion = new Question("Initial Question", 1);
        initialQuestion.AddOption(new Option("Option 1", 1));
        initialQuestion.AddOption(new Option("Option 2", 2));
        survey.AddQuestion(initialQuestion);
        survey.Publish();

        await _repository.AddAsync(survey);

        var newQuestion = new Question("New Question", 2);
        newQuestion.AddOption(new Option("Option A", 1));
        newQuestion.AddOption(new Option("Option B", 2));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _repository.AddQuestionToSurveyAsync(survey.Id, newQuestion));
    }
}
