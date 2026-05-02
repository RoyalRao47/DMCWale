namespace DMCWale.Admin.ViewModels.Crm;

public class CrmModuleViewModel
{
    public string ModuleCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public List<string> SupportedActions { get; set; } = [];
    public List<CrmFieldViewModel> Fields { get; set; } = [];
    public List<CrmRowViewModel> Rows { get; set; } = [];
}

public class CrmFieldViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string FieldType { get; set; } = "text";
    public string Value { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsReadOnly { get; set; }
    public List<CrmSelectOptionViewModel> Options { get; set; } = [];
}

public class CrmSelectOptionViewModel
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public class CrmRowViewModel
{
    public int Id { get; set; }
    public List<CrmCellViewModel> Cells { get; set; } = [];
}

public class CrmCellViewModel
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class CrmDashboardViewModel
{
    public string Title { get; set; } = string.Empty;
    public List<CrmMetricViewModel> Metrics { get; set; } = [];
}

public class CrmMetricViewModel
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
