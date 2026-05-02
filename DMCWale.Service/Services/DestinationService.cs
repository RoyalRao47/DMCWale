using DMCWale.Data.Constants;
using DMCWale.Service.Interfaces;

namespace DMCWale.Service.Services;

public class DestinationService(ICrmModuleService crmModuleService)
    : CrmCrudModuleService(crmModuleService, CrmModuleConstants.Destination), IDestinationService;
