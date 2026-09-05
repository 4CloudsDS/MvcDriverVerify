param(
    [string]$MvcProject = "MvcDriverVerify.csproj",
    [string]$MvcUrl = "http://localhost:5172"
)

$ErrorActionPreference = "Stop"

$mvc = Start-Process -FilePath "dotnet" -ArgumentList @("run", "--project", $MvcProject, "--urls", $MvcUrl) -PassThru -WindowStyle Hidden
try {
    Start-Sleep -Seconds 8

    $pages = @(
        "$MvcUrl/",
        "$MvcUrl/Relationships",
        "$MvcUrl/Verify",
        "$MvcUrl/Me",
        "$MvcUrl/Admin"
    )

    foreach ($page in $pages) {
        Invoke-WebRequest $page -UseBasicParsing | Out-Null
    }

    Invoke-WebRequest "$MvcUrl/Home/Verify?query=Thabo" -UseBasicParsing | Out-Null
    Invoke-WebRequest "$MvcUrl/Relationships/Search?query=Fleet&mode=opportunity&relationshipType=Fleet%20contract" -UseBasicParsing | Out-Null

    Write-Host "MvcDriverVerify browser smoke checks passed for landing search, relationship search, opportunity search, Verify, Me, and Admin routes."
}
finally {
    if (!$mvc.HasExited) {
        Stop-Process -Id $mvc.Id
    }
}
