public interface IUserService
{
    public Task editProfile(UpdateUserProfileDto payload, Guid userId);
}