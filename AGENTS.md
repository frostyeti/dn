# AGENTS

## Purpose
Execution guide for contributors and coding agents in this repository.

## Tooling
- Use `mise` for .NET tooling when available on `PATH`.
- Prefer `mise exec -- dotnet ...` for all `dotnet` commands.
- If `mise` is unavailable, use `dotnet` directly.

## Finalization Checklist (Required)
When finalizing changes, run in this order:
1. `mise exec -- dotnet format`
2. `mise exec -- dotnet build`
3. `mise exec -- dotnet test`

If `mise` is unavailable:
1. `dotnet format`
2. `dotnet build`
3. `dotnet test`

Address `dotnet fmt` warnings whenever possible as part of the same change.

## Documentation Requirements
- Document ALL non-private members (classes, structs, interfaces, constructors, properties, methods, operators).
- Use XML docs with language-tagged code examples for ALL non-private classes AND ALL non-private members:
  - Every `<summary>` must be accompanied by a `<remarks>` containing `<example>` with `<code lang="csharp">...</code>`
  - `<example>` must be within the `<remarks>` element
  - `<code>` must have actual executable code samples, NOT placeholders such as "invoke member here", "TODO", or pseudocode
  - XML doc comments must be attached to the member declaration (place docs before attributes so analyzers bind them correctly)
- XML docs must be complete for public and internal APIs:
  - Include `<summary>` on all documented members/types.
  - Include `<param>` for every method/constructor parameter.
  - Include `<returns>` for all non-`void` methods.
  - Include `<typeparam>` for every generic type/method parameter.
  - Constructor summaries should start with standard phrasing: `Initializes a new instance of the ... class.`
- Keep XML docs warning-clean for StyleCop documentation analyzers (for example `SA1611`, `SA1615`, `SA1618`, `SA1642`).

### Documentation Validation
Run the validation script to check for missing code examples:
```bash
./scripts/validate-docs.sh
```
This script MUST pass before any changes can be committed. It checks:
- Every non-private member has a `<summary>` element
- Every `<summary>` has a corresponding `<example>` with `<code lang="csharp">`
- No placeholder text in code examples

## Testing Requirements
- Use xUnit 3 style tests.
- Use vanilla `Assert` APIs.
- Use normal test methods with concise naming.
- Do not use box comments.
- Do not use Gherkin/BDD-style tests.

## OS/FFI-Dependent Test Policy
- If a test exercises FFI functionality or behavior tied to a specific OS, skip when the required OS is not available.
- If a test requires an external app/tool, skip when the expected executable is not found on `PATH`.
- Skips must be explicit and tied to the missing runtime precondition.
- For platform-specific code/tests, run tests only on supported platforms and annotate tests with `[SupportedOSPlatform]` / `[UnsupportedOSPlatform]` as appropriate.

## Naming
- Use concise naming for tests, methods, variables, and helpers.
