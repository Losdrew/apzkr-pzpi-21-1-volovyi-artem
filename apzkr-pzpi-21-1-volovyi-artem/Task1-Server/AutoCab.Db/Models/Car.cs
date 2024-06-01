using System.ComponentModel.DataAnnotations.Schema;
using Point = NetTopologySuite.Geometries.Point;

namespace AutoCab.Db.Models;

public class Car : Entity
{
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? LicencePlate { get; set; }
    public CarStatus Status { get; set; }
    public int PassengerSeatsNum { get; set; }
    public decimal Temperature { get; set; }
    public decimal Fuel { get; set; }
    public bool IsDoorOpen { get; set; }
    public string? DeviceId { get; set; }

    [Column(TypeName="geometry (point)")]
    public Point? Location { get; set; }
}

public enum CarStatus
{
    Inactive,
    Idle,
    EnRoute,
    WaitingForPassenger,
    OnTrip,
    Maintenance,
    Danger
}