using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TiendaServicios.Api.CarritoCompra.Persistencia;

namespace TiendaServicios.Api.CarritoCompra.Aplicacion
{
    public class Eliminar
    {
        public class Ejecuta : IRequest<Unit>
        {
            public int CarritoSesionId { get; set; }
            public string ProductoSeleccionadoId { get; set; }
        }

        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(x => x.CarritoSesionId).NotEmpty();
                RuleFor(x => x.ProductoSeleccionadoId).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta, Unit>
        {
            private readonly CarritoContexto _contexto;

            public Manejador(CarritoContexto contexto)
            {
                _contexto = contexto;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var detalleItem = await _contexto.CarritoSesionDetalle
                    .FirstOrDefaultAsync(d => d.CarritoSesionId == request.CarritoSesionId && d.ProductoSeleccionado == request.ProductoSeleccionadoId, cancellationToken);

                if (detalleItem == null)
                {
                    throw new Exception("No se encontró el item del carrito a eliminar");
                }

                _contexto.CarritoSesionDetalle.Remove(detalleItem);
                var resultado = await _contexto.SaveChangesAsync(cancellationToken);

                if (resultado > 0)
                {
                    return Unit.Value;
                }

                throw new Exception("No se pudo eliminar el item del carrito");
            }
        }
    }
}