using Infnet.OnlineSurveys.Api.DTOs;
using Infnet.OnlineSurveys.Domain.Entities;
using Infnet.OnlineSurveys.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Infnet.OnlineSurveys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResponsesController : ControllerBase
{
    private readonly IResponseRepository _responseRepository;
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<ResponsesController> _logger;

    public ResponsesController(
        IResponseRepository responseRepository,
        ISurveyRepository surveyRepository,
        ILogger<ResponsesController> logger)
    {
        _responseRepository = responseRepository;
        _surveyRepository = surveyRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ResponseDto>>> GetAll()
    {
        var responses = await _responseRepository.GetAllAsync();
        var responseDtos = responses.Select(r => new ResponseDto
        {
            Id = r.Id,
            SurveyId = r.SurveyId,
            Name = r.Name,
            Email = r.Email,
            SubmittedAt = r.SubmittedAt,
            Answers = r.Answers.Select(a => new AnswerDto
            {
                Id = a.Id,
                QuestionId = a.QuestionId,
                OptionId = a.OptionId
            }).ToList()
        });

        return Ok(responseDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseDto>> GetById(Guid id)
    {
        var response = await _responseRepository.GetByIdWithAnswersAsync(id);
        
        if (response == null)
            return NotFound($"Response with ID {id} not found.");

        var responseDto = new ResponseDto
        {
            Id = response.Id,
            SurveyId = response.SurveyId,
            Name = response.Name,
            Email = response.Email,
            SubmittedAt = response.SubmittedAt,
            Answers = response.Answers.Select(a => new AnswerDto
            {
                Id = a.Id,
                QuestionId = a.QuestionId,
                OptionId = a.OptionId
            }).ToList()
        };

        return Ok(responseDto);
    }

    [HttpGet("survey/{surveyId}")]
    public async Task<ActionResult<IEnumerable<ResponseDto>>> GetBySurveyId(Guid surveyId)
    {
        var survey = await _surveyRepository.GetByIdWithQuestionsAsync(surveyId);
        
        if (survey == null)
            return NotFound($"Survey with ID {surveyId} not found.");

        var responses = await _responseRepository.GetBySurveyIdAsync(surveyId);
        
        var responseDtos = responses.Select(r => new ResponseDto
        {
            Id = r.Id,
            SurveyId = r.SurveyId,
            Name = r.Name,
            Email = r.Email,
            SubmittedAt = r.SubmittedAt,
            Answers = r.Answers.Select(a =>
            {
                var question = survey.Questions.FirstOrDefault(q => q.Id == a.QuestionId);
                var option = question?.Options.FirstOrDefault(o => o.Id == a.OptionId);
                
                return new AnswerDto
                {
                    Id = a.Id,
                    QuestionId = a.QuestionId,
                    OptionId = a.OptionId,
                    QuestionText = question?.Text ?? "Question not found",
                    OptionText = option?.Text ?? "Option not found"
                };
            }).ToList()
        });

        return Ok(responseDtos);
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<IEnumerable<ResponseDto>>> GetByEmail(string email)
    {
        var responses = await _responseRepository.GetByEmailAsync(email);
        var responseDtos = responses.Select(r => new ResponseDto
        {
            Id = r.Id,
            SurveyId = r.SurveyId,
            Name = r.Name,
            Email = r.Email,
            SubmittedAt = r.SubmittedAt,
            Answers = r.Answers.Select(a => new AnswerDto
            {
                Id = a.Id,
                QuestionId = a.QuestionId,
                OptionId = a.OptionId
            }).ToList()
        });

        return Ok(responseDtos);
    }

    [HttpPost]
    public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateResponseDto createResponseDto)
    {
        var survey = await _surveyRepository.GetByIdAsync(createResponseDto.SurveyId);
        
        if (survey == null)
            return NotFound($"Survey with ID {createResponseDto.SurveyId} not found.");

        if (survey.Status != Domain.Enums.SurveyStatus.Published)
            return BadRequest("Survey is not published. Only published surveys can receive responses.");

        var response = new Response(createResponseDto.SurveyId, createResponseDto.Name, createResponseDto.Email);
        
        foreach (var answerDto in createResponseDto.Answers)
        {
            var answer = new Answer(answerDto.QuestionId, answerDto.OptionId);
            response.AddAnswer(answer);
        }

        await _responseRepository.AddAsync(response);

        var responseDto = new ResponseDto
        {
            Id = response.Id,
            SurveyId = response.SurveyId,
            Name = response.Name,
            Email = response.Email,
            SubmittedAt = response.SubmittedAt,
            Answers = response.Answers.Select(a => new AnswerDto
            {
                Id = a.Id,
                QuestionId = a.QuestionId,
                OptionId = a.OptionId
            }).ToList()
        };

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, responseDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var response = await _responseRepository.GetByIdAsync(id);
        
        if (response == null)
            return NotFound($"Response with ID {id} not found.");

        await _responseRepository.DeleteAsync(id);

        return NoContent();
    }
}
