using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace valida_conta_instagram_poc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstagramCheckController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Injetamos o IHttpClientFactory no construtor
        public InstagramCheckController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> CheckProfile(string username)
        {
            const string userAgent = "Instagram 361.0.0.0.84 Android (28/9; 480dpi; 1080x1920; samsung; SM-G930F; herolte; samsungexynos8890; en_US; 673256705)";
            var url = $"https://i.instagram.com/api/v1/users/web_profile_info/?username={username}";

            try
            {
                // Criamos um cliente HTTP a partir da factory para cada requisição
                var client = _httpClientFactory.CreateClient();

                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.TryAddWithoutValidation("User-Agent", userAgent);

                var response = await client.SendAsync(requestMessage);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new { Error = content });
                }

                return Ok(JsonDocument.Parse(content));
            }
            catch (Exception ex)
            {
                // Este log de erro agora DEVE aparecer no Coolify se algo falhar
                Console.WriteLine($"ERRO CRÍTICO: {ex.ToString()}");
                return StatusCode(500, new { Error = "Ocorreu um erro interno crítico.", Details = ex.Message });
            }
        }
    }
}