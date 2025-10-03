namespace Appointments.Domain.Dtos;

public record RescheduleAppointmentDto(
    DateTime NewDate,
    TimeSpan NewTime
);
