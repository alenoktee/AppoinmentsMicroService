using Appointments.API.Infrastructure.Data;
using Appointments.Application.Dtos;
using Appointments.Domain.Entities;
using Appointments.Domain.Interfaces;

using Dapper;

using Npgsql;

using System.Data;

namespace Appointments.Infrastructure.Repositories;

public class AppointmentsRepository : IAppointmentsRepository
{
    private readonly DapperContext _context;

    public AppointmentsRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Appointment> CreateAsync(Appointment appointment)
    {
        using var connection = _context.CreateConnection();

        var parameters = new
        {
            p_id = appointment.Id,
            p_patient_id = appointment.PatientId,
            p_doctor_id = appointment.DoctorId,
            p_service_id = appointment.ServiceId,
            p_date = appointment.Date,
            p_time = appointment.Time,
            p_service_name = appointment.ServiceName,
            p_doctor_first_name = appointment.DoctorFirstName,
            p_doctor_last_name = appointment.DoctorLastName,
            p_doctor_middle_name = appointment.DoctorMiddleName,
            p_patient_first_name = appointment.PatientFirstName,
            p_patient_last_name = appointment.PatientLastName,
            p_patient_middle_name = appointment.PatientMiddleName
        };

        var sql = "SELECT create_appointment(" +
              "@p_id, @p_patient_id, @p_doctor_id, @p_service_id, @p_date, @p_time, " +
              "@p_service_name, @p_doctor_first_name, @p_doctor_last_name, @p_doctor_middle_name, " +
              "@p_patient_first_name, @p_patient_last_name, @p_patient_middle_name)";

        await connection.ExecuteAsync(sql, parameters);

        return appointment;
    }

    public async Task<int> ChangeStatusAsync(Guid id, short status)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT update_appointment_status(@p_id, @p_status)";
        var parameters = new { p_id = id, p_status = status };

        return await connection.ExecuteAsync(sql, parameters);
    }

    public async Task<AppointmentForDoctorDto?> GetForDoctorByIdAsync(Guid id)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT * FROM get_appointment_for_doctor(@id)";
        return await connection.QuerySingleOrDefaultAsync<AppointmentForDoctorDto>(sql, new { id });
    }

    public async Task<AppointmentForPatientDto?> GetForPatientByIdAsync(Guid id)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT * FROM get_appointment_for_patient(@id)";
        return await connection.QuerySingleOrDefaultAsync<AppointmentForPatientDto>(sql, new { id });
    }

    public async Task<AppointmentForReceptionistDto?> GetForReceptionistByIdAsync(Guid id)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT * FROM get_appointment_for_receptionist(@id)";
        return await connection.QuerySingleOrDefaultAsync<AppointmentForReceptionistDto>(sql, new { id });
    }

    public Task<Appointment?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Appointment>> GetAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AppointmentForDoctorDto>> GetForDoctorPaginatedAsync(Guid doctorId, int pageSize, int pageNumber)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AppointmentForPatientDto>> GetForPatientPaginatedAsync(Guid patientId, int pageSize, int pageNumber)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Appointment?> GetAppointmentWithResultAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
