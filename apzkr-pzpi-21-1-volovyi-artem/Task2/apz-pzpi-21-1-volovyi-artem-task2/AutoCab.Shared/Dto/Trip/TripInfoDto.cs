using AutoCab.Shared.Helpers;

namespace AutoCab.Shared.Dto.Trip;

public class TripInfoDto : TripDto
{
    public Guid Id { get; set; }    
    public Guid UserId { get; set; }    
    public TripStatus TripStatus { get; set; }
}