using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TiendaServicios.Api.CarritoCompra.Aplicacion;

namespace TiendaServicios.Api.CarritoCompra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarritoComprasController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CarritoComprasController(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult> Crear(Nuevo.Ejecuta data)
        {
            await _mediator.Send(data);
            return Ok();
        }
        
        [HttpPost("agregar")]
        public async Task<ActionResult> Agregar(Agregar.Ejecuta data)
        {
            await _mediator.Send(data);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<CarritoDto>>> GetCarritos()
        {
            return await _mediator.Send(new ConsultaAll.Ejecuta());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CarritoDto>> GetCarrito(int id) {
            return await _mediator.Send(new Consulta.Ejecuta { CarritoSesionId = id });
        }

        [HttpDelete("{id}/productos/{productoId}")]
        public async Task<ActionResult> EliminarItem(int id, string productoId)
        {
            await _mediator.Send(new Eliminar.Ejecuta { CarritoSesionId = id, ProductoSeleccionadoId = productoId });
            return Ok();
        }
    }
}