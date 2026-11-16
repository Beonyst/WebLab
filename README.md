Сайт библиотека с прочитанными книгами для отслеживания (всё специально сделано плохо - чтобы по меер выполнения лаб приводить сайт в нормальный вид)

## Настройка базы данных (после переезда на MySQL/MariaDB)

Ошибка `MySqlException: Unable to connect to any of the specified MySQL hosts` означает,
что приложение не может подключиться к серверу MySQL на localhost:3306 — локально не
запущен MySQL/MariaDB (или строка подключения не совпадает с реальными учётными данными).

Что нужно сделать:

1. Установить и запустить MySQL или MariaDB (например, `sudo service mariadb start`,
   либо через XAMPP / установщик MySQL для Windows).
2. Создать базу, таблицы и тестовые данные:
   ```
   mysql -uroot -p < schema.sql
   mysql -uroot -p < seed.sql
   ```
3. Проверить строку подключения в `appsettings.json`
   (`ConnectionStrings:BooksDb`) — сервер, порт, имя БД, пользователь и пароль
   должны совпадать с вашей установки MySQL. По умолчанию ожидается:
   `server=localhost;port=3306;database=books_db;user=root;password=root`.
   При другом пароле измените значение в `appsettings.json` (или задайте
   переменную окружения `ConnectionStrings__BooksDb`).
