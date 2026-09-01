namespace MvcDriverVerify.Models;

public sealed class DriverTrustDashboardViewModel
{
    public bool ApiConnected { get; init; }
    public string ApiStatus { get; init; } = "Using preview trust signals until the API is available.";
    public IReadOnlyList<DriverTrustCard> Drivers { get; init; } = [];
}

public sealed class DriverTrustCard
{
    public string Initials { get; init; } = "VD";
    public string Name { get; init; } = "Unknown driver";
    public string Category { get; init; } = "Driver";
    public string Region { get; init; } = "Region pending";
    public string VehicleRegistration { get; init; } = "Registration pending";
    public string VehicleDescription { get; init; } = "Vehicle pending";
    public string PartnerName { get; init; } = "Partner pending";
    public string UserType { get; init; } = "Unclassified";
    public decimal Rating { get; init; }
    public int TrustScore { get; init; }
    public int ReportCount { get; init; }
    public string RiskLevel { get; init; } = "Review";
    public string ActivitySummary { get; init; } = "No recent activity has been loaded yet.";
    public IReadOnlyList<string> Signals { get; init; } = [];
}