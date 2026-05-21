using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.CarritoCompra.Modelo;
using TiendaServicios.Api.CarritoCompra.Persistencia;
using TiendaServicios.Api.CarritoCompra.RemoteInterface;

namespace TiendaServicios.Api.CarritoCompra.Aplicacion
{
    public class Consulta
    {
        public class Ejecuta : IRequest<CarritoDto> {
            public int CarritoSesionId { get; set; }
        }

        public class Manejador : IRequestHandler<Ejecuta, CarritoDto>
        {
            private readonly CarritoContexto _contexto;
            private readonly ILibrosService _libroService;
            private readonly IAutorService _autorService;
            private readonly ILogger<Manejador> _logger;
            private readonly Random _random; // Random instance to mock prices

            public Manejador(CarritoContexto contexto, ILibrosService libroService, IAutorService autorService, ILogger<Manejador> logger) {
                _contexto = contexto;
                _libroService = libroService;
                _autorService = autorService;
                _logger = logger;
                _random = new Random();
            }

            public async  Task<CarritoDto> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var carritoSesion = await _contexto.CarritoSesion.FirstOrDefaultAsync(x => x.CarritoSesionId == request.CarritoSesionId);
                if (carritoSesion == null) {
                    _logger.LogWarning($"No se encontró la sesión de carrito con id {request.CarritoSesionId}");
                    return null;
                }
                var carritoSesionDetalle = await _contexto.CarritoSesionDetalle.Where(x => x.CarritoSesionId == request.CarritoSesionId).ToListAsync();
                if (!carritoSesionDetalle.Any()) {
                    _logger.LogWarning($"No se encontraron detalles para la sesión de carrito con id {request.CarritoSesionId}");
                }

                var listaCarritoDto = new  List<CarritoDetalleDto>();
                double totalCarrito = 0; // Variable para sumar el total

                foreach (var libro in carritoSesionDetalle) {
                    _logger.LogInformation($"Procesando libro: {libro.ProductoSeleccionado}");
                    var response = await _libroService.GetLibro(new Guid(libro.ProductoSeleccionado));
                    if (response.resultado) {
                        var objetoLibro = response.Libro;
                        _logger.LogInformation($"Libro obtenido: {objetoLibro.Titulo}, Autor ID: {objetoLibro.AutorLibro}");
                        
                        string nombreAutor = "Autor no encontrado"; // Valor por defecto

                        if (objetoLibro.AutorLibro.HasValue) {
                            var autorResponse = await _autorService.GetAutor(objetoLibro.AutorLibro.Value);
                            if(autorResponse.resultado && autorResponse.Autor != null)
                            {
                                var objetoAutor = autorResponse.Autor;
                                nombreAutor = $"{objetoAutor.Nombre} {objetoAutor.Apellido}";
                                _logger.LogInformation($"Autor obtenido: {nombreAutor}");
                            } else {
                                _logger.LogWarning($"No se pudo obtener el autor con id {objetoLibro.AutorLibro.Value}. Error: {autorResponse.ErrorMessage}");
                            }
                        } else {
                            _logger.LogWarning($"El libro con id {libro.ProductoSeleccionado} no tiene un autor asociado.");
                        }

                        // Generar precio aleatorio entre 10 y 100
                        double precioMock = Math.Round((_random.NextDouble() * 90) + 10, 2);
                        totalCarrito += precioMock;

                        var carritoDetalle = new CarritoDetalleDto
                        {
                            TituloLibro = objetoLibro.Titulo,
                            FechaPublicacion = objetoLibro.FechaPublicacion,
                            LibroId = objetoLibro.LibreriaMaterialId,
                            AutorLibro = nombreAutor,
                            PrecioUnitario = precioMock
                        };
                        listaCarritoDto.Add(carritoDetalle);

                    } else {
                        _logger.LogWarning($"No se pudo obtener el libro con id {libro.ProductoSeleccionado}. Error: {response.ErrorMessage}");
                    }
                }

                var carritoSesionDto = new CarritoDto
                {
                    CarritoId = carritoSesion.CarritoSesionId,
                    FechaCreacionSesion = carritoSesion.FechaCreacion,
                    ListaProductos = listaCarritoDto,
                    Total = Math.Round(totalCarrito, 2)
                };

                return carritoSesionDto;
            }
        }

    }
}