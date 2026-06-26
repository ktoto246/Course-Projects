# Course Projects - WPF & Windows Forms Projects

Коллекция учебных и курсовых проектов на **WPF** и **Windows Forms** на C#. Репозиторий включает полные приложения с базами данных SQL Server.

## 🚀 Быстрый старт

### Требования
- **Visual Studio** 2019+ или 2022
- **.NET Framework** 4.7.2+ (для Windows Forms)
- **.NET** 8.0+
- **SQL Server** 2019+ или SQL Server Express

### Установка БД

Каждый проект содержит SQL скрипт в своей папке. Для создания базы данных:

1. Откройте **SQL Server Management Studio**
2. Перейдите в папку проекта и найдите файл `.sql`
3. Откройте скрипт в SQL Server Management Studio
4. Выполните скрипт (`Ctrl+Shift+E` или через Query > Execute)
5. Обновите connection string в приложении при необходимости (если отличается от стандартного)

### Запуск приложения

#### Windows Forms
```
1. Откройте проект в Visual Studio
2. Нажмите F5 или Debug > Start Debugging
3. Приложение запустится с главной формой
```

#### WPF
```
1. Откройте проект в Visual Studio
2. Установите как Startup Project (правый клик > Set as Startup Project)
3. Нажмите F5
```

## 🗄️ Базы данных

Каждое приложение содержит SQL скрипт **в своей папке** для инициализации базы данных:

- **Формат:** `[ProjectName].sql`
- **Расположение:** `/WPF/[ProjectName]/` или `/WindowsForms/[ProjectName]/`
- **Создание:** Откройте скрипт в SQL Server Management Studio и выполните его

### Connection String

Стандартный формат подключения (обновите при необходимости):
```
Server=YOUR_SERVER_NAME;Database=YOUR_DATABASE_NAME;Integrated Security=true;
```

Найти имя сервера можно через:
- SQL Server Management Studio (обычно `(localdb)\MSSQLLocalDB` или `.\SQLEXPRESS`)
- Или через меню: Tools > Options > Database Tools > Data Connections

## 🛠️ Технологический стек

### Все проекты используют:
- **Язык:** C#
- **ОС:** Windows
- **СУБД:** SQL Server
- **IDE:** Visual Studio

### WPF специфическое:
- XAML для UI
- Data Binding

### Windows Forms специфическое:
- Designer (VS Designer)
- Event-driven архитектура
- Классический подход к UI

## 📝 Добавление новых проектов

При добавлении нового проекта в репозиторий:

1. **Создайте папку** в соответствующей директории (`/WPF` или `/WindowsForms`)
2. **Положите проект** туда (`.csproj`, исходники и т.д.)
3. **Добавьте SQL скрипт** в папку `/SQL`
4. **Обновите таблицы** в этом README
5. **Создайте `README.md`** в папке проекта с описанием (опционально):
   ```markdown
   # Название проекта
   
   ## Описание
   Краткое описание того, что делает приложение
   
   ## Функциональность
   - Функция 1
   - Функция 2
   
   ## Требования
   - .NET Framework 4.7.2+
   - SQL Server Database: [DatabaseName]
   
   ## Запуск
   1. Выполните скрипт: `SQL/ProjectName.sql`
   2. Откройте .sln в Visual Studio
   3. Нажмите F5
   ```

## 📌 Полезные ссылки

- [WPF Documentation](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
- [Windows Forms Documentation](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/)
- [SQL Server Express Download](https://www.microsoft.com/en-us/sql-server/sql-server-express)
- [Visual Studio](https://visualstudio.microsoft.com/)

## ⚠️ Важные замечания

- Все приложения требуют **SQL Server** для работы
- Connection strings могут отличаться в зависимости от вашей конфигурации SQL Server
- Проекты созданы в образовательных целях

## 📄 Лицензия

Эти проекты являются учебными материалами.

---
