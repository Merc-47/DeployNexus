namespace DeployNexus.Application.Dashboard.DTOs;

public class DashboardSummaryDto
{
    public int Users { get; set; }

    public int Organizations { get; set; }

    public int Roles { get; set; }

    public int Permissions { get; set; }

    public int Modules { get; set; }
}