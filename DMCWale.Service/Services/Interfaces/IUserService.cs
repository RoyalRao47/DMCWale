using DMCWale.Data.Models;

namespace DMCWale.Service.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetUserProfileByAspNetUserIdAsync(string aspNetUserId);

    Task<bool> IsActiveAsync(string aspNetUserId);

    Task<bool> IsLeftAsync(string aspNetUserId);

    Task UpdateUserProfileAsync(User user);
}
