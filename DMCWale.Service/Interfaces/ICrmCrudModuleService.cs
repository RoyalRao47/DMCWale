using System.Security.Claims;
using DMCWale.Service.DTOs.Crm;
using DMCWale.Service.Models.Common;

namespace DMCWale.Service.Interfaces;

public interface ICrmCrudModuleService
{
    string ModuleCode { get; }

    Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permissionCode, CancellationToken cancellationToken = default);

    Task<CrmModuleDto> GetListAsync(CancellationToken cancellationToken = default);

    Task<CrmModuleDto> GetFormAsync(int? id = null, CancellationToken cancellationToken = default);

    Task<CrmModuleDto> GetDetailsAsync(int id, CancellationToken cancellationToken = default);

    Task<ServiceResult> SaveAsync(CrmSaveRequestDto request, CancellationToken cancellationToken = default);

    Task<ServiceResult> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
}
