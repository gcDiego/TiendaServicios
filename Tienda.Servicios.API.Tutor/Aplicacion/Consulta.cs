using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TiendaServicios.Api.Autor.Persistencia;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TiendaServicios.Api.Autor.Modelo; //no estaba en la clase originalx

namespace TiendaServicios.Api.Autor.Aplicacion
{
    public class Consulta
    {
        public class ListaAutor : IRequest<List<AutorDto>>
        {
        }

        public class Manejador : IRequestHandler<ListaAutor, List<AutorDto>>
        {
            private readonly ContextoAutor _contexto;
            private readonly IMapper _mapper;

            public Manejador(ContextoAutor contexto, IMapper mapper)
            {
                _contexto = contexto;
                _mapper = mapper;
            }
            public async Task<List<AutorDto>> Handle(ListaAutor request, CancellationToken cancellationToken)
            {

                var autores = await _contexto.AutorLibro.ToListAsync();
                var autoresDto = _mapper.Map<List<AutorLibro>, List<AutorDto>>(autores);

                return autoresDto;
            }
        }
    }
}