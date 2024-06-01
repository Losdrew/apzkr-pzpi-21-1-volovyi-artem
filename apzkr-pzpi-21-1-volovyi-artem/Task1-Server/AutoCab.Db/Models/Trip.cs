namespace AutoCab.Db.Models;

public class Trip : Entity
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public TripStatus Status { get; set; }
    public decimal Price { get; set; }

    public Guid StartAddressId { get; set; }
    public Address? StartAddress { get; set; }

    public Guid DestinationAddressId { get; set; }
    public Address? DestinationAddress { get; set; }

    public Guid CarId { get; set; }
    public Car? Car { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Service>? Services { get; set; }
}

public enum TripStatus
{
    Created,
    InProgress,
    WaitingForPassenger,
    Completed,
    Cancelled
}