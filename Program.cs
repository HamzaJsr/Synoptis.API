using System.Text;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Synoptis.API.Data;
using Synoptis.API.DTOs;
using Synoptis.API.Mappings;
using Synoptis.API.Models;
using Synoptis.API.Services;
using Synoptis.API.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Enregistre les services MVC / API //Sans ça, ASP.NET Core ne saura pas comment instancier tes controllers, ni comment faire la liaison modèle → JSON, ni appliquer les attributs [HttpGet], [FromBody], etc.

builder.Services.AddControllers()
//et la on s'occupe de permettre de recvoir du text json pour les enums
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });


// 🔍 On ajoute un explorateur de endpoints HTTP
// Il scanne les routes (GET, POST…) = détection des routes
builder.Services.AddEndpointsApiExplorer();


// Pour la route /me pour 
TypeAdapterConfig.GlobalSettings.Scan(typeof(UserMappingConfig).Assembly);


// 🔍 On ajoute un explorateur de endpoints HTTP
// Il scanne les routes (GET, POST…) pour les rendre visibles dans Swagger = génération du document et de l’UI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Synoptis.API", Version = "v1" });
    // Voici la config pour activer le support JWT dans Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrer 'Bearer' suivi d'un espace et du token JWT. \r\n\r\nExemple : \"Bearer abcdef12345\""
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


// Ici je connecte/j'enregistre le DB context dans le program.cs 

builder.Services.AddDbContext<SynoptisDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ici j'ajoute le service appel d'offre pour pouvoir l'injecter dans le controlleurs.

builder.Services.AddScoped<IAppelOffreService, AppelOffreService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<EnumToStringService>();


// 🔑 Lecture de la clé JWT depuis la configuration et vérification qu'elle est bien définie
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT secret key is missing.");

// 🔐 Configure le système d'authentification de l'application pour utiliser le schéma JWT Bearer par défaut
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // ⚙️ Paramètres de validation du token JWT reçu dans les requêtes
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // ✅ Vérifie que l'émetteur du token est bien celui attendu (ex: ton backend)
            ValidateIssuer = true,

            // ✅ Vérifie que l’audience du token correspond à ce que tu attends (ex: un client autorisé)
            ValidateAudience = true,

            // ✅ Vérifie que le token n'est pas expiré
            ValidateLifetime = true,

            // ✅ Vérifie que la signature du token est valide (grâce à la clé secrète)
            ValidateIssuerSigningKey = true,

            // 🎯 L’émetteur attendu du token (doit correspondre à la valeur dans le token)
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            // 🎯 L’audience attendue du token (pareil, doit correspondre)
            ValidAudience = builder.Configuration["Jwt:Audience"],

            // 🔐 Utilisation de la Clé utilisée pour valider la signature du token
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// 🛂 Active le système d'autorisation basé sur les [Authorize], rôles, policies, etc.
// Nécessaire pour que les contrôleurs puissent vérifier si l'utilisateur est autorisé à accéder à une ressource
builder.Services.AddAuthorization();

// 1) Déclare la politique CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
          .WithOrigins("http://localhost:3000")
          .AllowAnyHeader()
          .AllowAnyMethod();
    });
});

// 🏗️ On construit l'application avec les services définis au-dessus
var app = builder.Build();

// Configure the HTTP request pipeline.
// 🧪 Si on est en mode développement (local, pas prod)
if (app.Environment.IsDevelopment())
{

    // 📄 Génère le fichier swagger.json contenant toutes les routes décrites
    app.UseSwagger();
    // 💻 Affiche l’interface web Swagger à l’URL /swagger
    app.UseSwaggerUI();
}

// // 🔐 Redirige automatiquement vers HTTPS si l'utilisateur utilise HTTP
// app.UseHttpsRedirection();

// 2) Place UseCors AVANT UseAuthorization / MapControllers
app.UseCors("AllowFrontend");

// 🔓 Analyse le token JWT envoyé dans l'en-tête Authorization, vérifie sa validité,
// et identifie l'utilisateur (principal) à partir du token.
// Obligatoire pour que le système [Authorize] fonctionne
app.UseAuthentication();

// ✅ Une fois que l'utilisateur est "authentifié", cette étape vérifie s'il est "autorisé"
// à accéder à une route (selon les rôles, policies, ou simplement la présence du token)
app.UseAuthorization();

// 🔄 Connecte les routes des contrôleurs les active on vas dire
// Exemple : [Route("api/ao")] dans AOController → devient accessible
//Sans ça, même si tes controllers sont enregistrés, aucune route ne sera active pour les atteindre : tes méthodes [HttpGet], [HttpPost], etc. ne répondront pas.
app.MapControllers();

app.Use(async (context, next) =>
{
    var endpoint = context.GetEndpoint();
    if (endpoint != null)
        Console.WriteLine($"[ROUTE MATCH] {endpoint.DisplayName}");

    await next();
});


// ▶️ Lancement de l'application
app.Run();

