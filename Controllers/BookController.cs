using Microsoft.AspNetCore.Mvc;
using WebLab.Models;

namespace WebLab.Controllers
{
    public class BookController : Controller
    {
        private readonly DbConfig db;

        public BookController(DbConfig db)
        {
            this.db = db;
        }

        // GET /book/{id} — страница книги.
        // Книга и отзывы читаются из БД (reviews по book_id).
        [HttpGet("/book")]
        [HttpGet("/book/{id:int}")]
        public IActionResult Index(int id = 1)
        {
            var book = BookRepository.GetBook(db, id);

            if (book == null)
            {
                return NotFound($"Книга с id={id} не найдена в базе данных.");
            }

            ViewData["Book"] = book;
            ViewData["Reviews"] = BookRepository.GetReviews(db, id);

            return View("Book");
        }

        // POST /book/{id} — добавление отзыва: вставка в таблицу reviews
        [HttpPost("/book/{id:int}")]
        public IActionResult AddReview(int id, string name, string text, int? rating)
        {
            name = (name ?? string.Empty).Trim();
            text = (text ?? string.Empty).Trim();

            var book = BookRepository.GetBook(db, id);

            if (book != null &&
                !string.IsNullOrWhiteSpace(name) &&
                !string.IsNullOrWhiteSpace(text))
            {
                if (rating is < 1 or > 5)
                {
                    rating = null;
                }

                BookRepository.InsertReview(db, id, name, text, rating);
            }

            return Redirect($"/book/{id}");
        }
    }
}
