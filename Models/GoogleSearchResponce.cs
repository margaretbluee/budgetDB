using Newtonsoft.Json;

public class GoogleSearchResponse
{
    [JsonProperty("items")]
    public List<GoogleSearchItem> Items { get; set; }
}