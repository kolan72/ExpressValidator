# Coding Conventions

**Analysis Date:** 2026-04-26

## Naming Patterns

**Files:**
- One public/internal type per file with PascalCase names in `src/ExpressValidator.Extensions.DependencyInjection/*.cs`.

**Functions:**
- Public API methods use PascalCase and descriptive names (`AddExpressValidatorWithReload` in `src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`).

**Variables:**
- Private fields use `_camelCase` (`_options`, `_lastUpdated` in `src/ExpressValidator.Extensions.DependencyInjection/OptionsMonitorContext.cs`).
- Local variables often use short names in factory lambdas (`s`, `eb` in `src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`).

**Types:**
- Generic type parameter naming uses `T`, `TObj`, `TOptions` consistently across interfaces and classes.

## Code Style

**Formatting:**
- Tooling config files (`.editorconfig`, `.ruleset`, `.globalconfig`) not detected in this module path.
- Style appears C# conventional with braces on new lines and tab-indented blocks.

**Linting:**
- No explicit analyzer config file in this module.
- Inline suppressions used when needed (e.g., `#pragma warning disable S2326` in `src/ExpressValidator.Extensions.DependencyInjection/SectionPathHolder.cs`).

## Import Organization

**Order:**
1. Framework/library namespaces (`Microsoft.Extensions.*`, `FluentValidation.*`)
2. `System*` namespaces
3. Project namespace declaration

**Path Aliases:**
- Not used; standard C# namespace imports only.

## Error Handling

**Patterns:**
- Fail-fast service resolution via `GetRequiredService` (`src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`, `src/ExpressValidator.Extensions.DependencyInjection/ProxyValidator.cs`).
- No wrapper exceptions; delegate error flow to underlying libraries.

## Logging

**Framework:** None detected.

**Patterns:**
- Logging calls are absent in current source files under `src/ExpressValidator.Extensions.DependencyInjection/`.

## Comments

**When to Comment:**
- XML docs for public extension methods and public interfaces (`src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`, `src/ExpressValidator.Extensions.DependencyInjection/IExpressValidatorWithReload.cs`).
- Short inline marker comments appear in tests for known shortcuts (`//HACK` in `tests/ExpressValidator.Extensions.DependencyInjection.Tests/ServiceCollectionExtensionsTests.cs`).

**JSDoc/TSDoc:**
- Not applicable; C# XML documentation comments are used.

## Function Design

**Size:** Functions stay medium-sized and focused around DI registration/factory creation.

**Parameters:** Prefer explicit configuration delegates plus optional `ServiceLifetime` or options object.

**Return Values:** Extension methods return `IServiceCollection` for fluent chaining.

## Module Design

**Exports:** Public surface is concentrated in `ServiceCollectionExtensions`, `ExpressValidatorOptions`, and `IExpressValidatorWithReload<TObj>`.

**Barrel Files:** Not applicable in C#; no aggregation file pattern.

---

*Convention analysis: 2026-04-26*
