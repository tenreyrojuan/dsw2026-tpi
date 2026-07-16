using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities;
//Estado de Cita
public enum AppointmentState
{
    Confirmed = 1,
    Canceled,
    Completed,
}
