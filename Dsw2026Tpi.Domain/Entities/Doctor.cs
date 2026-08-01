namespace Dsw2026Tpi.Domain.Entities;

public sealed class Doctor : EntityBase
{
    public string Name { get; private set; }
    public string LicenseNumber { get; private set; }
    public Guid? SpecialityId { get; private set; }
    public Speciality? Speciality { get; private set; }

    public ICollection<Availability> Availabilities { get; private set; } = [];

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
    public Doctor(string name, string licenseNumber, Speciality speciality, Guid? id = null) : base(id)
    {
        Name = name;
        LicenseNumber = licenseNumber;
        Speciality = speciality;
    }

    public void UpdateDoctor(string name, string licenseNumber, Guid? specialityId)
    {
        Name = name;
        LicenseNumber = licenseNumber;
        SpecialityId = specialityId;
        UpdateTimestamp();
    }
    public void UpdateDoctorAvailabilities(ICollection<Availability> availabilities)
    {
        Availabilities.Clear();
        foreach(var availability in availabilities)
            Availabilities.Add(availability);
    }

}
