using Appointments.Application.Appointments.Commands.CancelAppointment;
using Appointments.Application.Appointments.Commands.CreateAppointment;
using Appointments.Application.Appointments.Queries.GetAppointmentAsDoctor;
using Appointments.Application.Appointments.Queries.GetAppointmentAsPatient;
using Appointments.Application.Appointments.Queries.GetAppointmentAsReceptionist;
using Appointments.Application.Dtos;

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

        return CreatedAtAction(nameof(GetAppointmentForReceptionist), new { id = appointmentId }, appointmentId);
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

    [HttpGet("doctor/{id:guid}")]
    [ProducesResponseType(typeof(AppointmentForDoctorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentForDoctor(Guid id)
    {
        var query = new GetAppointmentAsDoctorQuery(id);
        var result = await _mediator.Send(query);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("patient/{id:guid}")]
    [ProducesResponseType(typeof(AppointmentForPatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentForPatient(Guid id)
    {
        var query = new GetAppointmentAsPatientQuery(id);
        var result = await _mediator.Send(query);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("receptionist/{id:guid}")]
    [ProducesResponseType(typeof(AppointmentForReceptionistDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentForReceptionist(Guid id)
    {
        var query = new GetAppointmentAsReceptionistQuery(id);
        var result = await _mediator.Send(query);

        return result is null ? NotFound() : Ok(result);
    }
}
