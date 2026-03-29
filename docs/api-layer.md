# API Layer

## Purpose

The API layer is responsible for all HTTP interaction and response shaping.

Its responsibilities are split clearly:

- clients own endpoint calls
- services own domain-oriented orchestration
- models keep contracts explicit

## Files

- `src/WikiAutomation.API/Clients/WikipediaParseClient.cs`
- `src/WikiAutomation.API/Services/WikipediaArticleService.cs`
- `src/WikiAutomation.API/Models/*.cs`

## Responsibilities

### WikipediaParseClient

- calls the MediaWiki Parse API
- resolves sections
- resolves the HTML for a specific section
- inherits generic HTTP behavior from `Framework/Clients/Api/BaseApiClient.cs`

### WikipediaArticleService

- finds the `Debugging features` section by title
- converts the section HTML directly into readable text
- removes heading and citation noise so Task 1 compares the same logical content as the UI path

## Boundary Rules

- tests do not call `HttpClient` directly
- UI page objects do not use API transport
- framework owns generic HTTP behavior, not assignment-specific endpoint logic
