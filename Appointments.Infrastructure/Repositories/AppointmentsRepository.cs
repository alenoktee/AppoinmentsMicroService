using Appointments.API.Infrastructure.Data;
using Appointments.Domain.Interfaces;
using Appointments.Domain.Entities;

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

    public async Task<Appointment> CreateAsync(Appointment appointment) // Метод теперь просто принимает готовый appointment
    {
        using var connection = new NpgsqlConnection(_context.ConnectionString);

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

    public async Task<Appointment?> GetAppointmentWithResultAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Appointment>> GetAsDoctorAsync(Guid doctorId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Appointment>> GetAsPatientAsync(Guid patientId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Appointment>> GetAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task ChangeStatusAsync(Guid id, short status)
    {
        throw new NotImplementedException();
    }
}
