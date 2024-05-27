namespace AutoCab.Shared.Dto.Trip;

public class UpdateTripServicesCommandDto
{
    public Guid TripId { get; set; }
    public ICollection<Guid>? Services { get; set; }
}