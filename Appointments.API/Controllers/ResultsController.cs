using Appointments.Application.Exceptions;
using Appointments.Application.Results.Commands.CreateResultCommand;
using Appointments.Application.Results.Commands.UpdateResultCommand;
using Appointments.Application.Results.Queries.GetResultByIdQuery;
using Appointments.Application.Results.Queries.GetResultXml;
using Appointments.Domain.Dtos;
using Appointments.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Appointments.API.Controllers;

[ApiController]
[Route("api/results")]
public class ResultsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResultsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{appointmentId:guid}/results")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateResultForAppointment(Guid appointmentId, [FromBody] CreateResultDto dto)
    {
        try
        {
            var command = new CreateResultCommand(
            appointmentId,
            dto.Complaints,
            dto.Conclusion,
            dto.Recommendations
        );

            var resultId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetResultById), new { id = resultId }, resultId);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error occurred: {ex}");
        }
    }

    [HttpGet("{id:guid}", Name = "GetResultById")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetResultById(Guid id)
    {
        var query = new GetResultByIdQuery(id);
        var result = await _mediator.Send(query);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("{id:guid}", Name = "UpdateResult")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateResult(Guid id, [FromBody] UpdateResultDto dto)
    {
        try
        {
            var command = new UpdateResultCommand(
                id,
                dto.Complaints,
                dto.Conclusion,
                dto.Recommendations
            );
            await _mediator.Send(command);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error occurred: {ex}");
        }
    }

    [HttpGet("{id:guid}/xml-result")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetXml(Guid id)
    {
        try
        {
            var query = new GetResultXmlQuery(id);
            var fileBytes = await _mediator.Send(query);
            string fileName = $"result_{id}.xml";
            return File(fileBytes, "application/xml", fileName);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error occurred: {ex}");
        }
    }
}
