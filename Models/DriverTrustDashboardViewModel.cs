namespace MvcDriverVerify.Models;

public sealed class DriverTrustDashboardViewModel
{
    public bool ApiConnected { get; init; }
    public string ApiStatus { get; init; } = "Using preview trust signals until the API is available.";
    public IReadOnlyList<DriverTrustCard> Drivers { get; init; } = [];
    public ModerationQueueSummary ModerationQueue { get; init; } = new();
    public RelationshipSummary Relationships { get; init; } = new();
    public TrustSignalSummary AdminTrustSignals { get; init; } = new();
    public SeedCoverageSummary AdminSeedCoverage { get; init; } = new();
}

public sealed class ModerationQueueSummary
{
    public int PendingFeedback { get; init; }
    public int VerificationCases { get; init; }
    public int DuplicateProfiles { get; init; }
    public int SuspiciousActivity { get; init; }
    public string Status { get; init; } = "Moderation queue not loaded yet.";
}

public sealed class RelationshipSummary
{
    public int TotalRelationships { get; init; }
    public int AvailableRelationships { get; init; }
    public int VerifiedRelationships { get; init; }
    public IReadOnlyList<string> RelationshipTypes { get; init; } = [];
    public IReadOnlyList<RelationshipWorkspaceCard> CurrentUserRelationships { get; init; } = [];
    public string Status { get; init; } = "Relationship seed data not loaded yet.";
}

public sealed class RelationshipWorkspaceCard
{
    public string RelationshipId { get; init; } = string.Empty;
    public string RelationshipType { get; init; } = "Relationship";
    public string VerificationStatus { get; init; } = "Pending";
    public string AvailabilityStatus { get; init; } = "Available";
    public int DriverUserId { get; init; }
    public int? OwnerUserId { get; init; }
    public int? VehicleId { get; init; }
    public int? PlatformId { get; init; }
    public int? PartnerId { get; init; }
}

public sealed class TrustSignalSummary
{
    public int ProfilesMonitored { get; init; }
    public int ReviewRisk { get; init; }
    public int HighRisk { get; init; }
    public int AverageTrustScore { get; init; }
}

public sealed class SeedCoverageSummary
{
    public int TotalRelationships { get; init; }
    public int AvailableRelationships { get; init; }
    public int VerifiedRelationships { get; init; }
    public IReadOnlyList<string> RelationshipTypes { get; init; } = [];
}

public sealed class DriverTrustCard
{
    public int UserId { get; init; }
    public int? VehicleId { get; init; }
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