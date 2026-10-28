namespace Dsw2026Tpi.Application.Dtos;

public record SlotModel
{
    public sealed record Request();

    public sealed record Response(
        Guid SlotId,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime);
}