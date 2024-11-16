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

            return authorWithMinId;
        }

        public async Task<List<Author>> GetAuthors()
        {
            return await _dbContext.Authors
            .Where(author => _dbContext.Books
                .Count(book => book.AuthorId == author.Id && book.PublishDate.Year > 2015) % 2 == 0)
            .ToListAsync();
        }
    }
}
