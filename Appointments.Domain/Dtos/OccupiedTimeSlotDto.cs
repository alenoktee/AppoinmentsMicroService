namespace Appointments.Domain.Dtos;

public record OccupiedTimeSlotDto(
    TimeSpan StartTime,
    TimeSpan EndTime
);
