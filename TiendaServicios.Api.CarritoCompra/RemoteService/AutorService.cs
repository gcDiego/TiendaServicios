using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TiendaServicios.Api.CarritoCompra.RemoteInterface;
using TiendaServicios.Api.CarritoCompra.RemoteModel;

namespace TiendaServicios.Api.CarritoCompra.RemoteService
{
    public class AutorService : IAutorService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ILogger<AutorService> _logger;

        public AutorService(IHttpClientFactory httpClient, ILogger<AutorService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<(bool resultado, AutorRemote Autor, string ErrorMessage)> GetAutor(Guid AutorId)
        {
            try
            {
                var cliente = _httpClient.CreateClient("Autores");
                var response = await cliente.GetAsync($"/api/Autor/{AutorId}");
                if (response.IsSuccessStatusCode)
                {
                    var contenido = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var resultado = JsonSerializer.Deserialize<AutorRemote>(contenido, options);
                    return (true, resultado, null);
                }

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