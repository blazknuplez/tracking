namespace Tracking.Ef.Entities;

public class Account
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}