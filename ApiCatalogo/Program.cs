using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "apiagenda", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space].Example: \'Bearer 12355656\'"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
                });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddSingleton<ITokenService>(new TokenService());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();



var app = builder.Build();

//endpoint login

app.MapPost("/login", [AllowAnonymous] (UserModel userModel, ITokenService tokenService) =>
{
    if (userModel == null)
    {
        return Results.BadRequest("Login Invalido");
    }

    if (userModel.UserName == "Vinicius" && userModel.Password == "Foqs#123")
    {
        var tokenString = tokenService.GerarToken(app.Configuration["Jwt:Key"],
        app.Configuration["Jwt:Issuer"],
        app.Configuration["Jwt:Audience"], userModel);

        return Results.Ok(new { token = tokenString });
    }
    else
    {
        return Results.BadRequest("Login invalido");
    }
}).Produces(StatusCodes.Status400BadRequest).Produces(StatusCodes.Status200OK).WithName("Login").WithTags("Autenticacao");

app.MapGet("/", () => "Catalogo de Produtos - 2024").ExcludeFromDescription();

//Listar todas as categorias
app.MapGet("/categorias", async (AppDbContext db) => await db.Categorias.ToListAsync()).WithTags("Categorias").RequireAuthorization();

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
app.MapGet("/produtos", async (AppDbContext db) => await db.Produtos.ToListAsync()).WithTags("Produtos").RequireAuthorization();

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

app.UseAuthentication();
app.UseAuthorization();

app.Run();
