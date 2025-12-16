using Infnet.OnlineSurveys.Api.Controllers;
using Infnet.OnlineSurveys.Api.DTOs;
using Infnet.OnlineSurveys.Domain.Entities;
using Infnet.OnlineSurveys.Domain.Enums;
using Infnet.OnlineSurveys.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Infnet.OnlineSurveys.Api.Tests.Controllers;

public class SurveysControllerTests
{
    private readonly Mock<ISurveyRepository> _mockSurveyRepository;
    private readonly Mock<ILogger<SurveysController>> _mockLogger;
    private readonly SurveysController _controller;

    public SurveysControllerTests()
    {
        _mockSurveyRepository = new Mock<ISurveyRepository>();
        _mockLogger = new Mock<ILogger<SurveysController>>();
        _controller = new SurveysController(_mockSurveyRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllSurveys()
    {
        // Arrange
        var surveys = new List<Survey>
        {
            new Survey("Survey 1"),
            new Survey("Survey 2")
        };

        _mockSurveyRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(surveys);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedSurveys = Assert.IsAssignableFrom<IEnumerable<SurveyDto>>(okResult.Value);
        Assert.Equal(2, returnedSurveys.Count());
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnSurvey()
    {
        // Arrange
        var surveyId = Guid.NewGuid();
        var survey = new Survey("Test Survey");

        _mockSurveyRepository.Setup(repo => repo.GetByIdWithQuestionsAsync(surveyId))
            .ReturnsAsync(survey);

        // Act
        var result = await _controller.GetById(surveyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedSurvey = Assert.IsType<SurveyDto>(okResult.Value);
        Assert.Equal("Test Survey", returnedSurvey.Title);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var surveyId = Guid.NewGuid();

        _mockSurveyRepository.Setup(repo => repo.GetByIdWithQuestionsAsync(surveyId))
            .ReturnsAsync((Survey?)null);

        // Act
        var result = await _controller.GetById(surveyId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByStatus_WithValidStatus_ShouldReturnSurveys()
    {
        // Arrange
        var surveys = new List<Survey>
        {
            new Survey("Draft Survey")
        };

        _mockSurveyRepository.Setup(repo => repo.GetByStatusAsync(SurveyStatus.Draft))
            .ReturnsAsync(surveys);

        // Act
        var result = await _controller.GetByStatus("Draft");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedSurveys = Assert.IsAssignableFrom<IEnumerable<SurveyDto>>(okResult.Value);
        Assert.Single(returnedSurveys);
    }

    [Fact]
    public async Task GetByStatus_WithInvalidStatus_ShouldReturnBadRequest()
    {
        // Act
        var result = await _controller.GetByStatus("InvalidStatus");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreatedSurvey()
    {
        // Arrange
        var createDto = new CreateSurveyDto { Title = "New Survey" };

        _mockSurveyRepository.Setup(repo => repo.AddAsync(It.IsAny<Survey>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedSurvey = Assert.IsType<SurveyDto>(createdResult.Value);
        Assert.Equal("New Survey", returnedSurvey.Title);
        Assert.Equal("Draft", returnedSurvey.Status);
    }

    [Fact]
    public async Task Update_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var surveyId = Guid.NewGuid();
        var survey = new Survey("Test Survey");
        var updateDto = new UpdateSurveyDto { Title = "Updated Survey" };

        _mockSurveyRepository.Setup(repo => repo.GetByIdAsync(surveyId))
            .ReturnsAsync(survey);

        _mockSurveyRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Survey>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(surveyId, updateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var surveyId = Guid.NewGuid();
        var updateDto = new UpdateSurveyDto { Title = "Updated Survey" };

        _mockSurveyRepository.Setup(repo => repo.GetByIdAsync(surveyId))
            .ReturnsAsync((Survey?)null);

        // Act
        var result = await _controller.Update(surveyId, updateDto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var surveyId = Guid.NewGuid();
        var survey = new Survey("Test Survey");

        _mockSurveyRepository.Setup(repo => repo.GetByIdAsync(surveyId))
            .ReturnsAsync(survey);

        _mockSurveyRepository.Setup(repo => repo.DeleteAsync(surveyId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(surveyId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var surveyId = Guid.NewGuid();

        _mockSurveyRepository.Setup(repo => repo.GetByIdAsync(surveyId))
            .ReturnsAsync((Survey?)null);

        // Act
        var result = await _controller.Delete(surveyId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Publish_WithValidSurvey_ShouldReturnOk()
    {
        // Arrange
        var surveyId = Guid.NewGuid();
        var survey = new Survey("Test Survey");
        var question = new Question("Test Question", 1);
        question.AddOption(new Option("Option 1", 1));
        question.AddOption(new Option("Option 2", 2));
        survey.AddQuestion(question);

        _mockSurveyRepository.Setup(repo => repo.GetByIdWithQuestionsAsync(surveyId))
            .ReturnsAsync(survey);

        _mockSurveyRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Survey>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Publish(surveyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Publish_WithoutQuestions_ShouldReturnBadRequest()
    {
        // Arrange
        var surveyId = Guid.NewGuid();
        var survey = new Survey("Test Survey");

        _mockSurveyRepository.Setup(repo => repo.GetByIdWithQuestionsAsync(surveyId))
            .ReturnsAsync(survey);

        // Act
        var result = await _controller.Publish(surveyId);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Publish_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var surveyId = Guid.NewGuid();

        _mockSurveyRepository.Setup(repo => repo.GetByIdWithQuestionsAsync(surveyId))
            .ReturnsAsync((Survey?)null);

        // Act
        var result = await _controller.Publish(surveyId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Close_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var surveyId = Guid.NewGuid();
        var survey = new Survey("Test Survey");

        _mockSurveyRepository.Setup(repo => repo.GetByIdAsync(surveyId))
            .ReturnsAsync(survey);

        _mockSurveyRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Survey>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Close(surveyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Close_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var surveyId = Guid.NewGuid();

        _mockSurveyRepository.Setup(repo => repo.GetByIdAsync(surveyId))
            .ReturnsAsync((Survey?)null);

        // Act
        var result = await _controller.Close(surveyId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task AddQuestion_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var surveyId = Guid.NewGuid();
        var createQuestionDto = new CreateQuestionDto
        {
            Text = "Test Question",
            Order = 1,
            Options = new List<CreateOptionDto>
            {
                new CreateOptionDto { Text = "Option 1", Order = 1 },
                new CreateOptionDto { Text = "Option 2", Order = 2 }
            }
        };

        _mockSurveyRepository.Setup(repo => repo.AddQuestionToSurveyAsync(surveyId, It.IsAny<Question>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddQuestion(surveyId, createQuestionDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedQuestion = Assert.IsType<QuestionDto>(createdResult.Value);
        Assert.Equal("Test Question", returnedQuestion.Text);
        Assert.Equal(2, returnedQuestion.Options.Count);
    }

    [Fact]
    public async Task AddQuestion_WhenSurveyNotFound_ShouldReturnBadRequest()
    {
        // Arrange
        var surveyId = Guid.NewGuid();
        var createQuestionDto = new CreateQuestionDto
        {
            Text = "Test Question",
            Order = 1,
            Options = new List<CreateOptionDto>
            {
                new CreateOptionDto { Text = "Option 1", Order = 1 },
                new CreateOptionDto { Text = "Option 2", Order = 2 }
            }
        };

        _mockSurveyRepository.Setup(repo => repo.AddQuestionToSurveyAsync(surveyId, It.IsAny<Question>()))
            .ThrowsAsync(new InvalidOperationException("Survey not found"));

        // Act
        var result = await _controller.AddQuestion(surveyId, createQuestionDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
