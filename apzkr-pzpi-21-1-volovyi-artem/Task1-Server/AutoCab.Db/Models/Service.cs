namespace AutoCab.Db.Models;

public class Service : Entity
{
    public string? Name {  get; set; }
    public string? Command { get; set; }

    public ICollection<Trip>? Trips { get; set; }
}
