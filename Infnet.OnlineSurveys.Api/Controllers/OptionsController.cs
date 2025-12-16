using Infnet.OnlineSurveys.Api.DTOs;
using Infnet.OnlineSurveys.Domain.Entities;
using Infnet.OnlineSurveys.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Infnet.OnlineSurveys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OptionsController : ControllerBase
{
    private readonly IOptionRepository _optionRepository;
    private readonly ILogger<OptionsController> _logger;

    public OptionsController(IOptionRepository optionRepository, ILogger<OptionsController> logger)
    {
        _optionRepository = optionRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OptionDto>>> GetAll()
    {
        var options = await _optionRepository.GetAllAsync();
        var optionDtos = options.Select(o => new OptionDto
        {
            Id = o.Id,
            Text = o.Text,
            Order = o.Order
        });

        return Ok(optionDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OptionDto>> GetById(Guid id)
    {
        var option = await _optionRepository.GetByIdAsync(id);
        
        if (option == null)
            return NotFound($"Option with ID {id} not found.");

        var optionDto = new OptionDto
        {
            Id = option.Id,
            Text = option.Text,
            Order = option.Order
        };

        return Ok(optionDto);
    }

    [HttpGet("question/{questionId}")]
    public async Task<ActionResult<IEnumerable<OptionDto>>> GetByQuestionId(Guid questionId)
    {
        var options = await _optionRepository.GetByQuestionIdAsync(questionId);
        var optionDtos = options.Select(o => new OptionDto
        {
            Id = o.Id,
            Text = o.Text,
            Order = o.Order
        });

        return Ok(optionDtos);
    }

    [HttpPost]
    public async Task<ActionResult<OptionDto>> Create([FromBody] CreateOptionDto createOptionDto)
    {
        var option = new Option(createOptionDto.Text, createOptionDto.Order);
        await _optionRepository.AddAsync(option);

        var optionDto = new OptionDto
        {
            Id = option.Id,
            Text = option.Text,
            Order = option.Order
        };

        return CreatedAtAction(nameof(GetById), new { id = option.Id }, optionDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateOptionDto updateOptionDto)
    {
        var option = await _optionRepository.GetByIdAsync(id);
        
        if (option == null)
            return NotFound($"Option with ID {id} not found.");

        await _optionRepository.UpdateAsync(option);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var option = await _optionRepository.GetByIdAsync(id);
        
        if (option == null)
            return NotFound($"Option with ID {id} not found.");

        await _optionRepository.DeleteAsync(id);

        return NoContent();
    }
}
