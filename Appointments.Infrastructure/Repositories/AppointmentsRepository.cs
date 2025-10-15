using Appointments.API.Infrastructure.Data;
using Appointments.Application.Dtos;
using Appointments.Domain.Dtos;
using Appointments.Domain.Entities;
using Appointments.Domain.Enums;
using Appointments.Domain.Interfaces;
using AutoMapper;
using Dapper;
using System.Data;

namespace Appointments.Infrastructure.Repositories;

public class AppointmentsRepository : IAppointmentsRepository
{
    private readonly DapperContext _context;
    private readonly IResultsRepository _resultsRepository;
    private readonly IMapper _mapper;

    public AppointmentsRepository(DapperContext context, IResultsRepository resultsRepository, IMapper mapper)
    {
        _context = context;
        _resultsRepository = resultsRepository;
        _mapper = mapper;
    }

    public async Task<Guid> CreateAsync(Appointment appointment)
    {
        using var connection = _context.CreateConnection();

        const string sql = "SELECT create_appointment(" +
                           "@Id, @PatientId, @DoctorId, @ServiceId, @OfficeId, @Date, @Time, @Status, " +
                           "@ServiceName, @DoctorFirstName, @DoctorLastName, @DoctorMiddleName, " +
                           "@PatientFirstName, @PatientLastName, @PatientMiddleName)";

        var parameters = new DynamicParameters();
        parameters.Add("@Id", appointment.Id);
        parameters.Add("@PatientId", appointment.PatientId);
        parameters.Add("@DoctorId", appointment.DoctorId);
        parameters.Add("@ServiceId", appointment.ServiceId);
        parameters.Add("@OfficeId", appointment.OfficeId);
        parameters.Add("@Date", appointment.Date, DbType.Date);
        parameters.Add("@Time", appointment.Time, DbType.Time);
        parameters.Add("@Status", (short)appointment.Status); // TODO: aaa
        parameters.Add("@ServiceName", appointment.ServiceName);
        parameters.Add("@DoctorFirstName", appointment.DoctorFirstName);
        parameters.Add("@DoctorLastName", appointment.DoctorLastName);
        parameters.Add("@DoctorMiddleName", appointment.DoctorMiddleName);
        parameters.Add("@PatientFirstName", appointment.PatientFirstName);
        parameters.Add("@PatientLastName", appointment.PatientLastName);
        parameters.Add("@PatientMiddleName", appointment.PatientMiddleName);

        return await connection.ExecuteScalarAsync<Guid>(sql, parameters);
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

    public async Task<IEnumerable<AppointmentForDoctorDto>> GetForDoctorPaginatedAsync(Guid doctorId, int pageSize, int pageNumber, DateTime date)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT * FROM get_appointments_for_doctor_paginated(@doctor_id, @page_size, @page_number, @filter_date)";

        var parameters = new DynamicParameters();
        parameters.Add("doctor_id", doctorId);
        parameters.Add("page_size", pageSize);
        parameters.Add("page_number", pageNumber);
        parameters.Add("filter_date", date, DbType.Date);

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

    public async Task<int> RescheduleAsync(Guid id, DateTime newDate, TimeSpan newTime)
    {
        using var connection = _context.CreateConnection();
        const string sql = "SELECT reschedule_appointment(@appointment_id, @new_date, @new_time)";

        var parameters = new DynamicParameters();
        parameters.Add("appointment_id", id);
        parameters.Add("new_date", newDate, DbType.Date);
        parameters.Add("new_time", newTime, DbType.Time);

        return await connection.ExecuteAsync(sql, parameters);
    }

    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"SELECT * FROM ""Appointments"" WHERE ""Id"" = @id";
        var appointmentData = await connection.QuerySingleOrDefaultAsync<Appointment>(sql, new { id });

        if (appointmentData == null)
        {
            return null;
        }

        Func<ICollection<Result>> resultsLoader = () =>
            _resultsRepository.GetByAppointmentId(appointmentData.Id).ToList();

        var proxy = _mapper.Map<AppointmentProxy>(appointmentData, opts =>
            opts.Items["ResultsLoader"] = resultsLoader);

        return proxy;
    }

    public async Task<IEnumerable<OccupiedTimeSlotDto>> GetOccupiedTimeSlotsAsync(Guid doctorId, DateTime date)
    {
        using var connection = _context.CreateConnection();
        const string sql = "SELECT * FROM get_occupied_time_slots(@doctor_id, @filter_date)";

        var parameters = new DynamicParameters();
        parameters.Add("doctor_id", doctorId);
        parameters.Add("filter_date", date, DbType.Date);

        return await connection.QueryAsync<OccupiedTimeSlotDto>(sql, parameters);
    }

    public async Task UpdateServiceNameInAppointments(Guid serviceId, string newName)
    {
        using (var connection = _context.CreateConnection())
        {
            await connection.ExecuteAsync("SELECT update_service_name_in_appointments(@ServiceId, @NewName)", new { ServiceId = serviceId, NewName = newName });
        }
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsForDateAsync(DateTime date)
    {
        const string sql = "SELECT * FROM get_appointments_for_date(@Date)";

        using (var connection = _context.CreateConnection())
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Date", date, DbType.Date);

            var appointments = await connection.QueryAsync<Appointment>(sql, parameters);
            return appointments;
        }
    }
}
