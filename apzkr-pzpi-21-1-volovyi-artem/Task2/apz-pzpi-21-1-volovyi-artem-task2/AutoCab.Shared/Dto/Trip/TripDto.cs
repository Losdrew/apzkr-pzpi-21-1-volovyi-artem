using AutoCab.Shared.Dto.Address;
using AutoCab.Shared.Dto.Service;

namespace AutoCab.Shared.Dto.Trip;

public class TripDto
{
    public DateTime StartDateTime { get; set; }
    public decimal Price { get; set; }
    public AddressDto? StartAddress { get; set; }
    public AddressDto? DestinationAddress { get; set; }
    public Guid CarId { get; set; }
    public ICollection<ServiceInfoDto>? Services { get; set; }
}