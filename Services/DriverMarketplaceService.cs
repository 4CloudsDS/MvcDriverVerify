using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MvcDriverVerify.Models;

namespace MvcDriverVerify.Services;

public sealed class DriverMarketplaceService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<DriverMarketplaceService> _logger;

    public DriverMarketplaceService(HttpClient httpClient, ILogger<DriverMarketplaceService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DriverTrustDashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken)
    {
        try
        {
            var users = await _httpClient.GetFromJsonAsync<List<ApiUserDto>>("api/Users", JsonOptions, cancellationToken);
            var drivers = users?.Select(MapDriver).ToList() ?? [];

            if (drivers.Count == 0)
            {
                return new DriverTrustDashboardViewModel
                {
                    ApiConnected = true,
                    ApiStatus = "API connected. No driver profiles were returned yet.",
                    Drivers = PreviewDrivers()
                };
            }

            return new DriverTrustDashboardViewModel
            {
                ApiConnected = true,
                ApiStatus = $"Live API connected. Showing {drivers.Count} driver profile{(drivers.Count == 1 ? string.Empty : "s")} from VerifyDriverAPI.",
                Drivers = drivers
            };
        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
        {
            _logger.LogWarning(ex, "VerifyDriverAPI responded with an unsuccessful driver profile status code.");

            return new DriverTrustDashboardViewModel
            {
                ApiConnected = false,
                ApiStatus = $"API reachable, but the driver profile endpoint returned {(int)ex.StatusCode.Value}. Showing preview trust signals.",
                Drivers = PreviewDrivers()
            };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogWarning(ex, "Could not load driver profiles from VerifyDriverAPI.");

            return new DriverTrustDashboardViewModel
            {
                ApiConnected = false,
                ApiStatus = "API unavailable. Showing preview trust signals until VerifyDriverAPI responds.",
                Drivers = PreviewDrivers()
            };
        }
    }

    public Task<DriverTrustDashboardViewModel> SearchDashboardAsync(string? query, CancellationToken cancellationToken)
    {
        return SearchDashboardAsync(query, null, null, null, cancellationToken);
    }

    public async Task<DriverTrustDashboardViewModel> SearchDashboardAsync(string? query, string? mode, string? intent, string? relationshipType, CancellationToken cancellationToken)
    {
        var cleanQuery = query?.Trim();
        var cleanMode = string.IsNullOrWhiteSpace(mode) ? "profile" : mode.Trim();
        var cleanIntent = intent?.Trim();
        var cleanRelationshipType = relationshipType?.Trim();

        if (string.IsNullOrWhiteSpace(cleanQuery))
        {
            return WithStatus(false, cleanMode.Equals("opportunity", StringComparison.OrdinalIgnoreCase)
                ? "Choose your relationship goal, then search for a vehicle, platform, fleet, licence type, or role."
                : "Enter a person, driver name, vehicle registration, partner, platform, or region to verify.", []);
        }

        try
        {
            var url = $"api/Profiles/search?{BuildSearchQuery(cleanQuery, cleanMode, cleanIntent, cleanRelationshipType)}";
            var search = await _httpClient.GetFromJsonAsync<ApiProfileSearchResponse>(url, JsonOptions, cancellationToken);
            var matches = search?.Results.Select(MapTrustProfile).ToList() ?? [];

            return WithStatus(
                true,
                matches.Count == 0
                    ? NoMatchStatus(cleanQuery, cleanMode, cleanIntent, cleanRelationshipType)
                    : MatchStatus(matches.Count, cleanQuery, cleanMode, cleanIntent, cleanRelationshipType),
                matches);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogWarning(ex, "Could not search driver profiles through VerifyDriverAPI. Falling back to preview search.");

            var previewMatches = PreviewDrivers()
                .Where(driver => Matches(driver, cleanQuery, cleanMode))
                .ToList();

            return WithStatus(
                false,
                previewMatches.Count == 0
                    ? NoMatchStatus(cleanQuery, cleanMode, cleanIntent, cleanRelationshipType)
                    : $"API search unavailable. Showing {previewMatches.Count} preview match{(previewMatches.Count == 1 ? string.Empty : "es")} for \"{cleanQuery}\".",
                previewMatches);
        }

        static DriverTrustDashboardViewModel WithStatus(bool apiConnected, string status, IReadOnlyList<DriverTrustCard> drivers) => new()
        {
            ApiConnected = apiConnected,
            ApiStatus = status,
            Drivers = drivers
        };
    }

    public async Task<WorkflowSubmissionResult> SubmitFeedbackAsync(FeedbackSignalSubmission request, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync("api/Feedback", request, JsonOptions, cancellationToken);
            return new WorkflowSubmissionResult(
                response.IsSuccessStatusCode,
                response.IsSuccessStatusCode
                    ? "Signal submitted to VerifyDriverAPI moderation queue."
                    : $"VerifyDriverAPI rejected the signal with status {(int)response.StatusCode}.");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogWarning(ex, "Could not submit feedback signal to VerifyDriverAPI.");
            return new WorkflowSubmissionResult(false, "Feedback API unavailable. Save the details and retry when VerifyDriverAPI is running.");
        }
    }

    public async Task<WorkflowSubmissionResult> CreateVerificationCaseAsync(VerificationCaseSubmission request, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync("api/VerificationCases", request, JsonOptions, cancellationToken);
            return new WorkflowSubmissionResult(
                response.IsSuccessStatusCode,
                response.IsSuccessStatusCode
                    ? "Verification case created in VerifyDriverAPI."
                    : $"VerifyDriverAPI rejected the verification case with status {(int)response.StatusCode}.");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogWarning(ex, "Could not create verification case in VerifyDriverAPI.");
            return new WorkflowSubmissionResult(false, "Verification case API unavailable. Keep the evidence packet and retry when VerifyDriverAPI is running.");
        }
    }

    public async Task<ModerationQueueSummary> GetModerationQueueAsync(CancellationToken cancellationToken)
    {
        try
        {
            var queue = await _httpClient.GetFromJsonAsync<ApiModerationQueueDto>("api/Moderation/queue", JsonOptions, cancellationToken);

            return new ModerationQueueSummary
            {
                PendingFeedback = queue?.Feedback.Count ?? 0,
                VerificationCases = queue?.VerificationCases.Count ?? 0,
                DuplicateProfiles = queue?.DuplicateProfiles.Count ?? 0,
                SuspiciousActivity = queue?.SuspiciousActivity.Count ?? 0,
                Status = "Moderation queue loaded from VerifyDriverAPI."
            };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogWarning(ex, "Could not load moderation queue from VerifyDriverAPI.");

            return new ModerationQueueSummary
            {
                Status = "Moderation queue unavailable until VerifyDriverAPI is running."
            };
        }
    }

    public async Task<RelationshipSummary> GetRelationshipSummaryAsync(CancellationToken cancellationToken)
    {
        try
        {
            var relationships = await _httpClient.GetFromJsonAsync<List<ApiRelationshipDto>>("api/Relationships", JsonOptions, cancellationToken) ?? [];
            var types = relationships
                .Select(item => item.RelationshipType)
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item)
                .ToList();

            return new RelationshipSummary
            {
                TotalRelationships = relationships.Count,
                AvailableRelationships = relationships.Count(item => item.AvailabilityStatus.Equals("Available", StringComparison.OrdinalIgnoreCase)),
                VerifiedRelationships = relationships.Count(item => item.VerificationStatus.Equals("Verified", StringComparison.OrdinalIgnoreCase)),
                RelationshipTypes = types,
                Status = "Relationship seed data loaded from VerifyDriverAPI."
            };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogWarning(ex, "Could not load relationship seed data from VerifyDriverAPI.");

            return new RelationshipSummary
            {
                Status = "Relationship seed data unavailable until VerifyDriverAPI is running."
            };
        }
    }

    public async Task<MeDashboardViewModel> GetMeDashboardAsync(CancellationToken cancellationToken)
    {
        var dashboard = await GetDashboardAsync(cancellationToken);
        var relationships = await GetRelationshipSummaryAsync(cancellationToken);
        var queue = await GetModerationQueueDetailAsync(cancellationToken);
        var profile = dashboard.Drivers.FirstOrDefault();

        return new MeDashboardViewModel
        {
            DisplayName = profile?.Name ?? "Demo user",
            RoleScope = "Driver / Owner / Counterparty",
            PublicProfile = profile,
            Relationships = relationships,
            RelationshipRequests = queue
                .Select(item => new RelationshipRequestCard
                {
                    CaseId = item.CaseId,
                    CaseType = item.CaseType,
                    RelationshipContext = item.RelationshipContext,
                    PrimaryProfileId = item.PrimaryProfileId,
                    PrimaryProfileName = profile?.Name ?? $"Profile {item.PrimaryProfileId}",
                    Counterparty = string.IsNullOrWhiteSpace(item.Counterparty) ? "Counterparty pending" : item.Counterparty,
                    Status = item.Status,
                    PrivacyStatus = item.PrivacyStatus,
                    UpdatedAtUtc = item.UpdatedAtUtc,
                    ConfirmationClaims = item.Confirmations.Select(confirmation => $"{confirmation.Claim} — {confirmation.State}").ToList()
                })
                .ToList(),
            Status = queue.Count == 0
                ? "No counterparty approvals are waiting right now."
                : $"Showing {queue.Count} relationship request{(queue.Count == 1 ? string.Empty : "s")} awaiting action."
        };
    }

    public async Task<WorkflowSubmissionResult> UpdateVerificationCaseStatusAsync(
        Guid caseId,
        string status,
        CancellationToken cancellationToken)
    {
        try
        {
            using var response = await _httpClient.PatchAsJsonAsync(
                $"api/VerificationCases/{caseId}/status",
                new { status },
                JsonOptions,
                cancellationToken);

            return new WorkflowSubmissionResult(
                response.IsSuccessStatusCode,
                response.IsSuccessStatusCode
                    ? $"Relationship request marked {status}."
                    : $"VerifyDriverAPI rejected the relationship update with status {(int)response.StatusCode}.");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogWarning(ex, "Could not update relationship request {CaseId}.", caseId);
            return new WorkflowSubmissionResult(false, "Relationship update API unavailable. Retry when VerifyDriverAPI is running.");
        }
    }

    private static string BuildSearchQuery(string query, string mode, string? intent, string? relationshipType)
    {
        var parameters = new Dictionary<string, string?>
        {
            ["query"] = query,
            ["mode"] = mode,
            ["intent"] = intent,
            ["relationshipType"] = relationshipType
        };

        return string.Join("&", parameters
            .Where(item => !string.IsNullOrWhiteSpace(item.Value))
            .Select(item => $"{Uri.EscapeDataString(item.Key)}={Uri.EscapeDataString(item.Value!)}"));
    }

    private static string MatchStatus(int matchCount, string query, string mode, string? intent, string? relationshipType)
    {
        if (mode.Equals("opportunity", StringComparison.OrdinalIgnoreCase))
        {
            var context = string.Join(" / ", new[] { intent, relationshipType }.Where(value => !string.IsNullOrWhiteSpace(value)));

            return $"Found {matchCount} potential relationship match{(matchCount == 1 ? string.Empty : "es")} for \"{query}\"{(string.IsNullOrWhiteSpace(context) ? "." : $" in {context}.")}";
        }

        return $"Found {matchCount} profile{(matchCount == 1 ? string.Empty : "s")} related to \"{query}\".";
    }

    private static string NoMatchStatus(string query, string mode, string? intent, string? relationshipType)
    {
        if (mode.Equals("opportunity", StringComparison.OrdinalIgnoreCase))
        {
            var context = string.Join(" / ", new[] { intent, relationshipType }.Where(value => !string.IsNullOrWhiteSpace(value)));

            return $"No relationship matches found for \"{query}\"{(string.IsNullOrWhiteSpace(context) ? "." : $" in {context}.")} Try a vehicle registration, platform, fleet owner, driver type, or licence requirement.";
        }

        return $"No driver profile matched \"{query}\". You can submit a structured signal or request verification.";
    }

    private static bool Matches(DriverTrustCard driver, string query, string mode)
    {
        if (mode.Equals("profile", StringComparison.OrdinalIgnoreCase))
        {
            return Contains(driver.Name, query)
                || Contains(driver.VehicleRegistration, query);
        }

        return Contains(driver.Name, query)
            || Contains(driver.Category, query)
            || Contains(driver.Region, query)
            || Contains(driver.VehicleRegistration, query)
            || Contains(driver.VehicleDescription, query)
            || Contains(driver.PartnerName, query)
            || Contains(driver.UserType, query)
            || driver.Signals.Any(signal => Contains(signal, query));
    }

    private static bool Contains(string value, string query)
    {
        return value.Contains(query, StringComparison.OrdinalIgnoreCase);
    }

    private static DriverTrustCard MapDriver(ApiUserDto user)
    {
        var rating = user.URating;
        var trustScore = rating <= 5 ? (int)Math.Round(rating * 20) : (int)Math.Round(rating);
        trustScore = Math.Clamp(trustScore, 0, 100);
        var userType = string.IsNullOrWhiteSpace(user.UserType?.UTDescription) ? "Professional driver" : user.UserType.UTDescription;
        var vehicle = user.Vehicle is null
            ? "Vehicle pending"
            : string.Join(" ", new[] { user.Vehicle.VMake, user.Vehicle.VModelName, user.Vehicle.VModelYear }.Where(value => !string.IsNullOrWhiteSpace(value)));

        if (string.IsNullOrWhiteSpace(vehicle))
        {
            vehicle = "Vehicle pending";
        }

        return new DriverTrustCard
        {
            UserId = user.UID,
            Initials = GetInitials(user.UNames),
            Name = string.IsNullOrWhiteSpace(user.UNames) ? "Unnamed driver" : user.UNames,
            Category = userType,
            Region = "Region pending",
            VehicleRegistration = user.Vehicle?.VRegistration ?? "Registration pending",
            VehicleDescription = vehicle,
            PartnerName = user.Partner?.PName ?? "Partner pending",
            UserType = userType,
            Rating = rating,
            TrustScore = trustScore,
            ReportCount = Math.Max(1, trustScore / 8),
            RiskLevel = trustScore >= 80 ? "Low" : trustScore >= 60 ? "Review" : "High",
            ActivitySummary = $"{userType} profile linked to {user.Vehicle?.VRegistration ?? "a pending vehicle"} and {user.Partner?.PName ?? "a pending partner"}.",
            Signals = [
                $"Rating {rating:0.0}",
                user.Vehicle?.VRegistration ?? "Vehicle pending",
                user.Partner?.PName ?? "Partner pending"
            ]
        };
    }

    private static DriverTrustCard MapTrustProfile(ApiTrustProfileDto profile)
    {
        return new DriverTrustCard
        {
            UserId = profile.UserId,
            Initials = GetInitials(profile.Name),
            Name = string.IsNullOrWhiteSpace(profile.Name) ? $"Profile {profile.UserId}" : profile.Name,
            Category = profile.Role,
            Region = "Region pending",
            VehicleRegistration = profile.Vehicle?.Registration ?? "Registration pending",
            VehicleDescription = profile.Vehicle?.Description ?? "Vehicle pending",
            PartnerName = profile.Partner?.Name ?? "Partner pending",
            UserType = profile.Role,
            Rating = profile.Rating,
            TrustScore = profile.TrustScore,
            ReportCount = profile.FeedbackSummary?.TotalReports ?? 0,
            RiskLevel = profile.RiskLevel,
            ActivitySummary = profile.FeedbackSummary?.Summary ?? $"{profile.Role} trust profile loaded from VerifyDriverAPI.",
            Signals = profile.RankingSignals.Count > 0 ? profile.RankingSignals : profile.Signals
        };
    }

    private static string GetInitials(string? name)
    {
        var parts = (name ?? string.Empty)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length == 0)
        {
            return "VD";
        }

        return string.Concat(parts.Take(2).Select(part => char.ToUpperInvariant(part[0])));
    }

    private static IReadOnlyList<DriverTrustCard> PreviewDrivers() =>
    [
        new DriverTrustCard
        {
            UserId = 1,
            Initials = "TM",
            Name = "Thabo Mokoena",
            Category = "Rideshare driver",
            Region = "Pretoria",
            VehicleRegistration = "GP 42 MX",
            VehicleDescription = "Toyota Corolla Quest",
            PartnerName = "Bolt",
            UserType = "Rideshare",
            Rating = 4.2m,
            TrustScore = 84,
            ReportCount = 21,
            RiskLevel = "Review",
            ActivitySummary = "Three recent passenger reports mention safe driving and clean vehicle condition. One late pickup report is under review.",
            Signals = ["Safety 4.8", "Reliability 4.2", "1 pending"]
        },
        new DriverTrustCard
        {
            UserId = 2,
            Initials = "ND",
            Name = "Nomsa Dlamini",
            Category = "Delivery driver",
            Region = "Johannesburg",
            VehicleRegistration = "LJ 18 GP",
            VehicleDescription = "Hyundai H100",
            PartnerName = "Fleet partner",
            UserType = "Delivery",
            Rating = 4.6m,
            TrustScore = 92,
            ReportCount = 38,
            RiskLevel = "Low",
            ActivitySummary = "Fleet owner confirmed a completed contract with strong punctuality and vehicle care ratings.",
            Signals = ["Employer verified", "Vehicle linked", "Trust +3"]
        },
        new DriverTrustCard
        {
            UserId = 3,
            Initials = "SK",
            Name = "Sipho Khumalo",
            Category = "Truck driver",
            Region = "Durban corridor",
            VehicleRegistration = "ND 18 LK",
            VehicleDescription = "Freightliner Argosy",
            PartnerName = "Logistics fleet",
            UserType = "Trucking",
            Rating = 3.6m,
            TrustScore = 71,
            ReportCount = 12,
            RiskLevel = "Review",
            ActivitySummary = "New road-user incident submitted with location and vehicle registration. Awaiting evidence review.",
            Signals = ["Review required", "Reg: ND 18 LK", "Heavy vehicle"]
        }
    ];

    private sealed class ApiUserDto
    {
        [JsonPropertyName("uID")]
        public int UID { get; init; }

        [JsonPropertyName("uNames")]
        public string? UNames { get; init; }

        [JsonPropertyName("uGender")]
        public string? UGender { get; init; }

        [JsonPropertyName("uAge")]
        public int UAge { get; init; }

        [JsonPropertyName("uRating")]
        public decimal URating { get; init; }

        [JsonPropertyName("vehicle")]
        public ApiVehicleDto? Vehicle { get; init; }

        [JsonPropertyName("partner")]
        public ApiPartnerDto? Partner { get; init; }

        [JsonPropertyName("userType")]
        public ApiUserTypeDto? UserType { get; init; }
    }

    private sealed class ApiVehicleDto
    {
        [JsonPropertyName("vregistration")]
        public string? VRegistration { get; init; }

        [JsonPropertyName("vMake")]
        public string? VMake { get; init; }

        [JsonPropertyName("vModel_name")]
        public string? VModelName { get; init; }

        [JsonPropertyName("vModel_year")]
        public string? VModelYear { get; init; }
    }

    private sealed class ApiPartnerDto
    {
        [JsonPropertyName("pName")]
        public string? PName { get; init; }
    }

    private sealed class ApiUserTypeDto
    {
        [JsonPropertyName("U_T_description")]
        public string? UTDescription { get; init; }
    }

    private sealed class ApiProfileSearchResponse
    {
        public IReadOnlyList<ApiTrustProfileDto> Results { get; init; } = [];
    }

    private sealed class ApiTrustProfileDto
    {
        public int UserId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Role { get; init; } = "Driver";
        public decimal Rating { get; init; }
        public int TrustScore { get; init; }
        public string RiskLevel { get; init; } = "Review";
        public ApiVehicleSummaryDto? Vehicle { get; init; }
        public ApiPartnerSummaryDto? Partner { get; init; }
        public IReadOnlyList<string> Signals { get; init; } = [];
        public ApiFeedbackSummaryDto? FeedbackSummary { get; init; }
        public IReadOnlyList<string> RankingSignals { get; init; } = [];
    }

    private sealed class ApiVehicleSummaryDto
    {
        public string Registration { get; init; } = "Registration pending";
        public string Description { get; init; } = "Vehicle pending";
    }

    private sealed class ApiPartnerSummaryDto
    {
        public string Name { get; init; } = "Partner pending";
    }

    private sealed class ApiFeedbackSummaryDto
    {
        public int TotalReports { get; init; }
        public string Summary { get; init; } = "No moderated feedback summary is available yet.";
    }

    private sealed class ApiModerationQueueDto
    {
        public IReadOnlyList<object> Feedback { get; init; } = [];
        public IReadOnlyList<ApiVerificationCaseDto> VerificationCases { get; init; } = [];
        public IReadOnlyList<string> DuplicateProfiles { get; init; } = [];
        public IReadOnlyList<string> SuspiciousActivity { get; init; } = [];
    }

    private async Task<IReadOnlyList<ApiVerificationCaseDto>> GetModerationQueueDetailAsync(CancellationToken cancellationToken)
    {
        try
        {
            var queue = await _httpClient.GetFromJsonAsync<ApiModerationQueueDto>("api/Moderation/queue", JsonOptions, cancellationToken);
            return queue?.VerificationCases ?? [];
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogWarning(ex, "Could not load counterparty relationship requests from VerifyDriverAPI moderation queue.");
            return [];
        }
    }

    private sealed class ApiVerificationCaseDto
    {
        public Guid CaseId { get; init; }
        public string CaseType { get; init; } = "Relationship verification";
        public string RelationshipContext { get; init; } = "Relationship";
        public int PrimaryProfileId { get; init; }
        public string? Counterparty { get; init; }
        public string Status { get; init; } = "Draft";
        public string PrivacyStatus { get; init; } = "PrivateDocuments";
        public DateTimeOffset UpdatedAtUtc { get; init; }
        public IReadOnlyList<ApiCounterpartyConfirmationDto> Confirmations { get; init; } = [];
    }

    private sealed class ApiCounterpartyConfirmationDto
    {
        public string Claim { get; init; } = "Relationship confirmation";
        public string State { get; init; } = "Requested";
    }

    private sealed class ApiRelationshipDto
    {
        public string RelationshipType { get; init; } = string.Empty;
        public string VerificationStatus { get; init; } = string.Empty;
        public string AvailabilityStatus { get; init; } = string.Empty;
    }
}