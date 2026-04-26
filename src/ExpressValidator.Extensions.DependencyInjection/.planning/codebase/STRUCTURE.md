# Codebase Structure

**Analysis Date:** 2026-04-26

## Directory Layout

```
ExpressValidator/
|-- src/
|   |-- ExpressValidator.Extensions.DependencyInjection/   # DI extension library source
|-- tests/
|   |-- ExpressValidator.Extensions.DependencyInjection.Tests/  # NUnit test suite for DI extension
|-- samples/                                            # Example apps (referenced by README)
|-- packages/                                           # NuGet package cache for legacy test projects
```

## Directory Purposes

**`src/ExpressValidator.Extensions.DependencyInjection`:**
- Purpose: Publishable DI extension package.
- Contains: registration APIs, proxy/reload adapters, support interfaces, docs.
- Key files: `src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`, `src/ExpressValidator.Extensions.DependencyInjection/ExpressValidator.Extensions.DependencyInjection.csproj`.

**`tests/ExpressValidator.Extensions.DependencyInjection.Tests`:**
- Purpose: Unit/integration tests for DI registration and reload behaviors.
- Contains: NUnit fixtures and legacy test project config.
- Key files: `tests/ExpressValidator.Extensions.DependencyInjection.Tests/ServiceCollectionExtensionsTests.cs`, `tests/ExpressValidator.Extensions.DependencyInjection.Tests/AddExpressValidationIntegrationTests.cs`.

## Key File Locations

**Entry Points:**
- `src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`: public extension methods consumed by application startup.

**Configuration:**
- `src/ExpressValidator.Extensions.DependencyInjection/ExpressValidator.Extensions.DependencyInjection.csproj`: target frameworks, package metadata, package dependencies.
- `tests/ExpressValidator.Extensions.DependencyInjection.Tests/ExpressValidator.Extensions.DependencyInjection.Tests.csproj`: test framework and assembly references.

**Core Logic:**
- `src/ExpressValidator.Extensions.DependencyInjection/ProxyValidator.cs`: resolves configurator and delegates validate calls.
- `src/ExpressValidator.Extensions.DependencyInjection/ExpressValidatorWithReload.cs`: rebuilds validator on options change.
- `src/ExpressValidator.Extensions.DependencyInjection/OptionsMonitorContext.cs`: tracks options value and timestamp safely.

**Testing:**
- `tests/ExpressValidator.Extensions.DependencyInjection.Tests/ServiceCollectionExtensionsTests.cs`: registration smoke tests.
- `tests/ExpressValidator.Extensions.DependencyInjection.Tests/ServiceCollectionExtensionsTests.AddExpressValidation.cs`: configurator scanning and lifetime behavior.
- `tests/ExpressValidator.Extensions.DependencyInjection.Tests/ExpressValidatorWithReloadTests.cs`: reload behavior tests.
- `tests/ExpressValidator.Extensions.DependencyInjection.Tests/OptionsMonitorContextTests.cs`: context initialization behavior.

## Naming Conventions

**Files:**
- PascalCase for class-per-file C# source (`ServiceCollectionExtensions.cs`, `OptionsMonitorContext.cs`).
- Test file names end with `Tests.cs`; partial test fixtures use dotted suffix (`ServiceCollectionExtensionsTests.AddExpressValidation.cs`).

**Directories:**
- Solution-style module naming (`ExpressValidator.Extensions.DependencyInjection`) mirrored in `src/` and `tests/`.

## Where to Add New Code

**New DI registration feature:**
- Primary code: `src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`
- Tests: `tests/ExpressValidator.Extensions.DependencyInjection.Tests/ServiceCollectionExtensionsTests*.cs`

**New runtime validator wrapper:**
- Implementation: `src/ExpressValidator.Extensions.DependencyInjection/` (new class + internal interface if needed)
- Tests: add fixture in `tests/ExpressValidator.Extensions.DependencyInjection.Tests/`

**Utilities or support contracts:**
- Shared helpers/contracts: `src/ExpressValidator.Extensions.DependencyInjection/` alongside `IOptionsMonitorContext.cs` and `SectionPathHolder.cs`

## Special Directories

**`src/ExpressValidator.Extensions.DependencyInjection/docs`:**
- Purpose: package README assets (NuGet markdown).
- Generated: No.
- Committed: Yes.

**`src/ExpressValidator.Extensions.DependencyInjection/bin` and `obj`:**
- Purpose: build outputs and intermediates.
- Generated: Yes.
- Committed: No (normally ignored).

---

*Structure analysis: 2026-04-26*
