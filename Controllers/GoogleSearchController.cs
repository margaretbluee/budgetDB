using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class GoogleSearchController : ControllerBase
{
    private readonly GoogleSearchService _googleSearch;

    public GoogleSearchController(GoogleSearchService googleSearch)
    {
        _googleSearch = googleSearch;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var results = await _googleSearch.SearchAsync(query);
        return Ok(results);
    }
}
