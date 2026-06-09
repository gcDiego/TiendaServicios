using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using TiendaServicios.Api.CarritoCompra.RemoteInterface;
using TiendaServicios.Api.CarritoCompra.RemoteModel;

namespace TiendaServicios.Api.CarritoCompra.RemoteService
{
    public class ComicVineService : IComicVineService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ILogger<ComicVineService> _logger;
        private readonly string _apiKey;

        public ComicVineService(IHttpClientFactory httpClient, ILogger<ComicVineService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["ComicVine:ApiKey"];
        }

        public async Task<(bool resultado, ComicVineVolumeResponse Comic, string ErrorMessage)> GetComic(string comicId)
        {
            try
            {
                var cliente = _httpClient.CreateClient("ComicVine");
                // Comic Vine requires a custom User-Agent to avoid 403 Forbidden errors
                cliente.DefaultRequestHeaders.UserAgent.ParseAdd("TiendaServicios/1.0");

                // Comic Vine uses 'format=json' and requires the API key in the URL
                var url = $"/api/volume/4050-{comicId}?api_key={_apiKey}&format=json";
                
                _logger.LogInformation($"Requesting Comic Vine URL: {url}");
                var response = await cliente.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var contenido = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var resultado = JsonSerializer.Deserialize<ComicVineVolumeResponse>(contenido, options);
                    return (true, resultado, null);
                }

                _logger.LogWarning($"Comic Vine API Error: {response.StatusCode} - {response.ReasonPhrase}");
                return (false, null, response.ReasonPhrase);
            }
            catch (Exception e)
            {
                _logger?.LogError(e.ToString());
                return (false, null, e.Message);
            }
        }
    }
}