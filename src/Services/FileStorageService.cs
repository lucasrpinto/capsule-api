// src/Services/FileStorageService.cs
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;

namespace CapsuleApi.src.Services;

public class FileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public FileStorageService(IConfiguration config)
    {
        var settings = config.GetSection("CloudFlareR2");
        _bucketName = settings["Bucket"];

        var accessKey = settings["AccessKeyId"];
        var secretKey = settings["SecretAccessKey"];
        var endpoint = settings["Endpoint"];

        // Configuração do cliente S3 com URL da Cloudflare e forçando estilo de path
        var configS3 = new AmazonS3Config
        {
            ServiceURL = endpoint,
            ForcePathStyle = true,
            SignatureVersion = "4"
        };

        _s3Client = new AmazonS3Client(accessKey, secretKey, configS3);
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folder)
    {
        var fileName = $"{folder}/{Guid.NewGuid()}_{file.FileName}";

        using var stream = file.OpenReadStream();
        stream.Position = 0; // Garante que a leitura começa do início

        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName,
            InputStream = stream,
            ContentType = file.ContentType,
            AutoCloseStream = true,
            Headers = { ["x-amz-content-sha256"] = "UNSIGNED-PAYLOAD" }
        };

        await _s3Client.PutObjectAsync(putRequest);

        // Retorna a URL de acesso ao arquivo
        return $"{_s3Client.Config.ServiceURL}/{_bucketName}/{fileName}";
    }
}
