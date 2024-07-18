using ApiCatalogo.Context;
using ApiCatalogo.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});


var app = builder.Build();

app.MapGet("/", () => "Catalogo de Produtos - 2024").ExcludeFromDescription();

//Listar todas as categorias
app.MapGet("/categorias", async (AppDbContext db) => await db.Categorias.ToListAsync());

//Retornar uma categoria pelo ID
app.MapGet("/categorias/{id:int}", async (int id, AppDbContext db) =>
{
    return await db.Categorias.FindAsync(id) is CategoriaModel categoria ? Results.Ok(categoria) : Results.NotFound();
});
  
//Criar uma nova categoria
app.MapPost("/categorias", async (CategoriaModel categoria, AppDbContext db) =>
{
    db.Categorias.Add(categoria);
    await db.SaveChangesAsync();

    return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
});

//Atualizar uma categoria
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

//Deletar uma categoria
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


//------ENDPOINTS PRODUTOS-------//


//Listar todos os produtos
app.MapGet("/produtos", async (AppDbContext db) => await db.Produtos.ToListAsync());

//Listar um produto pelo ID
app.MapGet("/produtos/{id:int}", async (int id, AppDbContext db) =>
{
    return await db.Produtos.FindAsync(id) is ProdutoModel produto ? Results.Ok(produto) : Results.NotFound();
});

//Criar um novo produto
app.MapPost("/produtos", async (ProdutoModel produto, AppDbContext db) =>
{
    db.Produtos.Add(produto);
    await db.SaveChangesAsync();

    return Results.Created($"/produtos/{produto.ProdutoId}", produto); 
});

//Atualizar um produto
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

//Deletar um produto
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
} 
  
app.Run();
 