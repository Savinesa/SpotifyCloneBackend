using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SpotifyClone.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserDataController : ControllerBase
    {
        #region GetTopTracksByUserPref
        [HttpGet("top-tracks")]
        public async Task<IActionResult> GetUserTopTracks([FromHeader] string accessToken)
        {
            var url = "https://api.spotify.com/v1/me/top/tracks?time_range=short_term";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Ok(responseBody); // Return the top tracks as JSON
            }

            return BadRequest($"Failed to fetch top tracks. {responseBody}");
        }
        #endregion

        #region GetTopArtistsByUserPref
        [HttpGet("top-artists")]
        public async Task<IActionResult> GetUserTopArtists([FromHeader] string accessToken)
        {
            var url = "https://api.spotify.com/v1/me/top/artists";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Ok(responseBody); // Return the top artists as JSON
            }

            return BadRequest($"Failed to fetch top artists. {responseBody}");
        }
        #endregion

        #region GetAlbumsByArtistId
        [HttpGet("{artistId}/albums")]
        public async Task<IActionResult> GetArtistAlbums([FromHeader] string accessToken, string artistId)
        {
            var url = $"https://api.spotify.com/v1/artists/{artistId}/albums";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Ok(responseBody); // Return albums as JSON
            }

            return BadRequest($"Failed to fetch albums for artist {artistId}. {responseBody}");
        }
        #endregion

        #region GetAllAlbumsForInternalTesting
        [HttpGet("albums")]
        public async Task<IActionResult> GetAllAlbums([FromHeader] string accessToken)
        {
            var url = $"https://api.spotify.com/v1/albums/?ids=382ObEPsp2rxGrnsizN5TX,1A2GTWGtFfWp7KSQTwWOyo,2noRn2Aes5aoNVsU6iWThc";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Ok(responseBody); // Return albums as JSON
            }

            return BadRequest($"Failed to fetch albums {responseBody}");
        }
        #endregion

        #region Search
        [HttpGet("search")]
        public async Task<IActionResult> SearchSpotify([FromHeader] string accessToken, [FromQuery] string query, [FromQuery] string type = "track", [FromQuery] int limit = 10)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Query parameter is required.");
            }

            var url = $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(query)}&type={type}&limit={limit}";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Ok(responseBody); 
            }

            return BadRequest($"Failed to search Spotify. {responseBody}");
        }
        #endregion

    }
}
