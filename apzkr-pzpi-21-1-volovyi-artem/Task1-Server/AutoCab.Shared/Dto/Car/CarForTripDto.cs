using AutoCab.Shared.Dto.Geolocation;

namespace AutoCab.Shared.Dto.Car;

public class CarForTripDto
{
    public Guid Id { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? LicencePlate { get; set; }
    public LocationDto? Location { get; set; }
    public int PassengerSeatsNum { get; set; }
    public decimal Price { get; set; }
}
