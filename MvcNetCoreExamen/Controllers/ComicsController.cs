using Microsoft.AspNetCore.Mvc;
using MvcNetCoreExamen.Models;
using MvcNetCoreExamen.Repositories;

namespace MvcNetCoreExamen.Controllers
{
    public class ComicsController : Controller
    {
        private IRepositoryComics repo;

        public ComicsController(IRepositoryComics repo)
        {
            this.repo = repo;
        }

        public IActionResult Index()
        {
            List<Comic> comics = this.repo.GetComics();
            return View(comics);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Comic comic)
        {
            this.repo.InsertComic(comic.Nombre, comic.Imagen, comic.Descripcion);
            return RedirectToAction("Index");
        } 

        public IActionResult DeleteComic(int idcomic)
        {
            var comic = this.repo.GetComicID(idcomic);
            return View(comic);
            /*this.repo.DeleteComic(idcomic);
            return RedirectToAction("Index");*/
        }

        [HttpPost]
        public IActionResult DeleteComic(int idcomic, string nombre)
        {
            this.repo.DeleteComic(idcomic,nombre);
            return RedirectToAction("Index");
        }

        public IActionResult BuscadorComics()
        {
            ViewData["NOMBRES"] = this.repo.GetNombresComics();
            return View();
        }
        
        [HttpPost]
        public IActionResult BuscadorComics(string nombre)
        {
            ViewData["NOMBRES"] = this.repo.GetNombresComics();
            Comic comic = this.repo.GetComicNombre(nombre);
            return View(comic);
        }

        public IActionResult CreateProcedure()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateProcedure(string nombre, string imagen, string descripcion)
        {
            this.repo.InsertComicProcedure(nombre,imagen, descripcion);
            return RedirectToAction("Index");
        }
    }
}
