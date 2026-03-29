# Wikipedia Automation Home Assignment

Standalone C# automation framework for the assignment scope:

- UI automation with `Microsoft.Playwright`
- API validation with `HttpClient`
- Test execution with `NUnit`
- Reporting with `Allure.NUnit`

The framework is intentionally small and keeps only the layers this assignment needs:

- `src/WikiAutomation.Framework/`
- `src/WikiAutomation.TestData/`
- `src/WikiAutomation.API/`
- `src/WikiAutomation.UI/`
- `tests/WikiAutomation.Tests/`

Layer ownership is intentionally explicit:

- framework for cross-cutting technical concerns
- testdata for stable reusable article labels and inputs
- api for clients, services, and typed models
- ui for page objects plus smaller reusable UI components
- tests for API, UI, and E2E behavior split by intent

## Assignment Coverage

### Task 1

- Extract the `Debugging features` section via UI
- Extract the same section via the MediaWiki Parse API
- Normalize both texts
- Count unique words
- Assert the counts are equal

### Task 2

- Go to the `Microsoft development tools` area relevant to debugging
- Validate that the technology names under `Testing and debugging` are text links
- Fail the test if any technology name is rendered without a text link

### Task 3

- Change Wikipedia `Color (beta)` to `Dark`
- Validate that the theme actually changed

## Project Structure

```text
project-root/
  .github/workflows/pr-validation.yml
  .github/workflows/run-home-assignment-suite.yml
  src/
    WikiAutomation.Framework/
    WikiAutomation.API/
    WikiAutomation.UI/
  tests/
    WikiAutomation.Tests/
  allureConfig.json
  run-tests.ps1
```

## Documentation

The `docs/` folder explains the implementation choices and operating model for this repository:

- `docs/target-architecture.md`
- `docs/project-structure.md`
- `docs/coding-standards.md`
- `docs/fixture-strategy.md`
- `docs/api-layer.md`
- `docs/ui-layer.md`
- `docs/test-strategy.md`
- `docs/reporting-and-ci.md`
- `docs/assignment-scope.md`
- `docs/conversation-summary.md`

## Setup

### 1. Install prerequisites

- .NET SDK 8
- PowerShell

Before running anything, confirm the tools exist:

```powershell
dotnet --info
powershell -Version
```

If `dotnet` is not recognized, install the .NET 8 SDK first and reopen the terminal.

### 2. Restore and build

```powershell
dotnet restore tests/WikiAutomation.Tests/WikiAutomation.Tests.csproj
dotnet build tests/WikiAutomation.Tests/WikiAutomation.Tests.csproj
```

### 3. Install Playwright browsers

```powershell
powershell -ExecutionPolicy Bypass -File tests\WikiAutomation.Tests\bin\Debug\net8.0\playwright.ps1 install chromium
```

## Run Tests

Run the whole suite from the repository root:

```powershell
dotnet test tests/WikiAutomation.Tests/WikiAutomation.Tests.csproj
```

Run a specific test class:

```powershell
dotnet test tests/WikiAutomation.Tests/WikiAutomation.Tests.csproj --filter "FullyQualifiedName~TestDebuggingFeaturesVerticalSlice"
```

Run by category:

```powershell
dotnet test tests/WikiAutomation.Tests/WikiAutomation.Tests.csproj --filter "Category=api"
dotnet test tests/WikiAutomation.Tests/WikiAutomation.Tests.csproj --filter "Category=ui"
dotnet test tests/WikiAutomation.Tests/WikiAutomation.Tests.csproj --filter "Category=e2e"
```

Run the suite while collecting browser screenshot and trace artifacts for every UI and E2E test:

```powershell
$env:BROWSER_EVIDENCE_MODE="always"
dotnet test tests/WikiAutomation.Tests/WikiAutomation.Tests.csproj
```

Return to the default behavior:

```powershell
$env:BROWSER_EVIDENCE_MODE="on_failure"
dotnet test tests/WikiAutomation.Tests/WikiAutomation.Tests.csproj
```

## Allure Reporting

Test artifacts are written to `artifacts/`.

Generate Allure results:

```powershell
dotnet test tests/WikiAutomation.Tests/WikiAutomation.Tests.csproj --logger "trx"
```

Generate a static report after the run:

```powershell
& allure.cmd generate artifacts/allure-results --clean -o artifacts/allure-report
```

Open the generated report:

```powershell
& allure.cmd open artifacts/allure-report
```

In GitHub Actions, the PR validation workflow also generates `artifacts/allure-report/` and uploads the full `artifacts/` folder as a workflow artifact.

## GitHub Actions Reports

After a CI or PR run completes:

1. Open the `Actions` tab in GitHub.
2. Open the relevant `PR Validation` run.
3. Open the job summary for the workflow run.
4. Under `Allure Report`:
   - click `here` next to `Report` to open the hosted Allure report
   - click `here` next to `Downloadable artifact` to download the generated report files

The hosted report URL is intended for direct viewing in the browser.

The downloadable artifact is a fallback package containing the generated report files and test artifacts.

## CI

The GitHub Actions setup is split into:

- `pr-validation.yml`
  the pull-request and `main` branch validation entrypoint
- `run-home-assignment-suite.yml`
  the reusable workflow that restores, builds, installs Playwright, runs tests, generates the Allure report, and uploads artifacts

That means the PR check you can require in branch protection is the `PR Validation` workflow/job.

## Environment Variables

Optional configuration:

- `BASE_URL` default: `https://en.wikipedia.org`
- `ARTICLE_TITLE` default: `Playwright_(software)`
- `HEADLESS` default: `true`
- `DEFAULT_TIMEOUT_MS` default: `15000`
- `BROWSER_EVIDENCE_MODE` default: `on_failure`
  Use `always` to attach screenshot and Playwright trace for every UI and E2E test instead of only on failures.

## Notes

- Task 2 is intentionally strict. If Wikipedia contains plain text items instead of links in the targeted subsection, the test will fail and list them explicitly.
