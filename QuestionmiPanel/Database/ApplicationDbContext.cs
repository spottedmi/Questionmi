using Microsoft.EntityFrameworkCore;

namespace QuestionmiPanel.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
            { }

        public DbSet<Tell> Tells { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AccessCode> AccessCodes { get; set; }
        public DbSet<BadWord> BadWords { get; set; }
    }
}
