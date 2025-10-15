using Appointments.Application.Appointments.Commands.ApproveAppointment;
using Appointments.Application.Appointments.Commands.CancelAppointment;
using Appointments.Application.Appointments.Commands.CreateAppointment;
using Appointments.Application.Appointments.Commands.RescheduleAppointment;
using Appointments.Application.Appointments.Queries.GetAppointments.GetAppointmentsForDoctor;
using Appointments.Application.Appointments.Queries.GetAppointments.GetAppointmentsForPatient;
using Appointments.Application.Appointments.Queries.GetAppointments.GetAppointmentsForReceptionist;
using Appointments.Application.Appointments.Queries.GetById.GetAppointmentAsDoctor;
using Appointments.Application.Appointments.Queries.GetById.GetAppointmentAsPatient;
using Appointments.Application.Appointments.Queries.GetById.GetAppointmentAsReceptionist;
using Appointments.Application.Appointments.Queries.GetFreeSlots;
using Appointments.Application.Dtos;
using Appointments.Application.Exceptions;
using Appointments.Domain.Dtos;

using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Appointments.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public AppointmentsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto dto)
    {
        try
        {
            var command = _mapper.Map<CreateAppointmentCommand>(dto);
            var appointmentId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAppointmentForReceptionist), new { id = appointmentId }, appointmentId);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    [HttpPatch("{id}/cancel")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CancelAppointment(Guid id)
    {
        try
        {
            var command = new CancelAppointmentCommand(id);
            await _mediator.Send(command);
            return Ok();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    [HttpGet("doctor/{id:guid}")]
    [ProducesResponseType(typeof(AppointmentForDoctorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentForDoctor(Guid id)
    {
        var query = new GetAppointmentAsDoctorQuery(id);
        var appointment = await _mediator.Send(query);
        return appointment is null ? NotFound() : Ok(appointment);
    }

    [HttpGet("patient/{id:guid}")]
    [ProducesResponseType(typeof(AppointmentForPatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentForPatient(Guid id)
    {
        var query = new GetAppointmentAsPatientQuery(id);
        var appointment = await _mediator.Send(query);
        return appointment is null ? NotFound() : Ok(appointment);
    }

    [HttpGet("receptionist/{id:guid}")]
    [ProducesResponseType(typeof(AppointmentForReceptionistDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentForReceptionist(Guid id)
    {
        var query = new GetAppointmentAsReceptionistQuery(id);
        var appointment = await _mediator.Send(query);

        return appointment is null ? NotFound() : Ok(appointment);
    }

    [HttpGet("doctor")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentForDoctorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentsForDoctor(
        [FromQuery] Guid doctorId,
        [FromQuery] DateTime date,
        [FromQuery] int pageSize = 20,
        [FromQuery] int pageNumber = 1)
    {
        var query = new GetAppointmentsForDoctorQuery(doctorId, pageSize, pageNumber, date);
        var appointments = await _mediator.Send(query);
        return Ok(appointments ?? Enumerable.Empty<AppointmentForDoctorDto>());
    }

    [HttpGet("patient")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentForPatientDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentsForPatient([FromQuery] Guid patientId, [FromQuery] int pageSize = 20, [FromQuery] int pageNumber = 1)
    {
        var query = new GetAppointmentsForPatientQuery(patientId, pageSize, pageNumber);
        var appointments = await _mediator.Send(query);
        return Ok(appointments ?? Enumerable.Empty<AppointmentForPatientDto>());
    }

    [HttpGet("receptionist")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentForReceptionistDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentsForReceptionist(int pageSize, int pageNumber, DateTime? date, string? doctorFullName, string? serviceName, short? status, Guid? officeId)
    {
        var query = new GetAppointmentsForReceptionistQuery(pageSize, pageNumber, date, doctorFullName, serviceName, status, officeId);
        var appointments = await _mediator.Send(query);
        return Ok(appointments ?? Enumerable.Empty<AppointmentForReceptionistDto>());
    }

    [HttpPatch("{id}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ApproveAppointment(Guid id)
    {
        try
        {
            var command = new ApproveAppointmentCommand(id);
            await _mediator.Send(command);
            return Ok();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    [HttpPatch("{id:guid}/reschedule")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RescheduleAppointment(Guid id, [FromBody] RescheduleAppointmentDto dto)
    {
        try
        {
            var command = new RescheduleAppointmentCommand(id, dto.NewDate, dto.NewTime);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    [HttpGet("doctors/{doctorId:guid}/free-slots")]
    [ProducesResponseType(typeof(IEnumerable<TimeSpan>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFreeSlots(Guid doctorId, [FromQuery] DateTime date)
    {
        try
        {
            var query = new GetFreeSlotsQuery(doctorId, date);
            var slots = await _mediator.Send(query);
            return Ok(slots);
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
