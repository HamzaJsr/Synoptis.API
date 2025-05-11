using Microsoft.EntityFrameworkCore;
using Synoptis.API.Data;
using Synoptis.API.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Enregistre les services MVC / API //Sans ça, ASP.NET Core ne saura pas comment instancier tes controllers, ni comment faire la liaison modèle → JSON, ni appliquer les attributs [HttpGet], [FromBody], etc.
builder.Services.AddControllers();

// 🔍 On ajoute un explorateur de endpoints HTTP
// Il scanne les routes (GET, POST…) = détection des routes
builder.Services.AddEndpointsApiExplorer();

// 🔍 On ajoute un explorateur de endpoints HTTP
// Il scanne les routes (GET, POST…) pour les rendre visibles dans Swagger = génération du document et de l’UI
builder.Services.AddSwaggerGen();

// Ici je connecte/j'enregistre le DB context dans le program.cs 

builder.Services.AddDbContext<SynoptisDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// 🔐 Redirige automatiquement vers HTTPS si l'utilisateur utilise HTTP
app.UseHttpsRedirection();

// 🔄 Connecte les routes des contrôleurs les active on vas dire
// Exemple : [Route("api/ao")] dans AOController → devient accessible
//Sans ça, même si tes controllers sont enregistrés, aucune route ne sera active pour les atteindre : tes méthodes [HttpGet], [HttpPost], etc. ne répondront pas.
app.MapControllers();

// ▶️ Lancement de l'application
app.Run();

