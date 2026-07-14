using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities;
//Estado de Turno
public enum TimeSlotState
{
    Available = 1,
    Booked,
    Blocked,
}
