using ApiCatalogo.ApiEndPoints;
using ApiCatalogo.AppServicesExtensions;
using ApiCatalogo.Context; 
using ApiCatalogo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; 
using System.Text; 

var builder = WebApplication.CreateBuilder(args);

builder.AddApiSwagger();
builder.AddPersistence();
builder.Services.AddCors();
builder.AddAutenticationJwt();

var app = builder.Build();

app.MapAutenticacaoEndPoints();
app.MapCategoriasEndPoints();
app.MapProdutosEndPoints();

var environmet = app.Environment;

app.UseExceptionHanling(environmet).UseSwaggerMiddleware().UseAppCors();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
