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

        // Справочник книг: код книги -> данные для страницы "Мастер и Маргарита"
        private static readonly Dictionary<string, BookInfo> Books =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["master-and-margarita"] = new BookInfo
                {
                    Title = "Мастер и Маргарита",
                    Author = "Михаил Булгаков",
                    Genre = "Фантастика, Классика, Мистика",
                    Year = 1967,
                    Pages = 480,
                    Image = "/images/master-and-margarita.jpg",
                    Description1 = "«Мастер и Маргарита» — роман Михаила Булгакова," +
                        " над которым он работал с конца 1920-х годов до своей смерти." +
                        " Это философское произведение, сочетающее в себе элементы" +
                        " мистики, сатиры и любовной истории.",
                    Description2 = "Действие романа разворачивается в Москве 1930-х годов," +
                        " куда прибывает дьявол со своей свитой. Параллельно" +
                        " развивается история любви Мастера и Маргариты," +
                        " а также рассказ о Понтии Пилате и Иешуа Га-Ноцри.",
                },
                ["1984"] = new BookInfo
                {
                    Title = "1984",
                    Author = "Джордж Оруэлл",
                    Genre = "Антиутопия, Классика",
                    Year = 1949,
                    Pages = 328,
                    Image = "/images/1984.png",
                    Description1 = "«1984» — антиутопия Джорджа Оруэлла, один из самых" +
                        " известных романов о тоталитарном обществе в мировой литературе.",
                    Description2 = "Действие происходит в государстве Океания, где за всеми" +
                        " следит Большой Брат, а Уинстон Смит пытается сохранить" +
                        " человеческое достоинство и способность мыслить свободно.",
                },
                ["crime-and-punishment"] = new BookInfo
                {
                    Title = "Преступление и наказание",
                    Author = "Фёдор Достоевский",
                    Genre = "Классика, Психология",
                    Year = 1866,
                    Pages = 671,
                    Image = "/images/crime-and-punishment.jpg",
                    Description1 = "«Преступление и наказание» — роман Фёдора Достоевского" +
                        " о студенте Родионе Раскольникове, решившемся на убийство ради" +
                        " проверки собственной теории.",
                    Description2 = "Произведение глубоко исследует психологию преступника," +
                        " темы совести, страдания и возможного искупления.",
                },
                ["great-gatsby"] = new BookInfo
                {
                    Title = "Великий Гэтсби",
                    Author = "Фрэнсис Скотт Фицджеральд",
                    Genre = "Классика, Драма",
                    Year = 1925,
                    Pages = 180,
                    Image = "/images/great-gatsby.jpg",
                    Description1 = "«Великий Гэтсби» — роман Фрэнсиса Скотта Фицджеральда," +
                        " летопись «века джаза» и американской мечты.",
                    Description2 = "История загадочного миллионера Джей Гэтсби, который" +
                        " устраивает роскошные вечеринки ради одной единственной цели —" +
                        " вернуть любовь Дэйзи Бьюкенен.",
                },
                ["harry-potter"] = new BookInfo
                {
                    Title = "Гарри Поттер и философский камень",
                    Author = "Дж. К. Роулинг",
                    Genre = "Фэнтези, Приключения",
                    Year = 1997,
                    Pages = 432,
                    Image = "/images/Harry-Potter-and-the-Philosopher's-Stone.jpg",
                    Description1 = "«Гарри Поттер и философский камень» — первая книга" +
                        " всемирно известной серии о юном волшебнике.",
                    Description2 = "Одиннадцатилетний Гарри узнаёт, что он волшебник," +
                        " и отправляется учиться в школу Хогвартс, где его ждут дружба," +
                        " тайны и первое противостояние с Тёмным Лордом.",
                },
                ["lord-of-the-rings"] = new BookInfo
                {
                    Title = "Властелин колец",
                    Author = "Дж. Р. Р. Толкин",
                    Genre = "Фэнтези, Эпос",
                    Year = 1954,
                    Pages = 1178,
                    Image = "/images/lord-of-the-rings.jpg",
                    Description1 = "«Властелин колец» — эпическое фэнтези Дж. Р. Р. Толкина" +
                        " о борьбе за судьбу Средиземья.",
                    Description2 = "Хоббит Фродо получает Кольцо Всевластья и должен" +
                        " уничтожить его в огне Роковой горы, пройдя через бесчисленные" +
                        " опасности вместе с Братством Кольца.",
                },
                ["catcher-in-the-rye"] = new BookInfo
                {
                    Title = "Над пропастью во ржи",
                    Author = "Джером Д. Сэлинджер",
                    Genre = "Классика, coming-of-age",
                    Year = 1951,
                    Pages = 214,
                    Image = "/images/catcher-in-the-rye.jpeg",
                    Description1 = "«Над пропастью во ржи» — единственный роман Джерома" +
                        " Сэлинджера и культовая книга о взрослении.",
                    Description2 = "Шестнадцатилетний Холден Колфилд бродит по Нью-Йорку," +
                        " размышляя о школе, лицемерии взрослых и о том, как уберечь" +
                        " детей от падения в пропасть взрослой жизни.",
                },
                ["fight-club"] = new BookInfo
                {
                    Title = "Бойцовский клуб",
                    Author = "Чак Паланик",
                    Genre = "Психология, Философия",
                    Year = 1996,
                    Pages = 224,
                    Image = "/images/Fight-club.jpg",
                    Description1 = "«Бойцовский клуб» — роман Чака Паланика, едкая сатира" +
                        " на общество потребления.",
                    Description2 = "Рассказчик, страдающий бессонницей, встречает" +
                        " харизматичного Тайлера Дёрдена, и вместе они основывают" +
                        " подпольный бойцовский клуб, который перерастает в нечто большее.",
                },
            };

        [HttpGet("/book")]
        [HttpGet("/book/{code}")]
        public IActionResult Index(string code = "master-and-margarita")
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                code = "master-and-margarita";
            }

            if (!Books.TryGetValue(code, out var book))
            {
                return NotFound($"Книга «{WebUtility.HtmlEncode(code)}» не найдена.");
            }

            var reviews = ReadReviews(book.Title);

            ViewData["Reviews"] = reviews;
            ViewData["Book"] = book;

            return View("Book");
        }

        [HttpPost("/book")]
        [HttpPost("/book/{code}")]
        public IActionResult AddReview(string name, string text, string code = "master-and-margarita")
        {
            name = NormalizeLineBreaks(StripTags(name ?? string.Empty)).Trim();
            text = NormalizeLineBreaks(StripTags(text ?? string.Empty)).Trim();

            if (!string.IsNullOrWhiteSpace(code) &&
                Books.TryGetValue(code, out var book) &&
                !string.IsNullOrWhiteSpace(name) &&
                !string.IsNullOrWhiteSpace(text))
            {
                var date = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                // Формат строки: Книга|Имя|Текст|Дата
                var review = $"{book.Title}|{name}|{text}|{date}";

                Directory.CreateDirectory(
                    Path.GetDirectoryName(reviewsFile)!
                );

                EnsureTrailingNewLine();

                System.IO.File.AppendAllText(
                    reviewsFile,
                    review + Environment.NewLine
                );
            }

            return Redirect($"/book/{code}");
        }

        private List<Review> ReadReviews(string bookTitle)
        {
            var reviews = new List<Review>();

            if (!System.IO.File.Exists(reviewsFile))
            {
                return reviews;
            }

            var lines = System.IO.File.ReadAllLines(reviewsFile);

            foreach (var line in lines)
            {
                var parts = line.Split('|', 4);

                if (parts.Length == 4)
                {
                    // Новый формат: Книга|Имя|Текст|Дата
                    if (!string.Equals(parts[0], bookTitle, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    reviews.Add(new Review
                    {
                        Name = StripTags(parts[1]),
                        Text = StripTags(parts[2]),
                        Date = StripTags(parts[3])
                    });
                }
                else
                {
                    var legacyParts = line.Split('|', 3);

                    if (legacyParts.Length == 3)
                    {
                        // Старый формат без привязки к книге: Имя|Текст|Дата
                        reviews.Add(new Review
                        {
                            Name = StripTags(legacyParts[0]),
                            Text = StripTags(legacyParts[1]),
                            Date = StripTags(legacyParts[2])
                        });
                    }
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

    public class BookInfo
    {
        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        public int Year { get; set; }

        public int Pages { get; set; }

        public string Image { get; set; } = string.Empty;

        public string Description1 { get; set; } = string.Empty;

        public string Description2 { get; set; } = string.Empty;
    }
}
