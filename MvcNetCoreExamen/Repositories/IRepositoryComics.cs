using MvcNetCoreExamen.Models;

namespace MvcNetCoreExamen.Repositories
{
    public interface IRepositoryComics
    {
        List<Comic> GetComics();
        void InsertComic(string nombre, string imagen, string descripcion);

        void DeleteComic(int idComic, string nombre);

        Comic GetComicID(int idComic);

        List<string> GetNombresComics();

        Comic GetComicNombre(string nombre);

        void InsertComicProcedure(string nombre, string imagen, string descripcion);
    }
}
