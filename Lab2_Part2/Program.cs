using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Lab2_Part2
{
    // 1. Сущность (Entity)
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    // 2. Контекст базы данных (EF Core)
    public class AppDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Используем локальный файл SQLite
            optionsBuilder.UseSqlite("Data Source=library.db");
        }
    }

    // 3. Обобщенный интерфейс репозитория
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(int id);
    }

    // 4. Реализация Generic Repository
    public class EfRepository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public EfRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task AddAsync(T entity) { await _dbSet.AddAsync(entity); await _context.SaveChangesAsync(); }
        public void Update(T entity) { _dbSet.Update(entity); _context.SaveChanges(); }
        public void Delete(int id) { var entity = _dbSet.Find(id); if (entity != null) { _dbSet.Remove(entity); _context.SaveChanges(); } }
    }

    // 5. Специализированный репозиторий для книг
    public interface IBookRepository : IRepository<Book>
    {
        Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author);
    }

    public class BookRepository : EfRepository<Book>, IBookRepository
    {
        public BookRepository(AppDbContext context) : base(context) { }
        public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author)
        {
            return await _context.Books.Where(b => b.Author.Contains(author)).ToListAsync();
        }
    }

    // 6. Главная программа для теста
    class Program
    {
        static async Task Main(string[] args)
        {
            // Создаем БД при первом запуске
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated();
            }

            using (var context = new AppDbContext())
            {
                var bookRepo = new BookRepository(context);

                Console.WriteLine("Добавление книг...");
                await bookRepo.AddAsync(new Book { Title = "Война и мир", Author = "Толстой", Price = 1500 });
                await bookRepo.AddAsync(new Book { Title = "Преступление и наказание", Author = "Достоевский", Price = 1200 });

                Console.WriteLine("\nСписок всех книг:");
                var allBooks = await bookRepo.GetAllAsync();
                foreach (var b in allBooks) Console.WriteLine($"- {b.Title} ({b.Author}) - {b.Price} руб.");

                Console.WriteLine("\nПоиск книг автора 'Толстой':");
                var tolstoyBooks = await bookRepo.GetBooksByAuthorAsync("Толстой");
                foreach (var b in tolstoyBooks) Console.WriteLine($"- {b.Title}");

                Console.WriteLine("\nУдаление книги с ID=1...");
                bookRepo.Delete(1);

                Console.WriteLine("\nОставшиеся книги:");
                foreach (var b in await bookRepo.GetAllAsync()) Console.WriteLine($"- {b.Title}");
            }
        }
    }
}