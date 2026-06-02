using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.Libro.Modelo;
using TiendaServicios.Api.Libro.Persistencia;

namespace TiendaServicios.Api.Libro.Aplicacion
{
    public class Nuevo
    {
        public class Ejecuta : IRequest<Unit> { 
            public  string Titulo { get; set; }
            public  DateTime? FechaPublicacion { get; set; }
            public  Guid? AutorLibro { get; set; }

        }

        public class EjecutaValidacion : AbstractValidator<Ejecuta> {

            public EjecutaValidacion() {
                RuleFor(x => x.Titulo).NotEmpty();
                RuleFor(x => x.FechaPublicacion).NotEmpty();
                RuleFor(x => x.AutorLibro).NotEmpty();
            }
        }


        public class Manejador : IRequestHandler<Ejecuta, Unit>
        {
            private readonly ContextoLibreria _contexto;

            public Manejador(ContextoLibreria contexto) {
                _contexto = contexto;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                // Convertir la fecha a UTC si tiene un valor, para asegurar compatibilidad con bases de datos
                DateTime? fechaPublicacionUtc = null;
                if (request.FechaPublicacion.HasValue)
                {
                    fechaPublicacionUtc = DateTime.SpecifyKind(request.FechaPublicacion.Value, DateTimeKind.Utc);
                }

                var libro = new LibreriaMaterial
                {
                    Titulo = request.Titulo,
                    FechaPublicacion = fechaPublicacionUtc,
                    AutorLibro = request.AutorLibro
                };

                _contexto.LibreriaMaterial.Add(libro);

                var value = await _contexto.SaveChangesAsync();

                if (value > 0) {
                    return Unit.Value;
                }

                
                throw new Exception("No se pudo guardar el libro");
                
            }
        }

    }
}