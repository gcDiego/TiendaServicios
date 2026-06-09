using System.Threading.Tasks;
using TiendaServicios.Api.CarritoCompra.RemoteModel;

namespace TiendaServicios.Api.CarritoCompra.RemoteInterface
{
    public interface IComicVineService
    {
        Task<(bool resultado, ComicVineVolumeResponse Comic, string ErrorMessage)> GetComic(string comicId);
    }
}