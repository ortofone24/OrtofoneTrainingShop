using System.Data.Entity;

namespace OrtofoneTrainingShop.Models.Data
{
    public class Database : DbContext
    {
        public DbSet <PageDTO> Pages { get; set; }
        public DbSet<SidebarDTO> Sidebar { get; set; }
        public DbSet<CategoryDTO> Categories { get; set; }
        public DbSet<ProductDTO> Products { get; set; }
    }
}