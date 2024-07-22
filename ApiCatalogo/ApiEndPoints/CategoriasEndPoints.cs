using ApiCatalogo.Context;
using ApiCatalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.ApiEndPoints
{
    public static class CategoriasEndPoints
    {
        public static void MapCategoriasEndPoints(this WebApplication app)
        {
            #region Listar Categorias
            app.MapGet("/categorias", async (AppDbContext db) => await db.Categorias.ToListAsync()).WithTags("Categorias").RequireAuthorization();
            #endregion

            #region Retornar uma categoria pelo ID
            app.MapGet("/categorias/{id:int}", async (int id, AppDbContext db) =>
            {
                return await db.Categorias.FindAsync(id) is CategoriaModel categoria ? Results.Ok(categoria) : Results.NotFound();
            });
            #endregion

            #region Criar uma nova categoria
            app.MapPost("/categorias", async (CategoriaModel categoria, AppDbContext db) =>
            {
                db.Categorias.Add(categoria);
                await db.SaveChangesAsync();

                return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
            });
            #endregion

            #region Atualizar uma categoria
            app.MapPut("/categorias/{id:int}", async (int id, CategoriaModel categoria, AppDbContext db) =>
            {
                if (categoria.CategoriaId != id)
                {
                    return Results.BadRequest();
                }

                var categoriaDB = await db.Categorias.FindAsync(id);

                if (categoriaDB is null) return Results.NotFound();

                categoriaDB.Nome = categoria.Nome;
                categoriaDB.Descricao = categoria.Descricao;

                await db.SaveChangesAsync();

                return Results.Ok(categoriaDB);
            });
            #endregion

            #region Deletar uma categoria
            app.MapDelete("/categorias/{id:int}", async (int id, AppDbContext db) =>
            {
                var categoriaDelete = await db.Categorias.FindAsync(id);

                if (categoriaDelete is null)
                {
                    return Results.NotFound();
                }

                db.Remove(categoriaDelete);
                await db.SaveChangesAsync();

                return Results.Ok(categoriaDelete);
            });
            #endregion
        }
    }
}
