using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CapsuleApi.src.Services
{
    public class FileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string? _bucketName;
        private readonly string? _endpoint;

        public string BucketName => _bucketName!;
        public string Endpoint => _endpoint!;

        public async Task DeleteFileAsync(string key)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
        }

        public FileStorageService(IConfiguration config)
        {
            var settings = config.GetSection("CloudflareR2");
            _bucketName = settings["Bucket"];
            _endpoint = settings["Endpoint"];

            var accessKey = settings["AccessKeyId"];
            var secretKey = settings["SecretAccessKey"];

            var configS3 = new AmazonS3Config
            {
                ServiceURL = _endpoint,
                ForcePathStyle = true,
                SignatureVersion = "4"
            };

            _s3Client = new AmazonS3Client(accessKey, secretKey, configS3);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            var fileName = $"{folder}/{Guid.NewGuid()}_{file.FileName}";
            using var stream = file.OpenReadStream();

            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                InputStream = stream,
                ContentType = file.ContentType,
                AutoCloseStream = true,
                DisablePayloadSigning = true
            };

            await _s3Client.PutObjectAsync(putRequest);

            return $"{_endpoint}/{_bucketName}/{fileName}";
        }
    }
}
