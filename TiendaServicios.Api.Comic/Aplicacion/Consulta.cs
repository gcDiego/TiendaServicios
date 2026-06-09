using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.Comic.Persistencia;

namespace TiendaServicios.Api.Comic.Aplicacion
{
    public class Consulta
    {
        public class ListaComics : IRequest<List<ComicDto>> { }

        public class Manejador : IRequestHandler<ListaComics, List<ComicDto>>
        {
            private readonly ContextoComic _contexto;
            private readonly IMapper _mapper;

            public Manejador(ContextoComic contexto, IMapper mapper)
            {
                _contexto = contexto;
                _mapper = mapper;
            }

            public async Task<List<ComicDto>> Handle(ListaComics request, CancellationToken cancellationToken)
            {
                var comics = await _contexto.Comics.ToListAsync(cancellationToken);
                var comicsDto = _mapper.Map<List<Modelo.Comic>, List<ComicDto>>(comics);
                return comicsDto;
            }
        }
    }
}