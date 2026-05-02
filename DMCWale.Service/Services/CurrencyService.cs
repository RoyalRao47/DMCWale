using DMCWale.Data.Constants;
using DMCWale.Service.Interfaces;

namespace DMCWale.Service.Services;

public class CurrencyService(ICrmModuleService crmModuleService)
    : CrmCrudModuleService(crmModuleService, CrmModuleConstants.Currency), ICurrencyService;
