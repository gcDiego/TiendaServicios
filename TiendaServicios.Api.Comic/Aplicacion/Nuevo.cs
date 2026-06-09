using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.Comic.Persistencia;

namespace TiendaServicios.Api.Comic.Aplicacion
{
    public class Nuevo
    {
        public class Ejecuta : IRequest<Unit>
        {
            public string Titulo { get; set; }
            public string Autor { get; set; }
            public DateTime? FechaPublicacion { get; set; }
        }

        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(x => x.Titulo).NotEmpty();
                RuleFor(x => x.Autor).NotEmpty();
                RuleFor(x => x.FechaPublicacion).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta, Unit>
        {
            private readonly ContextoComic _contexto;

            public Manejador(ContextoComic contexto)
            {
                _contexto = contexto;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                DateTime? fechaPublicacionUtc = null;
                if (request.FechaPublicacion.HasValue)
                {
                    fechaPublicacionUtc = DateTime.SpecifyKind(request.FechaPublicacion.Value, DateTimeKind.Utc);
                }

                var comic = new Modelo.Comic
                {
                    ComicId = Guid.NewGuid(),
                    Titulo = request.Titulo,
                    Autor = request.Autor,
                    FechaPublicacion = fechaPublicacionUtc
                };

                _contexto.Comics.Add(comic);

                var value = await _contexto.SaveChangesAsync(cancellationToken);

                if (value > 0)
                {
                    return Unit.Value;
                }

                throw new Exception("No se pudo guardar el comic");
            }
        }
    }
}