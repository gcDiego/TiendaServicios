using System;
using System.Threading.Tasks;
using TiendaServicios.Api.CarritoCompra.RemoteModel;

namespace TiendaServicios.Api.CarritoCompra.RemoteInterface
{
    public interface IAutorService
    {
        Task<(bool resultado, AutorRemote Autor, string ErrorMessage)> GetAutor(Guid AutorId);
    }
}