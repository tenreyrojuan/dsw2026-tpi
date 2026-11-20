namespace Dsw2026Tpi.Application.Dtos;

public record StatsModel
{
    public record Response(int ActiveDoctors, int ActiveSpecialties, int BookedAppointmentsToday);
}