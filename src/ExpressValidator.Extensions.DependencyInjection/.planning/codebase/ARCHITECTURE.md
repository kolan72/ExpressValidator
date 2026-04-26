<!-- refreshed: 2026-04-26 -->
# Architecture

**Analysis Date:** 2026-04-26

## System Overview

```text
Client app (ASP.NET Core / generic host)
  -> IServiceCollection extensions
     `src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`
  -> Registered validators / builders / reload wrappers
     `src/ExpressValidator.Extensions.DependencyInjection/ProxyValidator.cs`
     `src/ExpressValidator.Extensions.DependencyInjection/ExpressValidatorWithReload.cs`
  -> Validation execution
     via ExpressValidator interfaces from package dependency
```

## Component Responsibilities

| Component | Responsibility | File |
|-----------|----------------|------|
| `ServiceCollectionExtensions` | Registers validation services and options bindings into DI | `src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs` |
| `ValidatorConfigurator<T>` | Base class for class-based validator configuration | `src/ExpressValidator.Extensions.DependencyInjection/ValidatorConfigurator.cs` |
| `ProxyValidator<T>` | Adapts configurator-built validator to `IExpressValidator<T>` for DI resolution | `src/ExpressValidator.Extensions.DependencyInjection/ProxyValidator.cs` |
| `ExpressValidatorWithReload<TObj,TOptions>` | Rebuilds validator when options timestamp changes | `src/ExpressValidator.Extensions.DependencyInjection/ExpressValidatorWithReload.cs` |
| `OptionsMonitorContext<TOptions>` | Tracks current options + last update from `IOptionsMonitor` | `src/ExpressValidator.Extensions.DependencyInjection/OptionsMonitorContext.cs` |

## Pattern Overview

**Overall:** DI extension + adapter/proxy pattern

**Key Characteristics:**
- Extension methods provide composable registration entry points.
- Generics carry validated model and options types through DI.
- Validation implementation creation is deferred to factory delegates.

## Layers

**Registration Layer:**
- Purpose: Expose public DI APIs
- Location: `src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`
- Contains: `AddExpressValidation`, `AddExpressValidator`, `AddExpressValidatorBuilder`, `AddExpressValidatorWithReload`
- Depends on: `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.Options`
- Used by: Consumer startup code

**Runtime Adapter Layer:**
- Purpose: Convert configuration + DI state into executable validators
- Location: `src/ExpressValidator.Extensions.DependencyInjection/ProxyValidator.cs`, `src/ExpressValidator.Extensions.DependencyInjection/ExpressValidatorWithReload.cs`
- Contains: Proxy and reload-aware validator wrappers
- Depends on: DI service provider, options monitor context, ExpressValidator builders
- Used by: Resolved validator consumers

**Support Contracts Layer:**
- Purpose: Internal interfaces and option holder types
- Location: `src/ExpressValidator.Extensions.DependencyInjection/IOptionsMonitorContext.cs`, `src/ExpressValidator.Extensions.DependencyInjection/IValidatorConfigurator.cs`, `src/ExpressValidator.Extensions.DependencyInjection/SectionPathHolder.cs`
- Contains: Internal abstractions to decouple monitor and configurator behavior
- Depends on: .NET base libraries
- Used by: Registration and runtime adapter layers

## Data Flow

### Primary Request Path

1. Consumer calls `services.AddExpressValidator<T>(...)` (`src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`)
2. Factory delegate builds an `ExpressValidatorBuilder<T>` and applies configure callback (`src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`)
3. Built validator is resolved as `IExpressValidator<T>` and used for `Validate`/`ValidateAsync` (`src/ExpressValidator.Extensions.DependencyInjection/ProxyValidator.cs`)

### Options Reload Flow

1. Consumer calls `services.AddExpressValidatorWithReload<T,TOptions>(..., sectionPath)` (`src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`)
2. `OptionsMonitorContext<TOptions>` subscribes to `IOptionsMonitor.OnChange` and updates timestamp (`src/ExpressValidator.Extensions.DependencyInjection/OptionsMonitorContext.cs`)
3. `ExpressValidatorWithReload` rebuilds validator when `LastUpdated` advances (`src/ExpressValidator.Extensions.DependencyInjection/ExpressValidatorWithReload.cs`)

**State Management:**
- In-memory per-service-instance fields (`_expressValidator`, `_lastUpdated`, `_options`)

## Key Abstractions

**`IValidatorConfigurator<T>`:**
- Purpose: Encapsulate configuration for building validators discovered by assembly scan
- Examples: test configurators in `tests/ExpressValidator.Extensions.DependencyInjection.Tests/AddExpressValidationIntegrationTests.cs`
- Pattern: Generic configuration contract

**`IOptionsMonitorContext<TOptions>`:**
- Purpose: Supplies current options and update timestamp to reloadable validator
- Examples: `src/ExpressValidator.Extensions.DependencyInjection/OptionsMonitorContext.cs`
- Pattern: Internal interface adapter over `IOptionsMonitor<TOptions>`

## Entry Points

**Public extension APIs:**
- Location: `src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`
- Triggers: Application startup service registration
- Responsibilities: Register validators and helper services with configured lifetimes

## Architectural Constraints

- **Threading:** `OptionsMonitorContext<TOptions>` uses a private lock for updates/reads; `ExpressValidatorWithReload<TObj,TOptions>` performs unsynchronized `_expressValidator` replacement.
- **Global state:** None detected.
- **Circular imports:** Not detected.
- **Dependency model:** Strong coupling to ExpressValidator package APIs and Microsoft.Extensions abstractions.

## Anti-Patterns

### Duplicate service registrations

**What happens:** Repeated calls to `AddExpressValidation` append additional `IExpressValidator<>` descriptors.  
**Why it's wrong:** Service collection can accumulate duplicates unexpectedly in modular startup composition.  
**Do this instead:** Use idempotent registration (`TryAdd*`) when duplicate behavior is not desired in `src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`.

### Mixed project styles for library and tests

**What happens:** Library uses SDK-style project while tests use legacy non-SDK `packages.config` style.  
**Why it's wrong:** Maintenance complexity and divergent restore/build tooling.  
**Do this instead:** Migrate tests to SDK-style `PackageReference` in `tests/ExpressValidator.Extensions.DependencyInjection.Tests/ExpressValidator.Extensions.DependencyInjection.Tests.csproj`.

## Error Handling

**Strategy:** Relies on underlying DI and options APIs to throw; no custom exception translation layer.

**Patterns:**
- `GetRequiredService<T>()` is used to fail fast if required service is missing in `src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs` and `src/ExpressValidator.Extensions.DependencyInjection/ProxyValidator.cs`.
- Validation methods delegate to underlying validator implementations without wrapping.

## Cross-Cutting Concerns

**Logging:** Not implemented in this module.  
**Validation:** Delegated to ExpressValidator/FluentValidation integration.  
**Authentication:** Not applicable.

---

*Architecture analysis: 2026-04-26*
