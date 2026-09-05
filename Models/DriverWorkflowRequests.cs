namespace MvcDriverVerify.Models;

public sealed record FeedbackSignalSubmission(
    string Category,
    int Severity,
    string Context,
    int? RelatedProfileId,
    string? RelatedEntity,
    string? SubmitterType);

public sealed record VerificationCaseSubmission(
    string CaseType,
    string RelationshipContext,
    int PrimaryProfileId,
    string? Counterparty,
    IReadOnlyList<DocumentEvidenceSubmission>? Evidence,
    IReadOnlyList<CounterpartyConfirmationSubmission>? Confirmations);

public sealed record DocumentEvidenceSubmission(
    string DocumentType,
    string FileName,
    string? ContentType,
    long? SizeBytes);

public sealed record CounterpartyConfirmationSubmission(
    string Counterparty,
    string Claim,
    string? State);

public sealed record WorkflowSubmissionResult(
    bool Accepted,
    string Message);

public sealed record ProfileUpdateSubmission(
    int UserId,
    string Name,
    decimal Rating,
    int VehicleId,
    int PartnerId,
    int UserTypeId);

public sealed record VehicleUpdateSubmission(
    int VehicleId,
    string Registration,
    string Make,
    string ModelName,
    string ModelYear,
    int PlatformId,
    int PartnerId);

public sealed record RelationshipUpdateSubmission(
    string RelationshipId,
    string VerificationStatus,
    string AvailabilityStatus);

public sealed record VerificationRulesViewModel(
    string ProfileType,
    IReadOnlyList<string> AllowedCaseTypes,
    IReadOnlyList<string> RequiredEvidenceTypes,
    string Guidance);
