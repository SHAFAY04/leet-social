

using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{

    IMediaService _mediaService;
    AppDbContext _dbcontext;

    public UserService(IMediaService mediaService, AppDbContext dbContext)
    {
        _mediaService = mediaService;
        _dbcontext = dbContext;
    }

    public async Task editProfile(UpdateUserProfileDto payload, Guid userId)
    {
        var userRecord = await _dbcontext.Users.FirstOrDefaultAsync(a => a.Id == userId);
        
        if (userRecord != null)
        {

            if (payload.image != null)
            {
                MediaEntity media = await _mediaService.CreateMedia(payload.image, userId,mediaCategory:MediaCategory.ProfilePicture);
                userRecord.profileMedia=media;

            }


            if (payload.bio != null)
            {
                userRecord.bio = payload.bio;
            }
            if (payload.username != null)
            {
                userRecord.username = payload.username;
            }
            if (payload.pronouns != null)
            {
                userRecord.pronouns = payload.pronouns;
            }
            
            userRecord.UpdatedAt=DateTime.UtcNow;
            await _dbcontext.SaveChangesAsync();
        }
        else
        {
            throw new BadRequestException("User doesnt exist");
        }
    }
}