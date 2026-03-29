# Test Infrastructure Strategy

## Objective

The test suite keeps setup, teardown, and dependency ownership explicit through NUnit infrastructure classes.

## Ownership Model

- `TestBase` owns shared configuration and API service construction
- `PlaywrightTestBase` owns browser startup, context lifecycle, page lifecycle, and browser evidence capture
- each test class depends explicitly on the infrastructure it needs

## Scope Mapping

Current scope model:

- `OneTimeSetUp` creates the shared Playwright browser once per fixture class
- `SetUp` creates a fresh browser context and page per test
- `TearDown` closes the context and writes evidence for the current test
- `OneTimeTearDown` closes the shared browser

## Why This Design Works

- isolation is per test by default
- setup remains explicit
- cleanup is owned by the creator
- no hidden mutable browser state is reused across tests

## Reporting Ownership

Reporting stays in the infrastructure layer:

- screenshot and trace capture are controlled centrally through `BROWSER_EVIDENCE_MODE`
- `on_failure` captures browser evidence only for failed UI and E2E tests
- `always` captures browser evidence for every UI and E2E test
- Allure attachments through framework helpers

This keeps evidence collection out of business services and page-object logic.
