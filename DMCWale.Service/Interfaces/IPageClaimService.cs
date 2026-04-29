using DMCWale.Service.DTOs.PageClaims;
using DMCWale.Service.Models.Common;

namespace DMCWale.Service.Interfaces;

public interface IPageClaimService
{
    Task<PageClaimMatrixDto> GetPageClaimsAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult> UpdatePageClaimsAsync(UpdatePageClaimsRequestDto request, CancellationToken cancellationToken = default);
}
