

public interface IMediaService
{
    
    public Task<MediaEntity> CreateMedia(IFormFile file,Guid userId,MediaCategory mediaCategory);
    public string GetMediaUrl(string filePath);
}