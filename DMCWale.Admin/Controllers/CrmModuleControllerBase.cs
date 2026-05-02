using System.Security.Claims;
using DMCWale.Admin.ViewModels.Crm;
using DMCWale.Data.Constants;
using DMCWale.Service.DTOs.Crm;
using DMCWale.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMCWale.Admin.Controllers;

[Authorize]
public abstract class CrmModuleControllerBase : Controller
{
    protected static TViewModel Map<TViewModel>(CrmModuleDto dto)
        where TViewModel : CrmModuleViewModel, new()
    {
        return new TViewModel
        {
            ModuleCode = dto.ModuleCode,
            Title = dto.Title,
            Controller = dto.Controller,
            SupportedActions = dto.SupportedActions.ToList(),
            Fields = dto.Fields.Select(field => new CrmFieldViewModel
            {
                Name = field.Name,
                Label = field.Label,
                FieldType = field.FieldType,
                Value = field.Value,
                IsRequired = field.IsRequired,
                IsReadOnly = field.IsReadOnly,
                Options = field.Options.Select(option => new CrmSelectOptionViewModel
                {
                    Value = option.Value,
                    Text = option.Text
                }).ToList()
            }).ToList(),
            Rows = dto.Rows.Select(row => new CrmRowViewModel
            {
                Id = row.Id,
                Cells = row.Cells.Select(cell => new CrmCellViewModel
                {
                    Label = cell.Label,
                    Value = cell.Value
                }).ToList()
            }).ToList()
        };
    }

    protected static TViewModel FeatureModel<TViewModel>(
        string moduleCode,
        string title,
        string controller)
        where TViewModel : CrmModuleViewModel, new()
    {
        return new TViewModel
        {
            ModuleCode = moduleCode,
            Title = title,
            Controller = controller
        };
    }

    protected Task<bool> CanAsync(
        ICrmCrudModuleService service,
        string permissionCode,
        CancellationToken cancellationToken)
    {
        return service.HasPermissionAsync(User, permissionCode, cancellationToken);
    }

    protected CrmSaveRequestDto ToSaveRequest(CrmModuleViewModel model)
    {
        return new CrmSaveRequestDto
        {
            ModuleCode = model.ModuleCode,
            CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Values = model.Fields.ToDictionary(
                field => field.Name,
                field => (string?)field.Value,
                StringComparer.OrdinalIgnoreCase)
        };
    }

    protected void AddErrors(IEnumerable<string> errors)
    {
        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }
    }

    protected async Task<TViewModel> RebuildFormAsync<TViewModel>(
        ICrmCrudModuleService service,
        CrmModuleViewModel postedModel,
        int? id = null,
        CancellationToken cancellationToken = default)
        where TViewModel : CrmModuleViewModel, new()
    {
        var rebuilt = Map<TViewModel>(await service.GetFormAsync(id, cancellationToken));
        var postedValues = postedModel.Fields.ToDictionary(
            field => field.Name,
            field => field.Value,
            StringComparer.OrdinalIgnoreCase);

        foreach (var field in rebuilt.Fields)
        {
            if (postedValues.TryGetValue(field.Name, out var value))
            {
                field.Value = value;
            }
        }

        if (id.HasValue)
        {
            rebuilt.Rows = [new CrmRowViewModel { Id = id.Value }];
        }

        return rebuilt;
    }
}
