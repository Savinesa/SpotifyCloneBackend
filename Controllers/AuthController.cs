using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SpotifyClone.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region tokenApiForInternalTesting
        [HttpGet("token")]
        public async Task<IActionResult> GetAccessToken()
        {
            var clientId = "91a08a67dac54bd4aaa128b7c88b6d2c";
            var clientSecret = "bc1806d598bc45d2862f3068fd5bfc57";
            var tokenUrl = "https://accounts.spotify.com/api/token";
            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
            var requestBody = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
            {
                Content = new FormUrlEncodedContent(requestBody)
            };

            var response = await client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Ok(responseBody);
            }
            return BadRequest("Failed to authenticate with Spotify.");
        }
        #endregion

        #region InitialRedirectToSpotify
        [HttpGet("redirect")]
        public IActionResult RedirectToSpotify()
        {
            var clientId = "91a08a67dac54bd4aaa128b7c88b6d2c";
            var redirectUri = "http://localhost:5111/api/auth/callback";
            var scope = "user-top-read";

            var spotifyAuthUrl = $"https://accounts.spotify.com/authorize?client_id={clientId}&response_type=code&redirect_uri={redirectUri}&scope={scope}";
            return Redirect(spotifyAuthUrl);
        }
        #endregion

        #region CallBackToFrontendWithAccessToken
        [HttpGet("callback")]
        public async Task<IActionResult> HandleSpotifyCallback(string code)
        {

            var clientId = "91a08a67dac54bd4aaa128b7c88b6d2c";
            var clientSecret = "bc1806d598bc45d2862f3068fd5bfc57";
            var redirectUri = "http://localhost:5111/api/auth/callback";
            var frontendRedirectUri = "http://localhost:5173";
            var tokenUrl = "https://accounts.spotify.com/api/token";

            using var client = new HttpClient();

            var requestBody = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", redirectUri },
                { "client_id", clientId },
                { "client_secret", clientSecret }
            };
            var requestContent = new FormUrlEncodedContent(requestBody);
            var response = await client.PostAsync(tokenUrl, requestContent);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var tokenData = System.Text.Json.JsonSerializer.Deserialize<TokenResponse>(responseBody);
                if (tokenData != null && !string.IsNullOrEmpty(tokenData.access_token))

                return Redirect($"{frontendRedirectUri}/?access_token={tokenData.access_token}");
            }

            return BadRequest($"Failed to exchange token. {responseBody}");

        }
        #endregion

      

    }
}