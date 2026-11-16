using Microsoft.AspNetCore.Mvc;
using WebLab.Models;

namespace WebLab.Controllers
{
    public class CatalogController : Controller
    {
        private readonly DbConfig db;

        public CatalogController(DbConfig db)
        {
            this.db = db;
        }

        // GET /catalog — каталог книг тянется из базы данных (SELECT * из books)
        [HttpGet("/catalog")]
        public IActionResult Index()
        {
            ViewData["Books"] = BookRepository.GetBooks(db);
            return View("~/Views/Book/Catalog.cshtml");
        }
    }
}
