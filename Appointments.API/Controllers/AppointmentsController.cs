using Appointments.Application.Appointments.Commands.CancelAppointment;
using Appointments.Application.Appointments.Commands.CreateAppointment;
using Appointments.Application.DTOs;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Appointments.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto dto)
    {
        var command = new CreateAppointmentCommand(
            dto.PatientId,
            dto.DoctorId,
            dto.ServiceId,
            dto.Date,
            dto.Time,
            dto.ServiceName,
            dto.DoctorFirstName,
            dto.DoctorLastName,
            dto.DoctorMiddleName,
            dto.PatientFirstName,
            dto.PatientLastName,
            dto.PatientMiddleName
        );

        var appointmentId = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetAppointmentById), new { id = appointmentId }, appointmentId);
    }

    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> CancelAppointment(Guid id)
    {
        try
        {
            var command = new CancelAppointmentCommand(id);
            await _mediator.Send(command);
            return Ok();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAppointmentById(Guid id)
    {
        return Ok($"ыыыыыы");
    }
}
