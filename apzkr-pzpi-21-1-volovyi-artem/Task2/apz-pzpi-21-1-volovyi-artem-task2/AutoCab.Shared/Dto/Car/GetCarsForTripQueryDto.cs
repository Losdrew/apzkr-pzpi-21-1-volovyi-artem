using AutoCab.Shared.Dto.Address;

namespace AutoCab.Shared.Dto.Car;

public class GetCarsForTripQueryDto
{
    public AddressDto StartAddress { get; set; }
    public AddressDto DestinationAddress { get; set; }
}
