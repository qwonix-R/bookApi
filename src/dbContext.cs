using Microsoft.EntityFrameworkCore;

namespace bookApi
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions<BookContext> options)
            : base(options)
        {
        }

        // Добавьте DbSet для ваших моделей
        public DbSet<Book> books { get; set; }
        public DbSet<Comment> comments { get; set; }
    }
    public class Book
    {
        public long id { get; set; }
        public string? name { get; set; }
        public string? author { get; set; }
        public string? description { get; set; }
        public float? rating { get; set; }
    }
    public class Comment
    {
        public long? id { get; set; }
        public long book_id { get; set; }
        public long author_id { get; set; }
        public string text { get; set; }
        public float rating { get; set; }
    }
}
