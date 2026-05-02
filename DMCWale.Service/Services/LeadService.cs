using DMCWale.Data.Constants;
using DMCWale.Service.Interfaces;

namespace DMCWale.Service.Services;

public class LeadService(ICrmModuleService crmModuleService)
    : CrmCrudModuleService(crmModuleService, CrmModuleConstants.Lead), ILeadService;
