using DMCWale.Service.Models.Common;
using DMCWale.Data.Models.Identity;
using DMCWale.Service.ViewModels.Account;

namespace DMCWale.Service.Services.Interfaces;

public interface IAccountService
{
    Task<ServiceResult> LoginAsync(LoginViewModel model);

    Task LogoutAsync();

    Task<string> GetDashboardUrlByRoleAsync(ApplicationUser user);
}
