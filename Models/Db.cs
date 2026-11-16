using MySqlConnector;

namespace WebLab.Models
{
    // Конфигурация подключения к БД books_db
    public class DbConfig
    {
        public string ConnectionString { get; }

        public DbConfig(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public MySqlConnection CreateConnection() => new(ConnectionString);
    }

    // Модель книги (таблица books)
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string CoverUrl { get; set; } = string.Empty;
        public double AvgRating { get; set; }
    }

    // Модель отзыва (таблица reviews)
    public class Review
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int? Rating { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Date => CreatedAt.ToString("dd.MM.yyyy HH:mm");
    }

    // Доступ к данным (SELECT / INSERT) через MySQL/MariaDB
    public static class BookRepository
    {
        // SELECT * из books — все книги для каталога
        public static List<Book> GetBooks(DbConfig db)
        {
            var books = new List<Book>();

            using var conn = db.CreateConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT b.id, b.title, b.author, b.genre, b.cover_url,
                       IFNULL(AVG(r.rating), 0) AS avg_rating
                FROM books b
                LEFT JOIN reviews r ON r.book_id = b.id
                GROUP BY b.id, b.title, b.author, b.genre, b.cover_url
                ORDER BY b.id";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                books.Add(new Book
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Author = reader.GetString(2),
                    Genre = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    CoverUrl = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    AvgRating = reader.GetDouble(5)
                });
            }

            return books;
        }

        // Одна книга по id
        public static Book? GetBook(DbConfig db, int id)
        {
            using var conn = db.CreateConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT b.id, b.title, b.author, b.genre, b.cover_url,
                       IFNULL(AVG(r.rating), 0)
                FROM books b
                LEFT JOIN reviews r ON r.book_id = b.id
                WHERE b.id = @id
                GROUP BY b.id, b.title, b.author, b.genre, b.cover_url";
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new Book
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Author = reader.GetString(2),
                Genre = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                CoverUrl = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                AvgRating = reader.GetDouble(5)
            };
        }

        // Отзывы читаются из таблицы reviews по book_id
        public static List<Review> GetReviews(DbConfig db, int bookId)
        {
            var reviews = new List<Review>();

            using var conn = db.CreateConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, book_id, username, text, rating, created_at
                FROM reviews
                WHERE book_id = @bookId
                ORDER BY created_at";
            cmd.Parameters.AddWithValue("@bookId", bookId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                reviews.Add(new Review
                {
                    Id = reader.GetInt32(0),
                    BookId = reader.GetInt32(1),
                    Username = reader.GetString(2),
                    Text = reader.GetString(3),
                    Rating = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                    CreatedAt = reader.GetDateTime(5)
                });
            }

            return reviews;
        }

        // Вставка новой книги в таблицу books, возвращает id
        public static int InsertBook(DbConfig db, string title, string author,
                                     string genre, string coverUrl)
        {
            using var conn = db.CreateConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO books (title, author, genre, cover_url)
                VALUES (@title, @author, @genre, @cover);
                SELECT LAST_INSERT_ID();";
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@author", author);
            cmd.Parameters.AddWithValue("@genre", (object?)genre ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@cover", (object?)coverUrl ?? DBNull.Value);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // Вставка отзыва в таблицу reviews
        public static void InsertReview(DbConfig db, int bookId, string username,
                                       string text, int? rating)
        {
            using var conn = db.CreateConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO reviews (book_id, username, text, rating, created_at)
                VALUES (@bookId, @username, @text, @rating, NOW());";
            cmd.Parameters.AddWithValue("@bookId", bookId);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@text", text);
            cmd.Parameters.AddWithValue("@rating", (object?)rating ?? DBNull.Value);

            cmd.ExecuteNonQuery();
        }
    }
}
