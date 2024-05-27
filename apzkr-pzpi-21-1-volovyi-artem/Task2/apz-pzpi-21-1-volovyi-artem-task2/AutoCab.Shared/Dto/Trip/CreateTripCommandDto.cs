using AutoCab.Shared.Dto.Address;

namespace AutoCab.Shared.Dto.Trip;

public class CreateTripCommandDto
{
    public decimal Price { get; set; }
    public AddressDto? StartAddress { get; set; }
    public AddressDto? DestinationAddress { get; set; }
    public Guid CarId { get; set; }
}