using AutoCab.Shared.Dto.Geolocation;
using AutoCab.Shared.Helpers;

namespace AutoCab.Shared.Dto.Car;

public class CarDto
{
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? LicencePlate { get; set; }
    public CarStatus Status { get; set; }
    public LocationDto? Location { get; set; }
    public int PassengerSeatsNum { get; set; }
    public decimal Temperature { get; set; }
    public decimal Fuel { get; set; }
    public string? DeviceId { get; set; }
}