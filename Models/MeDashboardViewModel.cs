namespace MvcDriverVerify.Models;

public sealed class MeDashboardViewModel
{
    public string DisplayName { get; init; } = "Demo user";
    public string RoleScope { get; init; } = "Public";
    public DriverTrustCard? PublicProfile { get; init; }
    public IReadOnlyList<RelationshipRequestCard> RelationshipRequests { get; init; } = [];
    public RelationshipSummary Relationships { get; init; } = new();
    public string Status { get; init; } = "Personal workspace loaded.";
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
