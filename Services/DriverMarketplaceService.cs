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
        var dashboard = await GetDashboardAsync(cancellationToken);
        var cleanQuery = query?.Trim();
        var cleanMode = string.IsNullOrWhiteSpace(mode) ? "profile" : mode.Trim();
        var cleanIntent = intent?.Trim();
        var cleanRelationshipType = relationshipType?.Trim();

        if (string.IsNullOrWhiteSpace(cleanQuery))
        {
            return WithStatus(cleanMode.Equals("opportunity", StringComparison.OrdinalIgnoreCase)
                ? "Choose your relationship goal, then search for a vehicle, platform, fleet, licence type, or role."
                : "Enter a person, driver name, vehicle registration, partner, platform, or region to verify.", []);
        }

        var matches = dashboard.Drivers
            .Where(driver => Matches(driver, cleanQuery, cleanMode))
            .ToList();

        return WithStatus(
            matches.Count == 0
                ? NoMatchStatus(cleanQuery, cleanMode, cleanIntent, cleanRelationshipType)
                : MatchStatus(matches.Count, cleanQuery, cleanMode, cleanIntent, cleanRelationshipType),
            matches);

        DriverTrustDashboardViewModel WithStatus(string status, IReadOnlyList<DriverTrustCard> drivers) => new()
        {
            ApiConnected = dashboard.ApiConnected,
            ApiStatus = status,
            Drivers = drivers
        };
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
}