using Infnet.OnlineSurveys.Api.DTOs;
using Infnet.OnlineSurveys.Domain.Entities;
using Infnet.OnlineSurveys.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Infnet.OnlineSurveys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ILogger<QuestionsController> _logger;

    public QuestionsController(IQuestionRepository questionRepository, ILogger<QuestionsController> logger)
    {
        _questionRepository = questionRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetAll()
    {
        var questions = await _questionRepository.GetAllAsync();
        var questionDtos = questions.Select(q => new QuestionDto
        {
            Id = q.Id,
            Text = q.Text,
            Order = q.Order,
            Options = q.Options.Select(o => new OptionDto
            {
                Id = o.Id,
                Text = o.Text,
                Order = o.Order
            }).ToList()
        });

        return Ok(questionDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuestionDto>> GetById(Guid id)
    {
        var question = await _questionRepository.GetByIdWithOptionsAsync(id);
        
        if (question == null)
            return NotFound($"Question with ID {id} not found.");

        var questionDto = new QuestionDto
        {
            Id = question.Id,
            Text = question.Text,
            Order = question.Order,
            Options = question.Options.Select(o => new OptionDto
            {
                Id = o.Id,
                Text = o.Text,
                Order = o.Order
            }).ToList()
        };

        return Ok(questionDto);
    }

    [HttpGet("survey/{surveyId}")]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetBySurveyId(Guid surveyId)
    {
        var questions = await _questionRepository.GetBySurveyIdAsync(surveyId);
        var questionDtos = questions.Select(q => new QuestionDto
        {
            Id = q.Id,
            Text = q.Text,
            Order = q.Order,
            Options = q.Options.Select(o => new OptionDto
            {
                Id = o.Id,
                Text = o.Text,
                Order = o.Order
            }).ToList()
        });

        return Ok(questionDtos);
    }

    [HttpPost]
    public async Task<ActionResult<QuestionDto>> Create([FromBody] CreateQuestionDto createQuestionDto)
    {
        var question = new Question(createQuestionDto.Text, createQuestionDto.Order);
        
        foreach (var optionDto in createQuestionDto.Options)
        {
            var option = new Option(optionDto.Text, optionDto.Order);
            question.AddOption(option);
        }

        await _questionRepository.AddAsync(question);

        var questionDto = new QuestionDto
        {
            Id = question.Id,
            Text = question.Text,
            Order = question.Order,
            Options = question.Options.Select(o => new OptionDto
            {
                Id = o.Id,
                Text = o.Text,
                Order = o.Order
            }).ToList()
        };

        return CreatedAtAction(nameof(GetById), new { id = question.Id }, questionDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateQuestionDto updateQuestionDto)
    {
        var question = await _questionRepository.GetByIdAsync(id);
        
        if (question == null)
            return NotFound($"Question with ID {id} not found.");

        await _questionRepository.UpdateAsync(question);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var question = await _questionRepository.GetByIdAsync(id);
        
        if (question == null)
            return NotFound($"Question with ID {id} not found.");

        await _questionRepository.DeleteAsync(id);

        return NoContent();
    }

    [HttpPost("{id}/options")]
    public async Task<ActionResult<OptionDto>> AddOption(Guid id, [FromBody] CreateOptionDto createOptionDto)
    {
        var question = await _questionRepository.GetByIdWithOptionsAsync(id);
        
        if (question == null)
            return NotFound($"Question with ID {id} not found.");

        var option = new Option(createOptionDto.Text, createOptionDto.Order);
        question.AddOption(option);
        
        await _questionRepository.UpdateAsync(question);

        var optionDto = new OptionDto
        {
            Id = option.Id,
            Text = option.Text,
            Order = option.Order
        };

        return CreatedAtAction(nameof(GetById), new { id = question.Id }, optionDto);
    }
}
