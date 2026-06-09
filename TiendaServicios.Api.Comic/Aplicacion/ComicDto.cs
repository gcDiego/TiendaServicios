using System;

namespace TiendaServicios.Api.Comic.Aplicacion
{
    public class ComicDto
    {
        public Guid? ComicId { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public DateTime? FechaPublicacion { get; set; }
    }
}