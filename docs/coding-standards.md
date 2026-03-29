# Coding Standards

The project follows these implementation standards:

- keep UI, API, framework, and tests clearly separated
- keep setup and teardown explicit in test infrastructure
- keep reporting in the framework layer
- prefer typed models over anonymous dictionaries in API code
- keep page objects browser-focused
- extract reusable page areas into smaller UI components when a single page would otherwise become overloaded
- keep stable article labels and other reusable inputs in a dedicated testdata layer
- keep orchestration visible in tests

## Test Layer Categories

- `api`
- `ui`
- `e2e`

These categories provide a simple intent-based split for the NUnit suite.
