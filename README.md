# MvcDriverVerify

ASP.NET Core MVC frontend for the Verify Driver marketplace. The app provides public verification search, relationship discovery, the `/Me` personal HUD, verification-case setup, and admin moderation dashboards backed by `VerifyDriverAPI`.

## Current focus

- Homepage search is public verification search and should remain broad enough to find drivers, vehicles, platforms, partners, roles, and trust signals.
- `/Relationships` has two search modes:
  - Known profile search for specific people or relationship records.
  - Opportunity search for counterparty discovery using intent and relationship type.
- `/Me` separates profile updates, vehicle add/update, relationship requests, and current relationships.
- Vehicle add and update are separate UI flows; add creates a new current-user-owned vehicle, while update edits an existing owned vehicle.

## Deferred API/design dependencies

1. Endpoint convention cleanup: migrate frontend calls from mixed current-user routes such as `/api/Vehicles/mine` to a consistent `/api/me/...` tree once the API provides compatibility aliases.
2. Current-user identity should stay API-owned. The frontend should not fetch a user ID and use it as authority for private workspace reads or writes.
3. Relationship card labels need enriched backend DTOs so the UI can show names, vehicle registrations, platforms, and partners instead of internal IDs.
4. A later backend data-model normalization pass should separate profiles, roles, vehicle ownership, platform eligibility, relationships, and verification claims.

The backend tracking items are in `Requirements\VerifyDriverAPI\specs\backend_requirements.yaml` as `API-FR-057`, `API-TR-014`, and `API-DR-004`.

## Local validation

From the workspace root:

```powershell
dotnet build "projects\MvcDriverVerify\MvcDriverVerify.csproj" --nologo
dotnet test "projects\MvcDriverVerify\Tests\MvcDriverVerify.Tests.csproj" --nologo
```

If a local MVC server is running and locks the executable during validation, use:

```powershell
dotnet build "projects\MvcDriverVerify\MvcDriverVerify.csproj" --nologo -p:UseAppHost=false
dotnet test "projects\MvcDriverVerify\Tests\MvcDriverVerify.Tests.csproj" --nologo -p:UseAppHost=false
```

Run locally:

```powershell
dotnet run --project "projects\MvcDriverVerify\MvcDriverVerify.csproj"
```

## Prompt-pipeline engine flow toward Azure

Run these from the workspace root and resume each workflow until it reaches `WORKFLOW COMPLETED`.

1. Frontend polish:

```powershell
python Automation\engine\copilot_runner.py --task "work on user experience" --project MvcDriverVerify
```

2. Git release candidate:

```powershell
python Automation\engine\copilot_runner.py --task "deploy MvcDriverVerify to git" --project MvcDriverVerify
```

3. Azure deployment preparation and execution:

```powershell
python Automation\engine\copilot_runner.py --task "deploy MvcDriverVerify to azure" --project MvcDriverVerify
```

The Azure workflow is expected to generate or validate `projects\MvcDriverVerify\infra\main.bicep`, `projects\MvcDriverVerify\.github\workflows\azure-deploy.yml`, `Deployment\MvcDriverVerify\azure_deployment\deployment-plan.md`, `cost-estimate.md`, `approval-gate.md`, deployment logs, and post-deployment validation reports.

For an end-to-end marketplace deployment, run the backend flow first, then the frontend flow, and configure the MVC app with the deployed `VerifyDriverAPI` base URL before final post-deployment validation.
