using AutoCab.Shared.Dto.Geolocation;
using AutoCab.Shared.Helpers;

namespace AutoCab.Shared.Dto.Car;

public class UpdateCarCommandDto
{
    public string? DeviceId { get; set; }
    public CarStatus Status { get; set; }
    public decimal Temperature { get; set; }
    public decimal Fuel { get; set; }
    public LocationDto? Location { get; set; }
}