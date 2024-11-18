using Microsoft.EntityFrameworkCore;
using TestTask.Data;
using TestTask.Models;
using TestTask.Services.Interfaces;

namespace TestTask.Services.Implementations
{
    public class AuthorService : IAuthorService
    {
        private readonly ApplicationDbContext _dbContext;
        public AuthorService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Author> GetAuthor()
        {
            var booksWithAuthors = await _dbContext.Books
                .Include(b => b.Author)
                .ToListAsync();

            var maxLength = booksWithAuthors.Max(b => b.Title.Length);

            var authorsWithMaxLengthBooks = booksWithAuthors
                .Where(b => b.Title.Length == maxLength)
                .Select(b => b.Author)
                .Distinct()
                .ToList();

            var authorWithMinId = authorsWithMaxLengthBooks
                .OrderBy(a => a.Id)
                .FirstOrDefault();

            authorWithMinId.Books = null;

            return authorWithMinId;
        }

        public async Task<List<Author>> GetAuthors()
        {
            var booksAfter2015 = await _dbContext.Books
                .Where(b => b.PublishDate > new DateTime(2015, 1, 1))
                .Include(b => b.Author)
                .ToListAsync();

            // Группируем книги по авторам и выбираем тех, у кого четное количество книг
            var authorsWithEvenBooks = booksAfter2015
                .GroupBy(b => b.Author.Id)
                .Where(g => g.Count() % 2 == 0)
                .Select(g => g.First().Author) // Получаем автора из группы
                .Distinct()
                .ToList();
            authorsWithEvenBooks.ForEach(x => x.Books = null);
            return authorsWithEvenBooks;
        }
    }
}
