using System.Data;

namespace Dsw2026Tpi.Domain.Entities;

public abstract class EntityBase(Guid? id = null)
{
    public Guid Id { get; init; } = id ?? Guid.NewGuid();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public bool IsActive { get; private set; } = true;

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
