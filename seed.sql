-- Тестовые данные для books_db (обложки соответствуют файлам в wwwroot/images)
USE books_db;

INSERT INTO books (title, author, genre, cover_url) VALUES
('1984', 'Джордж Оруэлл', 'Антиутопия', '/images/1984.png'),
('Бойцовский клуб', 'Чак Паланик', 'Роман', '/images/Fight-club.jpg'),
('Гарри Поттер и философский камень', 'Джоан Роулинг', 'Фэнтези', '/images/Harry-Potter-and-the-Philosopher''s-Stone.jpg'),
('Над пропастью во ржи', 'Джером Д. Сэлинджер', 'Роман', '/images/catcher-in-the-rye.jpeg'),
('Преступление и наказание', 'Фёдор Достоевский', 'Классика', '/images/crime-and-punishment.jpg'),
('Великий Гэтсби', 'Фрэнсис Скотт Фицджеральд', 'Классика', '/images/great-gatsby.jpg'),
('Властелин колец', 'Джон Р. Р. Толкин', 'Фэнтези', '/images/lord-of-the-rings.jpg'),
('Мастер и Маргарита', 'Михаил Булгаков', 'Классика', '/images/master-and-margarita.jpg');

INSERT INTO reviews (book_id, username, text, rating, created_at) VALUES
(1, 'читатель', 'Сильная антиутопия, обязательно к прочтению.', 5, NOW()),
(5, 'студент', 'Тяжело, но того стоит.', 4, NOW());
