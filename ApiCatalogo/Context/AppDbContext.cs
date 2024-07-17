using ApiCatalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<ProdutoModel>? Produtos { get; set; }
        public DbSet<CategoriaModel>? Categorias { get; set; }
    }
}
