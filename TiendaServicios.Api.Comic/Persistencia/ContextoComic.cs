using Microsoft.EntityFrameworkCore;
using TiendaServicios.Api.Comic.Modelo;

namespace TiendaServicios.Api.Comic.Persistencia
{
    public class ContextoComic : DbContext
    {
        public ContextoComic(DbContextOptions<ContextoComic> options) : base(options)
        {
        }

        public DbSet<Modelo.Comic> Comics { get; set; }
    }
}