namespace AutoCab.Shared.Dto.Car;

public class CreateCarCommandDto
{
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? LicencePlate { get; set; }
    public int PassengerSeatsNum { get; set; }
    public string? DeviceId { get; set; }
}