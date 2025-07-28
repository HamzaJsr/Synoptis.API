using Azure.Core;
using Azure.Storage.Blobs;
using Mapster;
using Synoptis.API.Data;
using Synoptis.API.DTOs;
using Synoptis.API.Models;
using Synoptis.API.Services.Interfaces;

public class BlobStorageService : IBlobStorageService
{
    private readonly SynoptisDbContext _context;
    private readonly BlobServiceClient _blobServiceClient;//Client
    private readonly BlobContainerClient _blobContainerClient; //Container


    public BlobStorageService(SynoptisDbContext context, BlobServiceClient blobServiceClient, IConfiguration config)
    {
        _context = context;
        _blobServiceClient = blobServiceClient;//Client
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

        return doc.Adapt<AppelOffreDocumentDTO>();
    }




    public async Task<Stream> DownloadAsync(string path)
    {
        var blobClient = _blobContainerClient.GetBlobClient(path);
        var result = await blobClient.DownloadAsync();
        return result.Value.Content;
    }
}
