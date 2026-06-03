using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.CarritoCompra.Modelo;
using TiendaServicios.Api.CarritoCompra.Persistencia;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace TiendaServicios.Api.CarritoCompra.Aplicacion
{
    public class Agregar
    {
        public class Ejecuta : IRequest<Unit>
        {
            public int CarritoSesionId { get; set; }
            public string ProductoSeleccionado { get; set; }
        }

        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(x => x.CarritoSesionId).NotEmpty();
                RuleFor(x => x.ProductoSeleccionado).NotEmpty();
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
                // 1. Verificar que el carrito de compras existe
                var carritoSesion = await _contexto.CarritoSesion.FirstOrDefaultAsync(x => x.CarritoSesionId == request.CarritoSesionId);
                if (carritoSesion == null)
                {
                    throw new Exception("El carrito de compras no existe");
                }

                // 2. Verificar que el libro que se va a agregar existe en el carrito
                var detalle = await _contexto.CarritoSesionDetalle.FirstOrDefaultAsync(x => x.CarritoSesionId == request.CarritoSesionId && x.ProductoSeleccionado == request.ProductoSeleccionado);
                
                if (detalle != null)
                {
                    // Si el libro ya está en el carrito, no hacemos nada o podríamos aumentar la cantidad en un futuro
                    return Unit.Value;
                }

                // 3. Crear el nuevo detalle del carrito
                var nuevoDetalle = new CarritoSesionDetalle
                {
                    FechaCreacion = DateTime.Now,
                    CarritoSesionId = request.CarritoSesionId,
                    ProductoSeleccionado = request.ProductoSeleccionado
                };

                _contexto.CarritoSesionDetalle.Add(nuevoDetalle);
                var resultado = await _contexto.SaveChangesAsync();

                if (resultado > 0)
                {
                    return Unit.Value;
                }

                throw new Exception("No se pudo agregar el producto al carrito de compras");
            }
        }
    }
}