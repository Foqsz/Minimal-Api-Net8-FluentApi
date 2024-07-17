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

        protected override void OnModelCreating(ModelBuilder mb)
        { 
            //Categorias
            mb.Entity<CategoriaModel>().HasKey(c => c.CategoriaId);

            mb.Entity<CategoriaModel>().Property(c => c.Nome).HasMaxLength(100).IsRequired();   

            mb.Entity<CategoriaModel>().Property(c => c.Descricao).HasMaxLength(150).IsRequired();

            //Produtos
            mb.Entity<ProdutoModel>().HasKey(c => c.ProdutoId);
            mb.Entity<ProdutoModel>().Property(c => c.Nome).HasMaxLength(100).IsRequired();
            mb.Entity<ProdutoModel>().Property(c => c.Descricao).HasMaxLength(150);
            mb.Entity<ProdutoModel>().Property(c => c.Imagem).HasMaxLength(100);

            mb.Entity<ProdutoModel>().Property(c => c.Preco).HasPrecision(14, 2);

            //Relacionamento

            mb.Entity<ProdutoModel>().HasOne<CategoriaModel>(c => c.Categoria).WithMany(p => p.Produtos).HasForeignKey(c => c.CategoriaId);
        }
    }
}
