using AutoMapper;
using TiendaServicios.Api.Comic.Modelo;

namespace TiendaServicios.Api.Comic.Aplicacion
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Modelo.Comic, ComicDto>();
        }
    }
}