# UI Layer

## Purpose

The UI layer models browser-visible behavior only.

Its ownership rules are:

- page objects own locators
- page objects own browser interactions
- tests keep orchestration visible

## Files

- `src/WikiAutomation.UI/Pages/BasePage.cs`
- `src/WikiAutomation.UI/Pages/WikipediaArticlePage.cs`
- `src/WikiAutomation.UI/Components/*.cs`
- `src/WikiAutomation.UI/Models/*.cs`

## Responsibilities

### BasePage

- stores the current `IPage`
- stores typed settings
- owns navigation to the article URL

### WikipediaArticlePage

- opens the target article
- composes smaller reusable UI helpers

### UI Components

- `ArticleSectionReader`
  reads article section content from the live DOM
- `DevelopmentToolsNavBox`
  isolates navigation-box link validation logic
- `AppearancePanel`
  isolates appearance and theme switching behavior

## Boundary Rules

- page objects do not call the API layer
- page objects do not attach Allure results directly
- page objects return meaningful values or result models for the tests to assert
- page-specific orchestration stays in the page object, while smaller reusable page areas live in components
