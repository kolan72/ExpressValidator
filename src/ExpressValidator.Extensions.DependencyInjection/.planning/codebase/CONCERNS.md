# Codebase Concerns

**Analysis Date:** 2026-04-26

## Tech Debt

**Legacy test project format:**
- Issue: Test project remains non-SDK with `packages.config` and explicit assembly references.
- Files: `tests/ExpressValidator.Extensions.DependencyInjection.Tests/ExpressValidator.Extensions.DependencyInjection.Tests.csproj`, `tests/ExpressValidator.Extensions.DependencyInjection.Tests/packages.config`
- Impact: Harder dependency maintenance and mixed tooling behavior compared with SDK-style projects.
- Fix approach: Migrate test project to SDK-style `.csproj` with `PackageReference`.

**Duplicate registration semantics:**
- Issue: Repeated `AddExpressValidation` calls append duplicate `IExpressValidator<>` registrations.
- Files: `src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`, `tests/ExpressValidator.Extensions.DependencyInjection.Tests/ServiceCollectionExtensionsTests.AddExpressValidation.cs`
- Impact: Service lists can grow unexpectedly and behavior depends on resolution style (`GetService` vs `IEnumerable<T>`).
- Fix approach: Decide whether idempotency is desired; if yes, switch to `TryAdd` patterns.

## Known Bugs

**Potential null dereference race in reload path:**
- Symptoms: If `Validate` is called before a valid build path initializes `_expressValidator`, a null reference would be possible.
- Files: `src/ExpressValidator.Extensions.DependencyInjection/ExpressValidatorWithReload.cs`
- Trigger: Misordered or failed initialization path in consumer setup.
- Workaround: Ensure options context and builder configuration are valid before first call.

## Security Considerations

**Reflection-based assembly scanning scope:**
- Risk: `AddExpressValidation` registers all matching configurators from provided assembly without explicit allowlist.
- Files: `src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`
- Current mitigation: Filter excludes abstract/generic definitions and requires `IValidatorConfigurator<>` implementation.
- Recommendations: Add optional predicate/namespace filters for tighter registration control in shared-host applications.

## Performance Bottlenecks

**Assembly type scan on startup:**
- Problem: `Assembly.GetTypes()` + LINQ filtering runs at startup for each scan call.
- Files: `src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`
- Cause: Reflection walk across all types in provided assembly.
- Improvement path: Cache discovery results by assembly + lifetime when startup reuse patterns exist.

## Fragile Areas

**Reloadable validator state updates:**
- Files: `src/ExpressValidator.Extensions.DependencyInjection/ExpressValidatorWithReload.cs`, `src/ExpressValidator.Extensions.DependencyInjection/OptionsMonitorContext.cs`
- Why fragile: Timestamp and validator field updates are split across components with different locking strategies.
- Safe modification: Keep timestamp and validator rebuild logic behaviorally compatible; add concurrency-focused tests before refactor.
- Test coverage: Basic behavior covered, but no dedicated stress test for concurrent `Validate` + options updates in this module tests.

## Scaling Limits

**Configurator discovery growth:**
- Current capacity: Works well for small-to-medium assemblies.
- Limit: Reflection and registration cost increase with very large assemblies or repeated startup composition.
- Scaling path: Precompute registrars or generated registration sources for large solutions.

## Dependencies at Risk

**Dual FluentValidation versions by target framework:**
- Risk: `netstandard2.0` and `net8.0` compile against different major dependencies.
- Impact: Divergent behavior can appear if APIs or semantics differ across versions.
- Migration plan: Maintain dual-target tests and regularly sync compatibility checks.

## Missing Critical Features

**No built-in diagnostics hooks:**
- Problem: Library has no optional logging/telemetry around registration and reload events.
- Blocks: Harder operational debugging in production hosts when validation services fail to resolve or update unexpectedly.

## Test Coverage Gaps

**Thread-safety and disposal paths for options monitoring:**
- What's not tested: Explicit disposal semantics for `OnChange` subscription and high-concurrency reload scenarios.
- Files: `src/ExpressValidator.Extensions.DependencyInjection/OptionsMonitorContext.cs`, `src/ExpressValidator.Extensions.DependencyInjection/ExpressValidatorWithReload.cs`
- Risk: Hidden memory retention or race behavior under sustained configuration churn.
- Priority: Medium.

---

*Concerns audit: 2026-04-26*
