using DMCWale.Data.Constants;
using DMCWale.Data.Models;
using DMCWale.Service.Models.Common;
using DMCWale.Data.Models.Identity;
using DMCWale.Repo.Interfaces;
using DMCWale.Service.Services.Interfaces;
using DMCWale.Service.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DMCWale.Service.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<AccountService> _logger;

    public AccountService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IRepository<User> userRepository,
        ILogger<AccountService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ServiceResult> LoginAsync(LoginViewModel model)
    {
        var login = model.UsernameOrEmail.Trim();
        var applicationUser = login.Contains('@')
            ? await _userManager.FindByEmailAsync(login)
            : await _userManager.FindByNameAsync(login);

        if (applicationUser is null)
        {
            return ServiceResult.Failure("Invalid username/email or password.");
        }

        var userProfile = await _userRepository
            .AsQueryable()
            .FirstOrDefaultAsync(user => user.AspNetUserId == applicationUser.Id);

        if (userProfile is null)
        {
            _logger.LogWarning("Login rejected because profile is missing for AspNetUserId {AspNetUserId}.", applicationUser.Id);
            return ServiceResult.Failure("Invalid username/email or password.");
        }

        if (!userProfile.IsActive)
        {
            return ServiceResult.Failure("Your account is inactive. Please contact the administrator.");
        }

        if (userProfile.IsLeft)
        {
            return ServiceResult.Failure("Your account is not allowed to sign in. Please contact the administrator.");
        }

        var signInResult = await _signInManager.PasswordSignInAsync(
            applicationUser,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);

        if (!signInResult.Succeeded)
        {
            return ServiceResult.Failure("Invalid username/email or password.");
        }

        var redirectUrl = await GetDashboardUrlByRoleAsync(applicationUser);
        return ServiceResult.Success("Login successful.", redirectUrl);
    }

    public Task LogoutAsync()
    {
        return _signInManager.SignOutAsync();
    }

    public async Task<string> GetDashboardUrlByRoleAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains(RoleConstants.Admin))
        {
            return "/Dashboard/Admin";
        }

        if (roles.Contains(RoleConstants.Agent))
        {
            return "/Dashboard/Agent";
        }

        if (roles.Contains(RoleConstants.Staff))
        {
            return "/Dashboard/Staff";
        }

        if (roles.Contains(RoleConstants.Supplier))
        {
            return "/Dashboard/Supplier";
        }

        return "/Dashboard/Index";
    }
}
