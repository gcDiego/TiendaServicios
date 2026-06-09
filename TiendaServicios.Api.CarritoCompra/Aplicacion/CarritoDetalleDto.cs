using System;

namespace TiendaServicios.Api.CarritoCompra.Aplicacion
{
    public class CarritoDetalleDto
    {
        public string ProductoId { get; set; }
        public string TituloProducto { get; set; }
        public string AutorProducto { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public double PrecioUnitario { get; set; }
        public string TipoProducto { get; set; } // "Libro" o "Comic"
        public string ImagenUrl { get; set; } // Para la imagen del comic
    }
}