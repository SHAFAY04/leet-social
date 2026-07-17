

using Microsoft.EntityFrameworkCore;

public enum FileType
{
    Pdf,
    Jpg,
    Jpeg,
    Png,
    Gif,
    Mp4
}
public class MediaEntity:BaseEntity
{
    
    public string fileName { get; set; }
    public string filePath { get; set; }
    public FileType FileType { get; set; }

}