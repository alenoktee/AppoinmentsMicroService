using Appointments.Application.Appointments.Commands.ApproveAppointment;
using Appointments.Application.Appointments.Commands.CancelAppointment;
using Appointments.Application.Appointments.Commands.CreateAppointment;
using Appointments.Application.Appointments.Queries.GetAppointments.GetAppointmentsForDoctor;
using Appointments.Application.Appointments.Queries.GetAppointments.GetAppointmentsForPatient;
using Appointments.Application.Appointments.Queries.GetAppointments.GetAppointmentsForReceptionist;
using Appointments.Application.Appointments.Queries.GetById.GetAppointmentAsDoctor;
using Appointments.Application.Appointments.Queries.GetById.GetAppointmentAsPatient;
using Appointments.Application.Appointments.Queries.GetById.GetAppointmentAsReceptionist;
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

    [HttpGet("doctor")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentForDoctorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAppointmentsForDoctor(
        [FromQuery] Guid doctorId,
        [FromQuery] DateTime date,
        [FromQuery] int pageSize = 20,
        [FromQuery] int pageNumber = 1)
    {
        var query = new GetAppointmentsForDoctorQuery(doctorId, pageSize, pageNumber, date);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("patient")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentForPatientDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAppointmentsForPatient([FromQuery] Guid patientId, [FromQuery] int pageSize = 20, [FromQuery] int pageNumber = 1)
    {
        var query = new GetAppointmentsForPatientQuery(patientId, pageSize, pageNumber);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPatch("{id}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveAppointment(Guid id)
    {
        try
        {
            var command = new ApproveAppointmentCommand(id);
            await _mediator.Send(command);
            return Ok();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("receptionist")]
    public async Task<IActionResult> GetAppointmentsForReceptionist(
    int pageSize, int pageNumber, DateTime? date,
    string? doctorFullName, string? serviceName, int? status, Guid? officeId) // Добавлен officeId
    {
        var query = new GetAppointmentsForReceptionistQuery(
            pageSize, pageNumber, date, doctorFullName, serviceName, status, officeId); // Добавлен officeId
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
