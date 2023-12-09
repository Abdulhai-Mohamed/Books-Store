using Books_Store.Models.AuthorModels;
using Microsoft.EntityFrameworkCore;

namespace Books_Store.Models.EF_Core_DB_Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasData(
                    new Author
                    {
                        Id = 2,
                        Name = "Mary",
                        Department = Dept.Professional,
                        Email = "mary@pragimtech.com"
                    },
                    new Author
                    {
                        Id = 3,
                        Name = "John",
                        Department = Dept.Beginner,
                        Email = "john@pragimtech.com"
                    }
                );
        }
    }
}