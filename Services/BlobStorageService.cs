using Azure.Core;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Synoptis.API.Data;
using Synoptis.API.DTOs;
using Synoptis.API.Models;
using Synoptis.API.Services.Interfaces;

public class BlobStorageService : IBlobStorageService
{
    private readonly SynoptisDbContext _context;
    private readonly BlobServiceClient _blobServiceClient;//Client
    private readonly BlobContainerClient _blobContainerClient; //Container
    private readonly IConfiguration _config;


    public BlobStorageService(SynoptisDbContext context, BlobServiceClient blobServiceClient, IConfiguration config)
    {
        _context = context;
        _blobServiceClient = blobServiceClient;//Client
        _config = config;
        var containerName = config["AzureBlobStorage:ContainerName"];

        _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);//Container
    }

    public async Task<List<string>> ListFilesAsync()
    {
        var results = new List<string>();

        await foreach (var blobItem in _blobContainerClient.GetBlobsAsync())
        {
            results.Add(blobItem.Name); // Nom du blob (peut inclure des "dossiers")
        }

        return results;
    }

    public async Task<AppelOffreDocumentDTO> UploadDocumentAsync(UploadDocumentRequest request, string userId)
    {

        //Je cree un nom de fichier que jappelle nom de blob enfait quand le nom du fichier est un chemin ca va creer des dossier
        var blobName = $"{request.AppelOffreId}/{request.File.ContentType}/{request.File.FileName}";

        // La j'initialise un blob avec un nom de fichier
        var blobClient = _blobContainerClient.GetBlobClient(blobName);

        // La je cree un stream que je vais envoyer vers azure a partir du File recu 
        using var streamContent = request.File.OpenReadStream();

        await blobClient.UploadAsync(streamContent, overwrite: true);

        var doc = new DocumentAppelOffre
        {
            AppelOffreId = request.AppelOffreId,
            DeposeParId = Guid.Parse(userId),
            NomFichier = request.File.FileName,
            TypeDocument = request.File.ContentType,
            DateDepot = DateTime.UtcNow,
            Url = blobClient.Uri.ToString(),
        };

        _context.DocumentsAppelOffre.Add(doc);
        await _context.SaveChangesAsync();


        // 👉 On renvoie un DTO avec URL SAS (temporaire) pour le front
        var dto = doc.Adapt<AppelOffreDocumentDTO>();
        dto.Url = GenerateSasUrl(blobName, TimeSpan.FromMinutes(10));

        return dto;
    }


    public async Task<AppelOffreDocumentDTO?> DeleteDocumentAsync(Guid documentId)
    {
        // Récupérer le doc de la bdd
        var doc = await _context.DocumentsAppelOffre
            // ⚠️ "Ne pas suivre cet objet dans le DbContext"
            //         🔧 À quoi ça sert ?
            // ✅ Lecture seule (ex : vérification d’existence, affichage simple)
            // ✅ Performance améliorée (pas de tracking, donc moins de mémoire utilisée)
            // ❌ Mais tu ne peux pas modifier doc puis faire SaveChanges() — EF ne la suivra pas
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == documentId);

        if (doc is null) return null;

        // (Optionnel) Vérifier les droits : ex. seul RA ou le déposant peut supprimer
        // if (doc.DeposeParId != Guid.Parse(requesterId)) { throw new UnauthorizedAccessException(); }

        // Reconstituer le blobName (même logique que l’upload)
        var blobName = $"{doc.AppelOffreId}/{doc.TypeDocument}/{doc.NomFichier}";
        // Une fois le blobname recup je cree le client qui est enfait le doc en question.
        var blobClient = _blobContainerClient.GetBlobClient(blobName);

        // Supprimer le blob (si existe)
        await blobClient.DeleteIfExistsAsync();

        // Supprimer la ligne BDD
        _context.DocumentsAppelOffre.Remove(doc);
        await _context.SaveChangesAsync();

        return doc.Adapt<AppelOffreDocumentDTO>();
    }




    public string GenerateSasUrl(string blobName, TimeSpan duration)
    {
        // 1️⃣ On récupère une référence vers le blob précis (fichier) dans le container
        var blobClient = _blobContainerClient.GetBlobClient(blobName);

        // 2️⃣ On prépare un constructeur de SAS (BlobSasBuilder) qui contient :
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _blobContainerClient.Name, // nom du container Azure
            BlobName = blobClient.Name,                    // nom du blob (fichier)
            Resource = "b",                                // "b" = blob (fichier unique)
            ExpiresOn = DateTimeOffset.UtcNow.Add(duration) // date/heure d'expiration du lien
        };

        // 3️⃣ On donne les permissions du SAS → ici, uniquement "Read" (lecture seule)
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        // 4️⃣ On récupère la clé secrète du compte Azure
        //    - en prod, elle doit venir de la variable d'environnement
        //    - en dev, on peut la prendre depuis la configuration appsettings
        var sharedKey = Environment.GetEnvironmentVariable("AZURE_BLOB_ACCOUNT_KEY")
                        ?? _config["AzureBlobStorage:AccountKey"]; // dev uniquement

        // 5️⃣ On signe le SAS avec la clé du compte
        //    Cela crée une signature cryptée qui prouve à Azure que ce lien est valide
        var sasToken = sasBuilder.ToSasQueryParameters(
            new StorageSharedKeyCredential(_blobServiceClient.AccountName, sharedKey)
        ).ToString();

        // 6️⃣ On retourne l'URL complète : URL publique du blob + paramètres SAS
        return $"{blobClient.Uri}?{sasToken}";
    }



    // public async Task<Stream> DownloadAsync(string path)
    // {
    //     var blobClient = _blobContainerClient.GetBlobClient(path);
    //     var result = await blobClient.DownloadAsync();
    //     return result.Value.Content;
    // }
}
