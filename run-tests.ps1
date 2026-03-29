$ErrorActionPreference = "Stop"

$testProject = "tests/WikiAutomation.Tests/WikiAutomation.Tests.csproj"
$outputDir = "tests/WikiAutomation.Tests/bin/Debug/net8.0"
$allureResultsDir = "artifacts/allure-results"
$allureReportDir = "artifacts/allure-report"

dotnet restore $testProject
dotnet build $testProject

if (Test-Path "$outputDir/playwright.ps1") {
    if (Get-Command pwsh -ErrorAction SilentlyContinue) {
        pwsh "$outputDir/playwright.ps1" install chromium
    }
    else {
        powershell -ExecutionPolicy Bypass -File "$outputDir/playwright.ps1" install chromium
    }
}

dotnet test $testProject

$allureCli = Get-Command allure.cmd -ErrorAction SilentlyContinue
if ($null -ne $allureCli) {
    & $allureCli.Source generate $allureResultsDir --clean -o $allureReportDir
    Write-Host "Allure report generated at $allureReportDir"
    Write-Host "Open it with: & `"$($allureCli.Source)`" open $allureReportDir"
}
else {
    Write-Warning "allure.cmd was not found on PATH. Test results are still available in $allureResultsDir."
}
