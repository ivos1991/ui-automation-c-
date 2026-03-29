# Assignment Scope

## Goal

Build a lightweight automation framework and test suite in C# for:

- UI automation with Playwright
- API validation against the MediaWiki Parse API
- Allure reporting

Target page:

- `https://en.wikipedia.org/wiki/Playwright_(software)`

Primary target content:

- `Debugging features`

## Implemented Tasks

### Task 1

- extract the `Debugging features` section through the live UI
- extract the same section through the MediaWiki Parse API
- normalize both texts
- count unique words
- assert equality

Implemented in:

- `tests/WikiAutomation.Tests/E2E/TestDebuggingFeaturesVerticalSlice.cs`

### Task 2

- navigate to the Microsoft development tools area
- validate that the relevant technology names under the testing/debugging area are text links

Implemented in:

- `tests/WikiAutomation.Tests/UI/TestWikipediaArticleUi.cs`

### Task 3

- change the Wikipedia `Color (beta)` setting to `Dark`
- validate that the theme actually changed

Implemented in:

- `tests/WikiAutomation.Tests/UI/TestWikipediaArticleUi.cs`
