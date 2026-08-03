namespace Dsw2026Tpi.Domain.Entities;

public sealed class Doctor : EntityBase
{
    public string Name { get; private set; }
    public string LicenseNumber { get; private set; }
    public Guid SpecialtyId { get; private set; }
    public Specialty Specialty { get; private set; }

    public ICollection<AvailabilityRule> AvailabilityRules { get; private set; } = [];

    public bool IsActive { get; private set; } = true;

    #region Constructor for EF
#pragma warning disable CS8618
    private Doctor()
    {
    }
#pragma warning restore CS8618
    #endregion

    public void Deactivate()
    {
        IsActive = false;
        UpdateTimestamp();
    }
    public Doctor(string name, string licenseNumber, Specialty specialty, Guid? id = null) : base(id)
    {
        Name = name;
        LicenseNumber = licenseNumber;
        Specialty = specialty;
    }

    public void UpdateDoctor(string name, string licenseNumber, Guid specialtyId)
    {
        Name = name;
        LicenseNumber = licenseNumber;
        SpecialtyId = specialtyId;
        UpdateTimestamp();
    }
    public void UpdateDoctorAvailabilities(ICollection<AvailabilityRule> availabilities)
    {
        AvailabilityRules.Clear();
        foreach(var availabilityRule in availabilities)
            AvailabilityRules.Add(availabilityRule);
    }

}
