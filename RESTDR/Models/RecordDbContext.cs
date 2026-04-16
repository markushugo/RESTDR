using Microsoft.EntityFrameworkCore;
using RESTDanmarksRadio.Models;

namespace RESTDanmarksRadio.Models
{
    public class RecordDbContext : DbContext
    {
        public RecordDbContext(DbContextOptions<RecordDbContext> options) : base(options)
        {
        }

        public DbSet<Record> Records { get; set; }
    }
}