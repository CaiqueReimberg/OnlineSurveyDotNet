using Infnet.OnlineSurveys.Api.Controllers;
using Infnet.OnlineSurveys.Api.DTOs;
using Infnet.OnlineSurveys.Domain.Entities;
using Infnet.OnlineSurveys.Domain.Enums;
using Infnet.OnlineSurveys.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Infnet.OnlineSurveys.Api.Tests.Controllers;

public class ResponsesControllerTests
{
    private readonly Mock<IResponseRepository> _mockResponseRepository;
    private readonly Mock<ISurveyRepository> _mockSurveyRepository;
    private readonly Mock<ILogger<ResponsesController>> _mockLogger;
    private readonly ResponsesController _controller;

    public ResponsesControllerTests()
    {
        _mockResponseRepository = new Mock<IResponseRepository>();
        _mockSurveyRepository = new Mock<ISurveyRepository>();
        _mockLogger = new Mock<ILogger<ResponsesController>>();
        _controller = new ResponsesController(
            _mockResponseRepository.Object,
            _mockSurveyRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllResponses()
    {
        // Arrange
        var surveyId = Guid.NewGuid();
        var responses = new List<Response>
        {
            new Response(surveyId, "João Silva", "joao@example.com"),
            new Response(surveyId, "Maria Santos", "maria@example.com")
        };

        _mockResponseRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(responses);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedResponses = Assert.IsAssignableFrom<IEnumerable<ResponseDto>>(okResult.Value);
        Assert.Equal(2, returnedResponses.Count());
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnResponse()
    {
        // Arrange
        var responseId = Guid.NewGuid();
        var surveyId = Guid.NewGuid();
        var response = new Response(surveyId, "João Silva", "joao@example.com");

        _mockResponseRepository.Setup(repo => repo.GetByIdWithAnswersAsync(responseId))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetById(responseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedResponse = Assert.IsType<ResponseDto>(okResult.Value);
        Assert.Equal("João Silva", returnedResponse.Name);
        Assert.Equal("joao@example.com", returnedResponse.Email);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var responseId = Guid.NewGuid();

        _mockResponseRepository.Setup(repo => repo.GetByIdWithAnswersAsync(responseId))
            .ReturnsAsync((Response?)null);

        // Act
        var result = await _controller.GetById(responseId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetBySurveyId_WithValidId_ShouldReturnResponses()
    {
        // Arrange
        var surveyId = Guid.NewGuid();
        var survey = new Survey("Test Survey");
        var question = new Question("Test Question", 1);
        question.AddOption(new Option("Option 1", 1));
        question.AddOption(new Option("Option 2", 2));
        survey.AddQuestion(question);

        var response = new Response(surveyId, "João Silva", "joao@example.com");
        var responses = new List<Response> { response };

        _mockSurveyRepository.Setup(repo => repo.GetByIdWithQuestionsAsync(surveyId))
            .ReturnsAsync(survey);

        _mockResponseRepository.Setup(repo => repo.GetBySurveyIdAsync(surveyId))
            .ReturnsAsync(responses);

        // Act
        var result = await _controller.GetBySurveyId(surveyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedResponses = Assert.IsAssignableFrom<IEnumerable<ResponseDto>>(okResult.Value);
        Assert.Single(returnedResponses);
    }

    [Fact]
    public async Task GetBySurveyId_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var surveyId = Guid.NewGuid();

        _mockSurveyRepository.Setup(repo => repo.GetByIdWithQuestionsAsync(surveyId))
            .ReturnsAsync((Survey?)null);

        // Act
        var result = await _controller.GetBySurveyId(surveyId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByEmail_WithValidEmail_ShouldReturnResponses()
    {
        // Arrange
        var email = "joao@example.com";
        var surveyId = Guid.NewGuid();
        var responses = new List<Response>
        {
            new Response(surveyId, "João Silva", email)
        };

        _mockResponseRepository.Setup(repo => repo.GetByEmailAsync(email))
            .ReturnsAsync(responses);

        // Act
        var result = await _controller.GetByEmail(email);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedResponses = Assert.IsAssignableFrom<IEnumerable<ResponseDto>>(okResult.Value);
        Assert.Single(returnedResponses);
        Assert.All(returnedResponses, r => Assert.Equal(email, r.Email));
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var surveyId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var optionId = Guid.NewGuid();

        var survey = new Survey("Test Survey");
        var question = new Question("Test Question", 1);
        question.AddOption(new Option("Option 1", 1));
        question.AddOption(new Option("Option 2", 2));
        survey.AddQuestion(question);
        survey.Publish();

        var createDto = new CreateResponseDto
        {
            SurveyId = surveyId,
            Name = "João Silva",
            Email = "joao@example.com",
            Answers = new List<CreateAnswerDto>
            {
                new CreateAnswerDto { QuestionId = questionId, OptionId = optionId }
            }
        };

        _mockSurveyRepository.Setup(repo => repo.GetByIdAsync(surveyId))
            .ReturnsAsync(survey);

        _mockResponseRepository.Setup(repo => repo.AddAsync(It.IsAny<Response>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedResponse = Assert.IsType<ResponseDto>(createdResult.Value);
        Assert.Equal("João Silva", returnedResponse.Name);
        Assert.Equal("joao@example.com", returnedResponse.Email);
        Assert.Single(returnedResponse.Answers);
    }

    [Fact]
    public async Task Create_WithInvalidSurveyId_ShouldReturnNotFound()
    {
        // Arrange
        var surveyId = Guid.NewGuid();
        var createDto = new CreateResponseDto
        {
            SurveyId = surveyId,
            Name = "João Silva",
            Email = "joao@example.com",
            Answers = new List<CreateAnswerDto>()
        };

        _mockSurveyRepository.Setup(repo => repo.GetByIdAsync(surveyId))
            .ReturnsAsync((Survey?)null);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_WithUnpublishedSurvey_ShouldReturnBadRequest()
    {
        // Arrange
        var surveyId = Guid.NewGuid();
        var survey = new Survey("Test Survey"); // Status = Draft

        var createDto = new CreateResponseDto
        {
            SurveyId = surveyId,
            Name = "João Silva",
            Email = "joao@example.com",
            Answers = new List<CreateAnswerDto>()
        };

        _mockSurveyRepository.Setup(repo => repo.GetByIdAsync(surveyId))
            .ReturnsAsync(survey);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var responseId = Guid.NewGuid();
        var surveyId = Guid.NewGuid();
        var response = new Response(surveyId, "João Silva", "joao@example.com");

        _mockResponseRepository.Setup(repo => repo.GetByIdAsync(responseId))
            .ReturnsAsync(response);

        _mockResponseRepository.Setup(repo => repo.DeleteAsync(responseId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(responseId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var responseId = Guid.NewGuid();

        _mockResponseRepository.Setup(repo => repo.GetByIdAsync(responseId))
            .ReturnsAsync((Response?)null);

        // Act
        var result = await _controller.Delete(responseId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
