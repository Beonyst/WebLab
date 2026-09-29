using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebLab.Controllers
{
    public class BookController : Controller
    {
        private readonly string reviewsFile;

        public BookController(IWebHostEnvironment environment)
        {
            reviewsFile = Path.Combine(
                environment.WebRootPath,
                "reviews.txt"
            );
        }

        [HttpGet("/book")]
        public IActionResult Index()
        {
            var reviews = ReadReviews();

            ViewData["Reviews"] = reviews;

            return View("Book");
        }

        [HttpPost("/book")]
        public IActionResult AddReview(string name, string text)
        {
            name = NormalizeLineBreaks(StripTags(name ?? string.Empty)).Trim();
            text = NormalizeLineBreaks(StripTags(text ?? string.Empty)).Trim();

            if (!string.IsNullOrWhiteSpace(name) &&
                !string.IsNullOrWhiteSpace(text))
            {
                var date = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                var review = $"{name}|{text}|{date}";

                Directory.CreateDirectory(
                    Path.GetDirectoryName(reviewsFile)!
                );

                EnsureTrailingNewLine();

                System.IO.File.AppendAllText(
                    reviewsFile,
                    review + Environment.NewLine
                );
            }

            return Redirect("/book");
        }

        private List<Review> ReadReviews()
        {
            var reviews = new List<Review>();

            if (!System.IO.File.Exists(reviewsFile))
            {
                return reviews;
            }

            var lines = System.IO.File.ReadAllLines(reviewsFile);

            foreach (var line in lines)
            {
                var parts = line.Split('|', 3);

                if (parts.Length == 3)
                {
                    reviews.Add(new Review
                    {
                        Name = StripTags(parts[0]),
                        Text = StripTags(parts[1]),
                        Date = StripTags(parts[2])
                    });
                }
            }

            return reviews;
        }

        private static string StripTags(string value)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                value,
                "<.*?>",
                string.Empty
            );
        }

        private static string NormalizeLineBreaks(string value)
        {
            // Заменяем любые переносы строк (CRLF / LF / CR) на пробелы
            return System.Text.RegularExpressions.Regex.Replace(
                value,
                "\r\n|\n|\r",
                " "
            );
        }

        private void EnsureTrailingNewLine()
        {
            if (!System.IO.File.Exists(reviewsFile))
            {
                return;
            }

            var content = System.IO.File.ReadAllText(reviewsFile);

            if (content.Length > 0 &&
                !content.EndsWith("\n") &&
                !content.EndsWith("\r"))
            {
                System.IO.File.AppendAllText(
                    reviewsFile,
                    Environment.NewLine
                );
            }
        }
    }

    public class Review
    {
        public string Name { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public string Date { get; set; } = string.Empty;
    }
}