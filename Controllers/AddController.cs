using Microsoft.AspNetCore.Mvc;
using WebLab.Models;

namespace WebLab.Controllers
{
    public class AddController : Controller
    {
        private readonly DbConfig db;
        private readonly IWebHostEnvironment env;

        public AddController(DbConfig db, IWebHostEnvironment env)
        {
            this.db = db;
            this.env = env;
        }

        // GET /add — форма добавления книги
        [HttpGet("/add")]
        public IActionResult Index()
        {
            return View("~/Views/Book/Add.cshtml");
        }

        // POST /add — реальная вставка новой книги в таблицу books
        [HttpPost("/add")]
        public IActionResult Create(string title, string author, string genre,
                                    string coverUrl, IFormFile? cover,
                                    string reviewText, int? reviewRating, string reviewName)
        {
            title = (title ?? string.Empty).Trim();
            author = (author ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author))
            {
                return BadRequest("Название и автор книги обязательны.");
            }

            var coversDir = Path.Combine(env.WebRootPath, "covers");
            Directory.CreateDirectory(coversDir);

            // Загрузка обложки: файл с формы либо URL из формы
            var finalCoverUrl = (coverUrl ?? string.Empty).Trim();

            if (cover != null && cover.Length > 0)
            {
                var safeName = Path.GetFileNameWithoutExtension(cover.FileName)
                    .Replace(" ", "-");
                var ext = Path.GetExtension(cover.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(ext))
                {
                    ext = ".jpg";
                }

                var fileName = $"{DateTime.Now:yyyyMMddHHmmss}-{safeName}{ext}";
                using (var stream = System.IO.File.Create(Path.Combine(coversDir, fileName)))
                {
                    cover.CopyTo(stream);
                }

                finalCoverUrl = $"/covers/{fileName}";
            }

            var bookId = BookRepository.InsertBook(db, title, author,
                                                   genre?.Trim() ?? string.Empty,
                                                   finalCoverUrl);

            // Если в форме есть рецензия — сразу вставляем отзыв в reviews
            if (!string.IsNullOrWhiteSpace(reviewText))
            {
                var username = string.IsNullOrWhiteSpace(reviewName)
                    ? "Гость"
                    : reviewName.Trim();

                BookRepository.InsertReview(db, bookId, username,
                                            reviewText.Trim(), reviewRating);
            }

            return Redirect($"/book/{bookId}");
        }
    }
}
