using JornadaMilhas.API.Endpoint;
using JornadaMilhas.API.Service;
using JornadaMilhas.Dados;
using JornadaMilhas.Dados.Database;
using JornadaMilhas.Dominio.Entidades;
using JornadaMilhas.Dominio.ValueObjects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddDbContext<JornadaMilhasContext>((options) => {
    options
        .UseLazyLoadingProxies()
        .UseSqlServer(builder.Configuration["ConnectionString:DefaultConnection"]);
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<JornadaMilhasContext>()
    .AddDefaultTokenProviders();

builder.Services.AddTransient(typeof(EntityDAL<OfertaViagem>));
builder.Services.AddTransient(typeof(EntityDAL<Rota>));
builder.Services.AddTransient(typeof(OfertaViagemConverter));
builder.Services.AddTransient(typeof(RotaConverter));
builder.Services.AddTransient(typeof(PeriodoConverter));
//Tratamento do Token
builder.Services.AddTransient(typeof(GenerateToken));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(
    opt => opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["JWTTokenConfiguration:Audience"],
        ValidIssuer = builder.Configuration["JWTTokenConfiguration:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWTKey:key"]!)),
    });

builder.Services.AddCors();

builder.Services.ConfigureSwaggerBearer();

var app = builder.Build();

MigracoesPendentes.ExecuteMigration(app);

ConfigureDefaultUser(app.Services).GetAwaiter().GetResult();

AdicionaRegistrosTabela(app.Services).GetAwaiter().GetResult();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options =>
{
    options.AllowAnyOrigin()
    .AllowAnyMethod()   
    .AllowAnyHeader();

});

app.UseSwagger();

app.UseHttpsRedirection();

//Endpoints
app.AddEndPointOfertas();
app.AddEndPointRota();
app.AddAuth();

app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

async Task ConfigureDefaultUser(IServiceProvider serviceProvider)
{
    using (var scope = serviceProvider.CreateScope())
    {     
        var userManager = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
        var defaultUser = await userManager!.FindByNameAsync("tester@email.com");

        if (defaultUser == null)
        {
            var identityUser = new IdentityUser
            {
                UserName = "tester@email.com",
                Email = "tester@email.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(identityUser,"Senha123@");
            if (!result.Succeeded)
            {
                throw new Exception("Erro ao criar o usuário padrão.");
            }
        }
    }
}

async Task AdicionaRegistrosTabela(IServiceProvider serviceProvider)
{
    using (var scope = serviceProvider.CreateScope())
    {
        using (var context = scope.ServiceProvider.GetService<JornadaMilhasContext>())
        {
            var ofertas = await context!.OfertasViagem.ToListAsync();
            if (ofertas.Count == 0)
            {
                var oferta1 = new OfertaViagem
                {
                    Preco = 108.99,
                    Rota = new Rota("Origem1", "Destino1"),
                    Periodo = new Periodo(new DateTime(2024, 1, 1), new DateTime(2024, 1, 5))
                };

                var oferta2 = new OfertaViagem
                {
                    Preco = 200.99,
                    Rota = new Rota("Origem2", "Destino2"),
                    Periodo = new Periodo(new DateTime(2024, 1, 1), new DateTime(2024, 1, 5))
                };

                context.OfertasViagem.AddRange(oferta1, oferta2);
                context.SaveChanges();
            }
        }
    }
}

public partial class Program { }



