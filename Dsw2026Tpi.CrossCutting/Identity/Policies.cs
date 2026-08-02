namespace Dsw2026Tpi.CrossCutting.Identity;

public class Policies
{
    public const string AdminPolicy = "AdminPolicy";
    public const string PatientPolicy = "PatientPolicy";

    // politicas para el rate limiting
    public const string AdminRequestsPolicy = "AdminRequestsPolicy";
    public const string PatientRequestsPolicy = "PatientRequestsPolicy";
    public const string AppointmentRequestsPolicy = "AppointmentsRequestsPolicy";
}
