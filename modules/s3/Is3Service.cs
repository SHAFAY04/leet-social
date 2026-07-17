

public interface IS3Service
{
    
    public Task uploadFileToS3(string fileName, string filePath, Stream stream,string ContentType);
    public Task<string> getPresignedUrl(string filePath);
}