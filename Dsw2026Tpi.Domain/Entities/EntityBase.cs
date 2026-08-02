namespace Dsw2026Tpi.Domain.Entities;

public abstract class EntityBase(Guid? id = null)
{
    public Guid Id { get; init; } = id ?? Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public bool Deleted { get; private set; } = false;

    public void SetDelete()
    {
        Deleted = true;
        UpdateTimestamp();
    }

    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.Now;
    }
}
