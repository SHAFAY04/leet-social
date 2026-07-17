

using Microsoft.EntityFrameworkCore;

public class mediaService : IMediaService
{

    AppDbContext _dbcontext;
    IS3Service _s3Service;
    public mediaService(AppDbContext dbContext, IS3Service s3Service)
    {
        _dbcontext = dbContext;
        _s3Service = s3Service;
    }
    public async Task<MediaEntity> CreateMedia(IFormFile file, Guid userId, MediaCategory mediaCategory)
    {

        var maxSize = mediaCategory switch
        {
            MediaCategory.ProfilePicture=> 5 * 1024 * 1024,
            MediaCategory.Post=>10 * 1024 * 1024,
            MediaCategory.VideoPost=>50 * 1024 * 1024
        };

        if (file.Length > maxSize)
        {
            throw new BadRequestException($"Max supported size for {mediaCategory.ToString()} is {maxSize}");
        }

        var extension = Path.GetExtension(file.FileName).ToLower();
        FileType fileType = extension switch
        {
            ".pdf" => FileType.Pdf,
            ".jpg" => FileType.Jpg,
            ".jpeg" => FileType.Jpeg,
            ".png" => FileType.Png,
            ".gif" => FileType.Gif,
            ".mp4" => FileType.Mp4,
            _ => throw new BadRequestException("Unsupported file type")
        };

        string uniqueFileName = Guid.NewGuid() + "-" + file.FileName;
        string filePath = userId + "/" + mediaCategory.ToString() + "/" + uniqueFileName;
        Stream stream = file.OpenReadStream();

        //first putting it upto s3 so that if it fails we dont have a fake record in db
        await _s3Service.uploadFileToS3(uniqueFileName, filePath, stream, file.ContentType);

        var newMedia = new MediaEntity
        {
            fileName = uniqueFileName,
            filePath = filePath,
            FileType = fileType
        };

        _dbcontext.Media.Add(newMedia);
        await _dbcontext.SaveChangesAsync();

        return newMedia;
    }
    public string GetMediaUrl(string filePath)
    {
        string pubDevUrl = Environment.GetEnvironmentVariable("R2_PUBLIC_URL")!;

        string publicMediaUrl = $"{pubDevUrl}/{filePath}";

        return publicMediaUrl;
    }
}