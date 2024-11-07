using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyApiProxy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;
        private readonly string _apiKey;

        public ProxyController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ThirdPartyApi:BaseUrl"];
            _apiKey = configuration["ThirdPartyApi:ApiKey"];
        }

        [HttpGet("user/{userName}/attended")]
        public async Task<IActionResult> GetAttendedEvents(string userName, [FromQuery] int p)
        {
            var requestUrl = $"{_baseApiUrl}/user/{userName}/attended?p={p}";

            // Prepare the request with API key header
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("x-api-key", _apiKey);

            // Forward the request to the third-party API
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return Content(responseContent, "application/json");
            }

            return StatusCode((int)response.StatusCode, "Failed to fetch data.");
        }
    }
}
