using Infnet.OnlineSurveys.Api.DTOs;
using Infnet.OnlineSurveys.Domain.Entities;
using Infnet.OnlineSurveys.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Infnet.OnlineSurveys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnswersController : ControllerBase
{
    private readonly IAnswerRepository _answerRepository;
    private readonly ILogger<AnswersController> _logger;

    public AnswersController(IAnswerRepository answerRepository, ILogger<AnswersController> logger)
    {
        _answerRepository = answerRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AnswerDto>>> GetAll()
    {
        var answers = await _answerRepository.GetAllAsync();
        var answerDtos = answers.Select(a => new AnswerDto
        {
            Id = a.Id,
            QuestionId = a.QuestionId,
            OptionId = a.OptionId
        });

        return Ok(answerDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AnswerDto>> GetById(Guid id)
    {
        var answer = await _answerRepository.GetByIdAsync(id);
        
        if (answer == null)
            return NotFound($"Answer with ID {id} not found.");

        var answerDto = new AnswerDto
        {
            Id = answer.Id,
            QuestionId = answer.QuestionId,
            OptionId = answer.OptionId
        };

        return Ok(answerDto);
    }

    [HttpGet("response/{responseId}")]
    public async Task<ActionResult<IEnumerable<AnswerDto>>> GetByResponseId(Guid responseId)
    {
        var answers = await _answerRepository.GetByResponseIdAsync(responseId);
        var answerDtos = answers.Select(a => new AnswerDto
        {
            Id = a.Id,
            QuestionId = a.QuestionId,
            OptionId = a.OptionId
        });

        return Ok(answerDtos);
    }

    [HttpGet("question/{questionId}")]
    public async Task<ActionResult<IEnumerable<AnswerDto>>> GetByQuestionId(Guid questionId)
    {
        var answers = await _answerRepository.GetByQuestionIdAsync(questionId);
        var answerDtos = answers.Select(a => new AnswerDto
        {
            Id = a.Id,
            QuestionId = a.QuestionId,
            OptionId = a.OptionId
        });

        return Ok(answerDtos);
    }

    [HttpPost]
    public async Task<ActionResult<AnswerDto>> Create([FromBody] CreateAnswerDto createAnswerDto)
    {
        var answer = new Answer(createAnswerDto.QuestionId, createAnswerDto.OptionId);
        await _answerRepository.AddAsync(answer);

        var answerDto = new AnswerDto
        {
            Id = answer.Id,
            QuestionId = answer.QuestionId,
            OptionId = answer.OptionId
        };

        return CreatedAtAction(nameof(GetById), new { id = answer.Id }, answerDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var answer = await _answerRepository.GetByIdAsync(id);
        
        if (answer == null)
            return NotFound($"Answer with ID {id} not found.");

        await _answerRepository.DeleteAsync(id);

        return NoContent();
    }
}
