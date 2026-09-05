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
