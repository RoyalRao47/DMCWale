using System.Security.Claims;
using DMCWale.Service.DTOs.Crm;
using DMCWale.Service.Interfaces;
using DMCWale.Service.Models.Common;

namespace DMCWale.Service.Services;

public abstract class CrmCrudModuleService : ICrmCrudModuleService
{
    private readonly ICrmModuleService _crmModuleService;

    protected CrmCrudModuleService(ICrmModuleService crmModuleService, string moduleCode)
    {
        _crmModuleService = crmModuleService;
        ModuleCode = moduleCode;
    }

    public string ModuleCode { get; }

    public Task<bool> HasPermissionAsync(
        ClaimsPrincipal user,
        string permissionCode,
        CancellationToken cancellationToken = default)
    {
        return _crmModuleService.HasPermissionAsync(user, ModuleCode, permissionCode, cancellationToken);
    }

    public Task<CrmModuleDto> GetListAsync(CancellationToken cancellationToken = default)
    {
        return _crmModuleService.GetListAsync(ModuleCode, cancellationToken);
    }

    public Task<CrmModuleDto> GetFormAsync(int? id = null, CancellationToken cancellationToken = default)
    {
        return _crmModuleService.GetFormAsync(ModuleCode, id, cancellationToken);
    }

    public Task<CrmModuleDto> GetDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return _crmModuleService.GetDetailsAsync(ModuleCode, id, cancellationToken);
    }

    public Task<ServiceResult> SaveAsync(CrmSaveRequestDto request, CancellationToken cancellationToken = default)
    {
        request.ModuleCode = ModuleCode;
        return _crmModuleService.SaveAsync(request, cancellationToken);
    }

    public Task<ServiceResult> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return _crmModuleService.SoftDeleteAsync(ModuleCode, id, cancellationToken);
    }
}
