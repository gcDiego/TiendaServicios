using System;

namespace TiendaServicios.Api.Comic.Modelo
{
    public class Comic
    {
        public Guid? ComicId { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        // Podríamos añadir más campos como 'Ilustrador', 'Genero', etc. en el futuro.
    }
}