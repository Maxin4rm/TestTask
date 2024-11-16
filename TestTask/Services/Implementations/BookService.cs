using Microsoft.EntityFrameworkCore;
using TestTask.Data;
using TestTask.Models;
using TestTask.Services.Interfaces;
using static System.Reflection.Metadata.BlobBuilder;

namespace TestTask.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _dbContext;
        public BookService(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<Book> GetBook()
        {
            return await _dbContext.Books
            .OrderByDescending(book => book.Price * book.QuantityPublished)
            .FirstOrDefaultAsync();
        }

        public async Task<List<Book>> GetBooks()
        {
            DateTime albumReleaseDate = new DateTime(2012, 5, 25);
            return await _dbContext.Books
                .Where(book => book.Title.Contains("Red") && book.PublishDate > albumReleaseDate)
                .ToListAsync();
        }
    }
}
