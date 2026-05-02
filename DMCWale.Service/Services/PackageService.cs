using DMCWale.Data.Constants;
using DMCWale.Service.Interfaces;

namespace DMCWale.Service.Services;

public class PackageService(ICrmModuleService crmModuleService)
    : CrmCrudModuleService(crmModuleService, CrmModuleConstants.Package), IPackageService;
