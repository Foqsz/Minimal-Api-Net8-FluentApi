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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
} 
  
app.Run();
 