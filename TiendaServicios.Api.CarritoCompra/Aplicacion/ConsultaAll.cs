using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.CarritoCompra.Persistencia;

namespace TiendaServicios.Api.CarritoCompra.Aplicacion
{
    public class ConsultaAll
    {
        public class Ejecuta : IRequest<List<CarritoDto>> { }

        public class Manejador : IRequestHandler<Ejecuta, List<CarritoDto>>
        {
            private readonly CarritoContexto _contexto;

            public Manejador(CarritoContexto contexto)
            {
                _contexto = contexto;
            }

            public async Task<List<CarritoDto>> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var carritos = await _contexto.CarritoSesion.Select(cs => new CarritoDto
                {
                    CarritoId = cs.CarritoSesionId,
                    FechaCreacionSesion = cs.FechaCreacion
                }).ToListAsync(cancellationToken);

                return carritos;
            }
        }
    }
}