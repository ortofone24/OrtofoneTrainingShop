using System.Data.Entity;

namespace OrtofoneTrainingShop.Models.Data
{
    public class Database : DbContext
    {
        public DbSet <PageDTO> Pages { get; set; }
    }
}