-- Схема базы данных books_db (MySQL / MariaDB)
CREATE DATABASE IF NOT EXISTS books_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE books_db;

CREATE TABLE IF NOT EXISTS books (
    id        INT AUTO_INCREMENT PRIMARY KEY,
    title     VARCHAR(255) NOT NULL,
    author    VARCHAR(255) NOT NULL,
    genre     VARCHAR(100) NULL,
    cover_url VARCHAR(500) NULL
);

CREATE TABLE IF NOT EXISTS reviews (
    id         INT AUTO_INCREMENT PRIMARY KEY,
    book_id    INT NOT NULL,
    username   VARCHAR(100) NOT NULL,
    text       TEXT NOT NULL,
    rating     TINYINT NULL,
    created_at DATETIME NOT NULL,
    CONSTRAINT fk_reviews_book FOREIGN KEY (book_id) REFERENCES books(id) ON DELETE CASCADE
);
