using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoicePulse.API.Extensions;
using VoicePulse.API.Filters;
using VoicePulse.Application.Common.Consts;
using VoicePulse.Application.Common.Errors;
using VoicePulse.Application.Common.Models;
using VoicePulse.Application.Contracts.Questions;
using VoicePulse.Application.Interfaces;
using VoicePulse.Application.Services;

namespace VoicePulse.API.Controllers;

[Route("api/polls/{pollId}/[controller]")]
[ApiController]
public class QuestionsController(IQuestionService questionService) : ControllerBase
{
    private readonly IQuestionService _questionservice = questionService;

    [HttpGet("")]
    [HasPermission(Permissions.GetQuestions)]
    public async Task<IActionResult> GetAll([FromRoute] int pollId, [FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var result = await _questionservice.GetAllAsync(pollId, filters, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : result.ToProblem();
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.GetQuestions)]
    public async Task<IActionResult> Get([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _questionservice.GetAsync(pollId ,id , cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost("")]
    [HasPermission(Permissions.AddQuestions)]
    public async Task<IActionResult> Add([FromRoute] int pollId , [FromBody] QuestionRequest request , CancellationToken cancellationToken)
    {
        var result = await _questionservice.AddAsync(pollId,request,cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { pollId, result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateQuestions)]
    public async Task<IActionResult> Update([FromRoute] int pollId, [FromRoute] int id, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
    {
        var result = await _questionservice.UpdateAsync(pollId, id, request, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }


    [HttpPut("{id}/toggle-status")]
    [HasPermission(Permissions.UpdateQuestions)]
    public async Task<IActionResult> ToggleStatus([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _questionservice.ToggleStatusAsync(pollId, id, cancellationToken);

        return result.IsSuccess 
            ? NoContent() 
            : result.ToProblem();
    }
}
