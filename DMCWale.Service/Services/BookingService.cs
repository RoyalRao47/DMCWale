using DMCWale.Data.Constants;
using DMCWale.Service.Interfaces;

namespace DMCWale.Service.Services;

public class BookingService(ICrmModuleService crmModuleService)
    : CrmCrudModuleService(crmModuleService, CrmModuleConstants.Booking), IBookingService;
