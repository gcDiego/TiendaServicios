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
            private readonly IComicVineService _comicVineService;
            private readonly ILogger<Manejador> _logger;
            private readonly Random _random;

            public Manejador(CarritoContexto contexto, ILibrosService libroService, IAutorService autorService, IComicVineService comicVineService, ILogger<Manejador> logger) {
                _contexto = contexto;
                _libroService = libroService;
                _autorService = autorService;
                _comicVineService = comicVineService;
                _logger = logger;
                _random = new Random();
            }

            public async Task<CarritoDto> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var carritoSesion = await _contexto.CarritoSesion.FirstOrDefaultAsync(x => x.CarritoSesionId == request.CarritoSesionId);
                if (carritoSesion == null) {
                    _logger.LogWarning($"No se encontró la sesión de carrito con id {request.CarritoSesionId}");
                    return null;
                }
                var carritoSesionDetalle = await _contexto.CarritoSesionDetalle.Where(x => x.CarritoSesionId == request.CarritoSesionId).ToListAsync();

                var listaCarritoDto = new List<CarritoDetalleDto>();
                double totalCarrito = 0;

                foreach (var item in carritoSesionDetalle) {
                    _logger.LogInformation($"Procesando item: {item.ProductoSeleccionado}");
                    
                    double precioMock = Math.Round((_random.NextDouble() * 90) + 10, 2);
                    totalCarrito += precioMock;

                    // Determinamos si es un Comic o un Libro basado en un prefijo
                    if (item.ProductoSeleccionado.StartsWith("comic-"))
                    {
                        var comicId = item.ProductoSeleccionado.Replace("comic-", "");
                        var response = await _comicVineService.GetComic(comicId);
                        
                        if (response.resultado && response.Comic?.Results != null)
                        {
                            var comic = response.Comic.Results;
                            listaCarritoDto.Add(new CarritoDetalleDto
                            {
                                ProductoId = item.ProductoSeleccionado,
                                TituloProducto = comic.Name ?? $"Comic #{comic.Id}",
                                AutorProducto = "Comic Vine API", // Comic Vine API doesn't always provide a single author easily
                                FechaPublicacion = null, // Deck is usually description, not date
                                PrecioUnitario = precioMock,
                                TipoProducto = "Comic",
                                ImagenUrl = comic.Image?.OriginalUrl
                            });
                        }
                        else
                        {
                            _logger.LogWarning($"No se pudo obtener el comic con id {comicId}. Error: {response.ErrorMessage}");
                            listaCarritoDto.Add(new CarritoDetalleDto
                            {
                                ProductoId = item.ProductoSeleccionado,
                                TituloProducto = "Comic no disponible",
                                PrecioUnitario = precioMock,
                                TipoProducto = "Comic"
                            });
                        }
                    }
                    else
                    {
                        // Si no tiene prefijo 'comic-', asumimos que es un GUID de un Libro (comportamiento original)
                        if (Guid.TryParse(item.ProductoSeleccionado, out Guid libroGuid))
                        {
                            var response = await _libroService.GetLibro(libroGuid);
                            if (response.resultado) {
                                var objetoLibro = response.Libro;
                                string nombreAutor = "Autor no encontrado";

                                if (objetoLibro.AutorLibro.HasValue) {
                                    var autorResponse = await _autorService.GetAutor(objetoLibro.AutorLibro.Value);
                                    if(autorResponse.resultado && autorResponse.Autor != null)
                                    {
                                        var objetoAutor = autorResponse.Autor;
                                        nombreAutor = $"{objetoAutor.Nombre} {objetoAutor.Apellido}";
                                    }
                                }

                                listaCarritoDto.Add(new CarritoDetalleDto
                                {
                                    ProductoId = item.ProductoSeleccionado,
                                    TituloProducto = objetoLibro.Titulo,
                                    FechaPublicacion = objetoLibro.FechaPublicacion,
                                    AutorProducto = nombreAutor,
                                    PrecioUnitario = precioMock,
                                    TipoProducto = "Libro"
                                });
                            }
                            else
                            {
                                _logger.LogWarning($"No se pudo obtener el libro con id {item.ProductoSeleccionado}. Error: {response.ErrorMessage}");
                                listaCarritoDto.Add(new CarritoDetalleDto
                                {
                                    ProductoId = item.ProductoSeleccionado,
                                    TituloProducto = "Libro no disponible",
                                    PrecioUnitario = precioMock,
                                    TipoProducto = "Libro"
                                });
                            }
                        }
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