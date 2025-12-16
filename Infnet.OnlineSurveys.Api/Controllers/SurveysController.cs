using Infnet.OnlineSurveys.Api.DTOs;
using Infnet.OnlineSurveys.Domain.Entities;
using Infnet.OnlineSurveys.Domain.Enums;
using Infnet.OnlineSurveys.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Infnet.OnlineSurveys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SurveysController : ControllerBase
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<SurveysController> _logger;

    public SurveysController(ISurveyRepository surveyRepository, ILogger<SurveysController> logger)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SurveyDto>>> GetAll()
    {
        var surveys = await _surveyRepository.GetAllAsync();
        var surveyDtos = surveys.Select(s => new SurveyDto
        {
            Id = s.Id,
            Title = s.Title,
            Status = s.Status.ToString(),
            CreatedAt = s.CreatedAt,
            Questions = s.Questions.Select(q => new QuestionDto
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
            }).ToList()
        });

        return Ok(surveyDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SurveyDto>> GetById(Guid id)
    {
        var survey = await _surveyRepository.GetByIdWithQuestionsAsync(id);
        
        if (survey == null)
            return NotFound($"Survey with ID {id} not found.");

        var surveyDto = new SurveyDto
        {
            Id = survey.Id,
            Title = survey.Title,
            Status = survey.Status.ToString(),
            CreatedAt = survey.CreatedAt,
            Questions = survey.Questions.Select(q => new QuestionDto
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
            }).ToList()
        };

        return Ok(surveyDto);
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<SurveyDto>>> GetByStatus(string status)
    {
        if (!Enum.TryParse<SurveyStatus>(status, true, out var surveyStatus))
            return BadRequest($"Invalid status. Valid values are: {string.Join(", ", Enum.GetNames<SurveyStatus>())}");

        var surveys = await _surveyRepository.GetByStatusAsync(surveyStatus);
        var surveyDtos = surveys.Select(s => new SurveyDto
        {
            Id = s.Id,
            Title = s.Title,
            Status = s.Status.ToString(),
            CreatedAt = s.CreatedAt,
            Questions = s.Questions.Select(q => new QuestionDto
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
            }).ToList()
        });

        return Ok(surveyDtos);
    }

    [HttpPost]
    public async Task<ActionResult<SurveyDto>> Create([FromBody] CreateSurveyDto createSurveyDto)
    {
        var survey = new Survey(createSurveyDto.Title);
        await _surveyRepository.AddAsync(survey);

        var surveyDto = new SurveyDto
        {
            Id = survey.Id,
            Title = survey.Title,
            Status = survey.Status.ToString(),
            CreatedAt = survey.CreatedAt,
            Questions = new List<QuestionDto>()
        };

        return CreatedAtAction(nameof(GetById), new { id = survey.Id }, surveyDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateSurveyDto updateSurveyDto)
    {
        var survey = await _surveyRepository.GetByIdAsync(id);
        
        if (survey == null)
            return NotFound($"Survey with ID {id} not found.");

        await _surveyRepository.UpdateAsync(survey);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var survey = await _surveyRepository.GetByIdAsync(id);
        
        if (survey == null)
            return NotFound($"Survey with ID {id} not found.");

        await _surveyRepository.DeleteAsync(id);

        return NoContent();
    }

    [HttpPost("{id}/publish")]
    public async Task<ActionResult> Publish(Guid id)
    {
        var survey = await _surveyRepository.GetByIdWithQuestionsAsync(id);
        
        if (survey == null)
            return NotFound($"Survey with ID {id} not found.");

        try
        {
            survey.Publish();
            await _surveyRepository.UpdateAsync(survey);
            return Ok(new { message = "Survey published successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/close")]
    public async Task<ActionResult> Close(Guid id)
    {
        var survey = await _surveyRepository.GetByIdAsync(id);
        
        if (survey == null)
            return NotFound($"Survey with ID {id} not found.");

        survey.Close();
        await _surveyRepository.UpdateAsync(survey);

        return Ok(new { message = "Survey closed successfully." });
    }

    [HttpPost("{id}/questions")]
    public async Task<ActionResult<QuestionDto>> AddQuestion(Guid id, [FromBody] CreateQuestionDto createQuestionDto)
    {
        try
        {
            var question = new Question(createQuestionDto.Text, createQuestionDto.Order);
            
            foreach (var optionDto in createQuestionDto.Options)
            {
                var option = new Option(optionDto.Text, optionDto.Order);
                question.AddOption(option);
            }

            await _surveyRepository.AddQuestionToSurveyAsync(id, question);

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

            return CreatedAtAction(nameof(GetById), new { id = id }, questionDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
