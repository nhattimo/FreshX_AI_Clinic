using System.Text;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CozeSettings _cozeSettings;
        private readonly ILogger<AiController> _logger;

        public AiController(IHttpClientFactory httpClientFactory, IOptions<CozeSettings> cozeSettings, ILogger<AiController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _cozeSettings = cozeSettings.Value;
            _logger = logger;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateAiResponse([FromBody] string userInput)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_cozeSettings.ApiKey}");

            var requestContent = new StringContent(JsonConvert.SerializeObject(new { input = userInput }), Encoding.UTF8, "application/json");
            _logger.LogInformation("Sending request to Coze API at {ApiEndpoint} with content: {Content}", _cozeSettings.ApiEndpoint, requestContent);

            var response = await client.PostAsync(_cozeSettings.ApiEndpoint, requestContent);

            _logger.LogInformation("Received response from Coze API with status code: {StatusCode}", response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response content: {ResponseContent}", responseContent);

            if (response.IsSuccessStatusCode)
            {
                return Ok(responseContent);
            }

            return StatusCode((int)response.StatusCode, responseContent);
        }
    }

}