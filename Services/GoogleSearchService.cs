using Newtonsoft.Json;

public class GoogleSearchService
{
    private readonly string _apiKey = "asfsa";  
    private readonly string _cx = "sdgsdfg";  
    private readonly HttpClient _httpClient;
 
    public GoogleSearchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<GoogleSearchItem>> SearchAsync(string query)
    {
        var encodedQuery = Uri.EscapeDataString(query);
        var requestUri = $"https://www.googleapis.com/customsearch/v1?q={encodedQuery}&key={_apiKey}&cx={_cx}";

        var response = await _httpClient.GetAsync(requestUri);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Google Search failed: {response.StatusCode} - {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GoogleSearchResponse>(content);

        return result?.Items ?? new List<GoogleSearchItem>();
    }
}
