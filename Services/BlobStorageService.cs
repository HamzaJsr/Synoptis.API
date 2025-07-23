using Azure.Storage.Blobs;

public class BlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;//Client
    private readonly BlobContainerClient _blobContainerClient; //Container


    public BlobStorageService(BlobServiceClient blobServiceClient, IConfiguration config)
    {
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

    public async Task UploadAsync(IFormFile file, string clientId, string category)
    {
        var blobName = $"{clientId}/{category}/{file.FileName}";

        var blobClient = _blobContainerClient.GetBlobClient(blobName);

        using var stream = file.OpenReadStream();

        await blobClient.UploadAsync(stream, overwrite: true);
    }

    public async Task<Stream> DownloadAsync(string path)
    {
        var blobClient = _blobContainerClient.GetBlobClient(path);
        var result = await blobClient.DownloadAsync();
        return result.Value.Content;
    }
}
