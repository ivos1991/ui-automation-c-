# Reporting And CI

## Reporting

The project uses a centralized reporting strategy:

- Allure is the main report surface
- evidence is attached from shared framework or infrastructure code
- UI tests keep screenshots and traces
- API and E2E tests attach normalized payloads and extracted content
- framework logging is written to the standard test output stream and surfaced by Allure as `Console Output`
- browser evidence capture supports two modes: `on_failure` and `always`

## Artifact Layout

- `tests/WikiAutomation.Tests/bin/Debug/net8.0/artifacts/allure-results/`
  raw Allure results written by the NUnit run
- `artifacts/allure-results/`
  copied target location used by local report generation from the repository root
- `artifacts/allure-report/`
  generated static report for export or browser viewing

## Reporting Ownership

- reporting helpers live under `Framework/Reporting`
- logging helpers live under `Framework/Logging`
- tests call helper methods for structured evidence
- low-level report mechanics are not mixed into assertions
- test lifecycle setup owns the log-scope start, status update, and shared test summary attachment

## CI

The workflow is split into:

- `pr-validation.yml` is the PR and `main` branch entrypoint
- `run-home-assignment-suite.yml` is the reusable workflow that performs the actual suite execution

The reusable workflow:

- restores dependencies
- builds the test project
- installs Playwright Chromium
- runs the NUnit suite
- generates a static Allure report
- uploads the full `artifacts/` directory

This keeps the assignment CI small while still producing a proper PR check and downloadable report evidence.

The default CI behavior keeps `BROWSER_EVIDENCE_MODE` unset, which means the suite runs in `on_failure` mode unless the workflow is explicitly extended to collect browser evidence for every UI and E2E test.

## Local Usage

From the repository root:

- run tests with `dotnet test tests/WikiAutomation.Tests/WikiAutomation.Tests.csproj`
- set `$env:BROWSER_EVIDENCE_MODE="always"` before the run when you want screenshot and trace attachments for every UI and E2E test
- generate the static report with `& allure.cmd generate artifacts/allure-results --clean -o artifacts/allure-report`
- open the generated report with `& allure.cmd open artifacts/allure-report`

Use `allure.cmd` in PowerShell because the `allure.ps1` shim can be blocked by execution policy on Windows.
