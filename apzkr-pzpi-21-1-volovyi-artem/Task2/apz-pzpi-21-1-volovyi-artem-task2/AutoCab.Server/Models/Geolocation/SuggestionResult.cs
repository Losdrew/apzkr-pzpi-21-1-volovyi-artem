namespace AutoCab.Server.Models.Geolocation;

public class SuggestionResult
{
    public List<Suggestion> Suggestions { get; set; }
    public string Attribution { get; set; }
}

public class Suggestion
{
    public string Name { get; set; }
    public string Mapbox_Id { get; set; }
    public string FeatureType { get; set; }
    public string PlaceFormatted { get; set; }
    public object Context { get; set; }
    public string Language { get; set; }
    public string Maki { get; set; }
    public List<string> PoiCategory { get; set; }
    public List<string> PoiCategoryIds { get; set; }
}
