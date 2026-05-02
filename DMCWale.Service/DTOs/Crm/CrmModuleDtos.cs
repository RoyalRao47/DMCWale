namespace DMCWale.Service.DTOs.Crm;

public class CrmModuleDto
{
    public string ModuleCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public List<string> SupportedActions { get; set; } = [];
    public List<CrmFieldDto> Fields { get; set; } = [];
    public List<CrmRowDto> Rows { get; set; } = [];
}

public class CrmFieldDto
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string FieldType { get; set; } = "text";
    public string Value { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsReadOnly { get; set; }
    public List<CrmSelectOptionDto> Options { get; set; } = [];
}

public class CrmSelectOptionDto
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public class CrmRowDto
{
    public int Id { get; set; }
    public List<CrmCellDto> Cells { get; set; } = [];
}

public class CrmCellDto
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class CrmSaveRequestDto
{
    public string ModuleCode { get; set; } = string.Empty;
    public int? Id { get; set; }
    public Dictionary<string, string?> Values { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string? CurrentUserId { get; set; }
}

public class CrmDashboardDto
{
    public string Title { get; set; } = string.Empty;
    public List<CrmMetricDto> Metrics { get; set; } = [];
}

public class CrmMetricDto
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
