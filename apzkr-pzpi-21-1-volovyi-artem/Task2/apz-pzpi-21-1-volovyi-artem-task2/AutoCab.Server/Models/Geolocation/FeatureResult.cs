namespace AutoCab.Server.Models.Geolocation;

public class FeatureResult
{
    public string Type { get; set; }
    public List<Feature> Features { get; set; }
    public string Attribution { get; set; }
}

public class Feature
{
    public string Type { get; set; }
    public Geometry Geometry { get; set; }
}

public class Geometry
{
    public List<double> Coordinates { get; set; }
}
