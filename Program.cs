using System.Text;
using System.Security.Cryptography.Xml;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enuns;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;

#region Builder
var builder = WebApplication.CreateBuilder(args);

    var key = builder.Configuration.GetSection("Jwt").ToString();
    if(string.IsNullOrEmpty(key)) key = "123456";

    builder.Services.AddAuthentication(option => {
        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(option => {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            //ValidateAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

    //builder.Services.AddAuthentication();
    builder.Services.AddAuthorization();

    builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
    builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(Options => {
        Options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Insira o token JWT aqui: {Seu token}"
        });
        Options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

    builder.Services.AddDbContext<DbContexto>(options =>{
        options.UseMySql(
            builder.Configuration.GetConnectionString("MySql"),
            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql"))
        );
    });

    var app = builder.Build();
#endregion

#region Home
    app.MapGet("/", () => Results.Json(new Home()))
    .AllowAnonymous()
    .WithTags("Home");
#endregion

#region Administradores

string GerarTokenJwt(Administrador administrador)
{
    if(string.IsNullOrEmpty(key)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil),
        new Claim(ClaimTypes.Role, administrador.Perfil),
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );
    return new JwtSecurityTokenHandler().WriteToken(token);
}

//Login de Administradores
    app.MapPost("/Administrador/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {
        var adm = administradorServico.Login(loginDTO);
        if(adm != null)
        {
            string token = GerarTokenJwt(adm);
            return Results.Ok(new AdministradorLogado
            {
                Email = adm.Email,
                Perfil = adm.Perfil,
                Token = token
            });
        }
        else
            return Results.Unauthorized();
    })
    .AllowAnonymous()
    .WithTags("Administradores");

//Incluir Administradores
    app.MapPost("/Administrador", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) => {
        
        var validacao = new ErrosDeValidacao{
        Mensagens = new List<string>()
    };

        if(string.IsNullOrEmpty(administradorDTO.Email))
            validacao.Mensagens.Add("O Email não pode ser vazio.");
        if(string.IsNullOrEmpty(administradorDTO.Senha))
            validacao.Mensagens.Add("A Senha não pode ser vazia.");
        if(administradorDTO.Perfil == null)
            validacao.Mensagens.Add("O Perfil não pode ser vazio.");
        
        var administrador = new Administrador{
            Email = administradorDTO.Email,
            Senha = administradorDTO.Senha,
            Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
        };
        administradorServico.Incluir(administrador);

        return Results.Created($"/administrador/{administrador.Id}", (new AdministradorModelView{
                    Id = administrador.Id,
                    Email = administrador.Email,
                    Perfil = administrador.Perfil
                }));
    })
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
    .WithTags("Administradores");

//Buscar Todos os Administradores   
    app.MapGet("/Administrador", ([FromQuery] int? pagina, IAdministradorServico administradorServico) => {
        var adms = new List<AdministradorModelView>();
        var administradores = administradorServico.Todos(pagina);

        foreach (var adm in administradores)
        {
            adms.Add(new AdministradorModelView{
                    Id = adm.Id,
                    Email = adm.Email,
                    Perfil = adm.Perfil
                    //Perfil = (Perfil)Enum.Parse(typeof(Perfil),adm.Perfil)
            });
        }
        return Results.Ok(adms);
    })
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
    .WithTags("Administradores").WithTags("Administradores");

//Selecionar Administrador por Id
    app.MapGet("/Administrador/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) => {
        var administrador = administradorServico.BuscaPorId(id);

        if(administrador == null) return Results.NotFound();

        return Results.Ok(new AdministradorModelView{
                    Id = administrador.Id,
                    Email = administrador.Email,
                    Perfil = administrador.Perfil
                });
    })
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
    .WithTags("Administradores");

#endregion

#region  Veiculos

ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosDeValidacao{
        Mensagens = new List<string>()
    };

        if(string.IsNullOrEmpty(veiculoDTO.Nome))
            validacao.Mensagens.Add("O Nome do Veículo não pode ser vazio.");

        if(string.IsNullOrEmpty(veiculoDTO.Marca))
            validacao.Mensagens.Add("A Marca não pode ficar em branco.");

        if(veiculoDTO.Ano < 1901)
            validacao.Mensagens.Add("Veículo muito antigo, aceito somente anos superiores a 1900.");

        return validacao;
}

// incluir Veiculo
    app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {

        var validacao = validaDTO(veiculoDTO); 
        if(validacao.Mensagens.Count > 0)
            return Results.BadRequest(validacao);

        var veiculo = new Veiculo{
            Nome = veiculoDTO.Nome,
            Marca = veiculoDTO.Marca,
            Ano = veiculoDTO.Ano
        };
        veiculoServico.Incluir(veiculo);

        return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
    })
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
    .WithTags("Veiculos");

 //Buscar todos Veiculos   
    app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) => {
        var veiculos = veiculoServico.Todos(pagina);

        return Results.Ok(veiculos);
    })
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
    .WithTags("Veiculos");

//Selecionar Veiculos por Id
    app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) => {
        var veiculo = veiculoServico.BuscaPorId(id);

        if(veiculo == null) return Results.NotFound();

        return Results.Ok(veiculo);
    })
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
    .WithTags("Veiculos");

//Atualizar Veiculo por Id
    app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {
        
        var veiculo = veiculoServico.BuscaPorId(id);
        if(veiculo == null) return Results.NotFound();

        var validacao = validaDTO(veiculoDTO); 
        if(validacao.Mensagens.Count > 0)
            return Results.BadRequest(validacao);

        veiculo.Nome = veiculoDTO.Nome;
        veiculo.Marca = veiculoDTO.Marca;
        veiculo.Ano = veiculoDTO.Ano;

        veiculoServico.Atualizar(veiculo);

        return Results.Ok(veiculo);
    })
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
    .WithTags("Veiculos");

//Deletar Veiculo por Id
    app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) => {
        var veiculo = veiculoServico.BuscaPorId(id);

        if(veiculo == null) return Results.NotFound();

        veiculoServico.Apagar(veiculo);

        return Results.NoContent();
    })
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
    .WithTags("Veiculos");

#endregion

#region App
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseAuthentication();
    app.UseAuthorization();

    app.Run();
#endregion