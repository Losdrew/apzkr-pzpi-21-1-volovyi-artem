using AutoCab.Shared.Helpers;

namespace AutoCab.Shared.Dto.Car;

public class SetCarStatusCommandDto
{
    public Guid Id { get; set; }
    public CarStatus NewStatus { get; set; }
}