using DMCWale.Data.Models;
using DMCWale.Repo.Interfaces;
using DMCWale.Service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DMCWale.Service.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<User?> GetUserProfileByAspNetUserIdAsync(string aspNetUserId)
    {
        return _userRepository
            .AsQueryable()
            .FirstOrDefaultAsync(user => user.AspNetUserId == aspNetUserId);
    }

    public async Task<bool> IsActiveAsync(string aspNetUserId)
    {
        return await _userRepository
            .AsQueryable()
            .AnyAsync(user => user.AspNetUserId == aspNetUserId && user.IsActive);
    }

    public async Task<bool> IsLeftAsync(string aspNetUserId)
    {
        return await _userRepository
            .AsQueryable()
            .AnyAsync(user => user.AspNetUserId == aspNetUserId && user.IsLeft);
    }

    public async Task UpdateUserProfileAsync(User user)
    {
        user.ModifyDate = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
    }
}
