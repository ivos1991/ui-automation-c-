# Project Structure

```text
project-root/
  docs/
    api-layer.md
    coding-standards.md
    fixture-strategy.md
    project-structure.md
    reporting-and-ci.md
    target-architecture.md
    test-strategy.md
    ui-layer.md
  src/
    WikiAutomation.TestData/
      Constants/
    WikiAutomation.Framework/
      Clients/
        Api/
      Config/
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
      E2E/
      Infrastructure/
      UI/
```

## Notes

- `Framework` is the cross-cutting technical core
- `TestData` stores stable reusable article labels and target constants
- `API` contains endpoint clients, services, and typed contracts
- `UI` contains page objects, smaller reusable UI components, and UI-facing result models
- `tests/Infrastructure` owns shared setup, teardown, and test lifecycle concerns
