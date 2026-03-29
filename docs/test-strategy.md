# Test Strategy

## Layer Split

The test suite uses an intent-based split:

- `API/` validates API behavior without UI
- `UI/` validates browser behavior without API assertions unless the task requires it
- `E2E/` validates a cross-layer slice that combines UI and API
- `Infrastructure/` owns setup and teardown for shared test lifecycle concerns

## Current Tests

### API

- `TestDebuggingFeaturesApi`
  validates that the MediaWiki Parse API returns usable content for the `Debugging features` section

### UI

- `TestWikipediaArticleUi.MicrosoftDevelopmentTools_TestingAndDebuggingItems_ValidationExpectsTextLinksOnly`
  validates that the targeted technology names are actual text links
- `TestWikipediaArticleUi.AppearanceColorSetting_DarkModeSelectionExpectsAppliedDarkTheme`
  validates the dark-mode theme switch through live browser state

### E2E

- `TestDebuggingFeaturesVerticalSlice`
  compares the normalized unique-word counts between UI extraction and API extraction

## Categories

NUnit categories are used to organize the suite:

- `api`
- `ui`
- `e2e`

## Evidence Modes

Browser-backed tests support two reporting modes:

- `BROWSER_EVIDENCE_MODE=on_failure`
  keeps screenshot and Playwright trace attachments only for failed UI and E2E tests
- `BROWSER_EVIDENCE_MODE=always`
  attaches screenshot and Playwright trace artifacts for every UI and E2E test so the report can be reviewed without forcing a failure

API-only tests do not create browser artifacts because they do not use Playwright.

## Strategy Notes

- tests are grouped by behavior type
- business intent stays visible in the test body
- cross-layer comparisons are isolated to E2E coverage instead of leaking into lower layers
