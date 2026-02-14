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
- Document all non-private methods.
- Document classes and methods.
- Include code examples for every documented class and method (no exceptions).
- Use XML docs with language-tagged code examples:
  - `<example>` with `<code lang="csharp">...</code>`
  - `<example>` must be within the `<remarks>` element
  - `<code>` must have actual code samples and not placeholders such as "invoke member here", TODOs, or pseudocode.
  - XML doc comments must be attached to the member declaration (place docs before attributes so analyzers bind them correctly).
- XML docs must be complete for public and internal APIs:
  - Include `<summary>` on all documented members/types.
  - Include `<param>` for every method/constructor parameter.
  - Include `<returns>` for all non-`void` methods.
  - Include `<typeparam>` for every generic type/method parameter.
  - Constructor summaries should start with standard phrasing: `Initializes a new instance of the ... class.`
- Keep XML docs warning-clean for StyleCop documentation analyzers (for example `SA1611`, `SA1615`, `SA1618`, `SA1642`).

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
