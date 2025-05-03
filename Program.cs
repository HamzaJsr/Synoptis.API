using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 🔧 Ajout des services Swagger
builder.Services.AddControllers();
// 🔍 On ajoute un explorateur de endpoints HTTP
// Il scanne les routes (GET, POST…) pour les rendre visibles dans Swagger
builder.Services.AddEndpointsApiExplorer();
// 🔍 On ajoute un explorateur de endpoints HTTP
// Il scanne les routes (GET, POST…) pour les rendre visibles dans Swagger
builder.Services.AddSwaggerGen();

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
app.MapControllers();

// ▶️ Lancement de l'application
app.Run();

