using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Data;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;

namespace Dsw2026Tpi.Application.Services;

public sealed class SlotService : ISlotService
{
    private readonly IPersistence  _persistence;

    public SlotService(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public async Task<Pagination<SlotModel.Response>> GetTodaysSlots(int pageSize, int pageIndex)
    {
        var date = DateOnly.FromDateTime(DateTime.Now);


        var slots = await _persistence.Paginate<AvailabilitySlot, DateOnly>(pageSize, pageIndex, x => x.Date == date, x => x.Date);

        return slots.Map(s => new SlotModel.Response(s.Id,s.Date,s.StartingTime,s.EndingTime));
    }
}