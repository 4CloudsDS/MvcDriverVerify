namespace MvcDriverVerify.Models;

public sealed class MeDashboardViewModel
{
    public string DisplayName { get; init; } = "Demo user";
    public string RoleScope { get; init; } = "Public";
    public DriverTrustCard? PublicProfile { get; init; }
    public UserProfileEditor ProfileEditor { get; init; } = new();
    public IReadOnlyList<VehicleWorkspaceCard> Vehicles { get; init; } = [];
    public IReadOnlyList<RelationshipRequestCard> RelationshipRequests { get; init; } = [];
    public RelationshipSummary Relationships { get; init; } = new();
    public string Status { get; init; } = "Personal workspace loaded.";
}

public sealed class UserProfileEditor
{
    public int UserId { get; init; }
    public string Name { get; init; } = "Demo user";
    public string UserType { get; init; } = "Public";
    public decimal Rating { get; init; }
    public int VehicleId { get; init; }
    public int PartnerId { get; init; }
    public int UserTypeId { get; init; }
}

public sealed class VehicleWorkspaceCard
{
    public int VehicleId { get; init; }
    public string Registration { get; init; } = "Registration pending";
    public string Make { get; init; } = string.Empty;
    public string ModelName { get; init; } = string.Empty;
    public string ModelYear { get; init; } = string.Empty;
    public string Description { get; init; } = "Vehicle pending";
    public int PlatformId { get; init; } = 1;
    public string Platform { get; init; } = "Platform pending";
    public int PartnerId { get; init; } = 10;
    public string Partner { get; init; } = "Partner pending";
    public bool IsOwnedByCurrentUser { get; init; }
    public string DriverLinkStatus { get; init; } = "Driver link requires approved counterparty claim.";
}

public sealed class RelationshipRequestCard
{
    public Guid CaseId { get; init; }
    public string CaseType { get; init; } = "Relationship verification";
    public string RelationshipContext { get; init; } = "Relationship";
    public int PrimaryProfileId { get; init; }
    public string PrimaryProfileName { get; init; } = "Profile pending";
    public string Counterparty { get; init; } = "Counterparty pending";
    public string Status { get; init; } = "Draft";
    public string PrivacyStatus { get; init; } = "PrivateDocuments";
    public DateTimeOffset UpdatedAtUtc { get; init; }
    public IReadOnlyList<string> ConfirmationClaims { get; init; } = [];
}
