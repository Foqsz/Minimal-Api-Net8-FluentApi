using ApiCatalogo.Models;
using ApiCatalogo.Services;
using Microsoft.AspNetCore.Authorization;

namespace ApiCatalogo.ApiEndPoints
{
    public static class AutenticacaoEndPoints
    {
        public static void MapAutenticacaoEndPoints(this WebApplication app)
        { 
            #region Login Swagger
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
            #endregion
        }
    }
}
