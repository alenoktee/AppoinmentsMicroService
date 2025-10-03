using Appointments.API.Infrastructure.Data;
using Appointments.Application.Dtos;
using Appointments.Domain.Entities;
using Appointments.Domain.Enums;
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

        var sql = "SELECT create_appointment(" +
                           "@Id, @PatientId, @DoctorId, @ServiceId, @OfficeId, @Date, @Time, " +
                           "@ServiceName, @DoctorFirstName, @DoctorLastName, @DoctorMiddleName, " +
                           "@PatientFirstName, @PatientLastName, @PatientMiddleName)";

        await connection.ExecuteAsync(sql, appointment);

        return appointment;
    }

    public async Task<int> ChangeStatusAsync(Guid id, short status)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT update_appointment_status(@appointment_id, @new_status)";

        return await connection.ExecuteAsync(sql, new { appointment_id = id, new_status = status });
    }

    public async Task<AppointmentForDoctorDto?> GetForDoctorByIdAsync(Guid id)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT * FROM get_appointment_for_doctor(@appointment_id)";

        return await connection.QuerySingleOrDefaultAsync<AppointmentForDoctorDto>(
            sql,
            new { appointment_id = id });
    }

    public async Task<AppointmentForPatientDto?> GetForPatientByIdAsync(Guid id)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT * FROM get_appointment_for_patient(@appointment_id)";

        return await connection.QuerySingleOrDefaultAsync<AppointmentForPatientDto>(
            sql,
            new { appointment_id = id });
    }

    public async Task<AppointmentForReceptionistDto?> GetForReceptionistByIdAsync(Guid id)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT * FROM get_appointment_for_receptionist(@appointment_id)";

        return await connection.QuerySingleOrDefaultAsync<AppointmentForReceptionistDto>(
            sql,
            new { appointment_id = id });
    }

    public Task<Appointment?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Appointment>> GetAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<AppointmentForDoctorDto>> GetForDoctorPaginatedAsync(Guid doctorId, int pageSize, int pageNumber, DateTime date)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT * FROM get_appointments_for_doctor_paginated(@doctor_id, @page_size, @page_number, @filter_date)";

        var parameters = new
        {
            doctor_id = doctorId,
            page_size = pageSize,
            page_number = pageNumber,
            filter_date = date
        };

        return await connection.QueryAsync<AppointmentForDoctorDto>(sql, parameters);
    }

    public async Task<IEnumerable<AppointmentForPatientDto>> GetForPatientPaginatedAsync(Guid patientId, int pageSize, int pageNumber)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT * FROM get_appointments_for_patient_paginated(@patient_id, @page_size, @page_number)";

        var parameters = new
        {
            patient_id = patientId,
            page_size = pageSize,
            page_number = pageNumber
        };

        return await connection.QueryAsync<AppointmentForPatientDto>(sql, parameters);
    }

    public async Task<IEnumerable<AppointmentForReceptionistDto>> GetForReceptionistPaginatedAsync(int pageSize, int pageNumber, DateTime? date, string? doctorFullName, string? serviceName, short? status, Guid? officeId)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT * FROM get_appointments_for_receptionist_paginated(" +
                           "@page_size, @page_number, @filter_date, @doctor_full_name, " +
                           "@service_name, @filter_status, @office_id)";

        var parameters = new
        {
            page_size = pageSize,
            page_number = pageNumber,
            filter_date = date,
            doctor_full_name = doctorFullName,
            service_name = serviceName,
            filter_status = status,
            office_id = officeId
        };

        return await connection.QueryAsync<AppointmentForReceptionistDto>(sql, parameters);
    }

    public async Task<int> ApproveAsync(Guid id)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT update_appointment_status(@appointment_id, @new_status)";

        var parameters = new
        {
            appointment_id = id,
            new_status = (short)AppointmentStatus.Approved
        };

        return await connection.ExecuteAsync(sql, parameters);
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
