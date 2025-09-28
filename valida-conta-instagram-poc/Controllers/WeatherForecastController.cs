using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace valida_conta_instagram_poc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstagramCheckController : ControllerBase
    {
        private static readonly HttpClient client = new HttpClient();

        [HttpGet("{username}")]
        public async Task<IActionResult> CheckProfile(string username)
        {
            const string userAgent = "Instagram 361.0.0.0.84 Android (28/9; 480dpi; 1080x1920; samsung; SM-G930F; herolte; samsungexynos8890; en_US; 673256705)";
            var url = $"https://i.instagram.com/api/v1/users/web_profile_info/?username={username}";

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", userAgent);

            try
            {
                var response = await client.GetAsync(url);

                // Se a resposta da API já for um erro (como 404 Not Found), consideramos que não existe.
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound(new { status = "nao_existe", motivo = $"API do Instagram retornou o erro {(int)response.StatusCode}." });
                }

                var content = await response.Content.ReadAsStringAsync();

                // --- NOVA VALIDAÇÃO DE CONTEÚDO ---
                try
                {
                    using JsonDocument doc = JsonDocument.Parse(content);
                    JsonElement root = doc.RootElement;

                    // A resposta esperada tem a estrutura: { "data": { "user": { "username": "..." } } }
                    if (root.TryGetProperty("data", out JsonElement dataElement) &&
                        dataElement.TryGetProperty("user", out JsonElement userElement) &&
                        userElement.TryGetProperty("username", out JsonElement usernameElement))
                    {
                        // Se encontramos o campo username, a conta existe.
                        string foundUsername = usernameElement.GetString() ?? string.Empty;
                        return Ok(new { status = "existe", username = foundUsername });
                    }
                    else
                    {
                        // O JSON é válido, mas não tem a estrutura ou o campo que esperamos.
                        return NotFound(new { status = "nao_existe", motivo = "A resposta JSON não continha o campo 'username' esperado." });
                    }
                }
                catch (JsonException)
                {
                    // Se o conteúdo da resposta não for um JSON válido, consideramos que não existe.
                    return NotFound(new { status = "nao_existe", motivo = "A resposta do Instagram não foi um JSON válido." });
                }
                // --- FIM DA NOVA VALIDAÇÃO ---
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "erro", motivo = "Ocorreu um erro interno na API.", detalhes = ex.Message });
            }
        }
    }
}