using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Application.Interfaces;

public interface ISlotService
{
    Task<Pagination<SlotModel.Response>> GetTodaysSlots(int pageSize, int pageIndex);
}