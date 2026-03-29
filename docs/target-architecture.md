# Target Architecture

## Objective

The architecture is intentionally limited to the layers needed for the assignment requirements:

- Playwright for UI
- `HttpClient` for API validation
- NUnit for execution
- Allure for reporting

## Folder Structure

```text
project-root/
  docs/
  .github/
    workflows/
  src/
    WikiAutomation.TestData/
      Constants/
    WikiAutomation.Framework/
      Config/
      Clients/
        Api/
      Reporting/
      Utilities/
    WikiAutomation.API/
      Clients/
      Models/
      Services/
    WikiAutomation.UI/
      Components/
      Models/
      Pages/
  tests/
    WikiAutomation.Tests/
      API/
      UI/
      E2E/
      Infrastructure/
```

## Layer Responsibilities

### Framework

- cross-cutting technical concerns only
- settings and runtime paths
- generic API client behavior
- reporting helpers
- normalization utilities

### API

- endpoint-oriented clients
- domain-oriented services
- typed response or domain models

### TestData

- stable constants used across UI helpers, tests, and article-specific flows
- keeps reusable test inputs separate from test bodies

### UI

- page objects only
- locators and browser interactions
- UI-specific result models where useful

### Tests

- `API/` for API-only behavior
- `UI/` for browser-only behavior
- `E2E/` for UI + API comparison flows
- `Infrastructure/` for NUnit setup/teardown ownership

## Boundaries

Allowed dependencies:

- tests -> framework, api, ui
- api -> framework
- ui -> framework

Disallowed dependencies:

- ui -> api transport
- ui -> framework reporting
- api -> ui
- framework -> assignment-specific business logic

## Notes

- The architecture stays intentionally small because the assignment scope is limited.
- The main goal is clear ownership and clean layer boundaries.
