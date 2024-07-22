using ApiCatalogo.Context;
using ApiCatalogo.Models;

namespace ApiCatalogo.ApiEndPoints
{
    public static class ProdutosEndPoints
    {
        public static void MapProdutosEndPoints(this WebApplication app)
        {
            #region Listar todos os produtos
            app.MapGet("/produtos", async (AppDbContext db) => await db.Produtos.ToListAsync()).WithTags("Produtos").RequireAuthorization();
            #endregion

            #region Listar um produto pelo ID
            app.MapGet("/produtos/{id:int}", async (int id, AppDbContext db) =>
            {
                return await db.Produtos.FindAsync(id) is ProdutoModel produto ? Results.Ok(produto) : Results.NotFound();
            });
            #endregion

            #region Criar um novo produto
            app.MapPost("/produtos", async (ProdutoModel produto, AppDbContext db) =>
            {
                db.Produtos.Add(produto);
                await db.SaveChangesAsync();

                return Results.Created($"/produtos/{produto.ProdutoId}", produto);
            });
            #endregion

            #region Editar um Produto
            app.MapPut("/produtos/{id:int}", async (int id, ProdutoModel produto, AppDbContext db) =>
            {
                if (produto.ProdutoId != id)
                {
                    return Results.BadRequest();
                }

                var produtoDB = await db.Produtos.FindAsync(id);

                if (produtoDB is null) return Results.NotFound();

                produtoDB.Nome = produto.Nome;
                produtoDB.Descricao = produto.Descricao;
                produtoDB.Preco = produto.Preco;
                produtoDB.Imagem = produto.Imagem;
                produtoDB.DataCompra = produto.DataCompra;
                produtoDB.Estoque = produto.Estoque;
                produtoDB.CategoriaId = produto.CategoriaId;

                await db.SaveChangesAsync();
                return Results.Ok(produtoDB);
            });
            #endregion

            #region Deletar um produto
            app.MapDelete("/produtos/{id:int}", async (int id, AppDbContext db) =>
            {
                var produtoId = await db.Produtos.FindAsync(id);
                if (produtoId is null)
                {
                    return Results.NotFound();
                }

                db.Produtos.Remove(produtoId);
                await db.SaveChangesAsync();

                return Results.Ok(produtoId);
            });
            #endregion
        }
    }
}
