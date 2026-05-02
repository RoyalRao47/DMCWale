using System.Security.Claims;
using DMCWale.Service.DTOs.Crm;
using DMCWale.Service.Models.Common;

namespace DMCWale.Service.Interfaces;

public interface ICrmModuleService
{
    Task<bool> HasPermissionAsync(ClaimsPrincipal user, string moduleCode, string permissionCode, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CrmModuleDto>> GetMenuModulesAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);

    Task<CrmModuleDto> GetListAsync(string moduleCode, CancellationToken cancellationToken = default);

    Task<CrmModuleDto> GetFormAsync(string moduleCode, int? id = null, CancellationToken cancellationToken = default);

    Task<CrmModuleDto> GetDetailsAsync(string moduleCode, int id, CancellationToken cancellationToken = default);

    Task<ServiceResult> SaveAsync(CrmSaveRequestDto request, CancellationToken cancellationToken = default);

    Task<ServiceResult> SoftDeleteAsync(string moduleCode, int id, CancellationToken cancellationToken = default);

    Task<CrmDashboardDto> GetDashboardAsync(string roleName, CancellationToken cancellationToken = default);
}
