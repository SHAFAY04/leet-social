

using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

public class S3Service : IS3Service
{
    AmazonS3Client s3client;
    string _bucketName;

    public S3Service()
    {
        s3client = new AmazonS3Client(
        new BasicAWSCredentials(
            Environment.GetEnvironmentVariable("R2_ACCESS_KEY_ID"),
            Environment.GetEnvironmentVariable("R2_SECRET_ACCESS_KEY")
        ),
        new AmazonS3Config
        {
            ServiceURL = Environment.GetEnvironmentVariable("R2_ENDPOINT"),
            ForcePathStyle = true
        }
    );

        _bucketName = Environment.GetEnvironmentVariable("R2_BUCKET_NAME");

    }

    public async Task uploadFileToS3(string fileName, string filePath, Stream stream, string ContentType)
    {

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName,
            FilePath = filePath,
            InputStream = stream,
            ContentType = ContentType,
        };
        request.Headers.CacheControl = "public, max-age=31536000";
        await s3client.PutObjectAsync(request);
    }
    public async Task<string> getPresignedUrl(string filePath)
    {
        await s3client.GetPreSignedURLAsync(
            new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = filePath,
                Expires = DateTime.UtcNow.AddMinutes(15)
            }
        );

        return "blah";
    }
}