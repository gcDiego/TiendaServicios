using System.Text.Json.Serialization;

namespace TiendaServicios.Api.CarritoCompra.RemoteModel
{
    public class ComicVineVolumeResponse
    {
        [JsonPropertyName("results")]
        public VolumeDetails Results { get; set; }
    }

    public class VolumeDetails
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("deck")]
        public string Deck { get; set; }

        [JsonPropertyName("image")]
        public ImageDetails Image { get; set; }
        
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class ImageDetails
    {
        [JsonPropertyName("original_url")]
        public string OriginalUrl { get; set; }
    }
}