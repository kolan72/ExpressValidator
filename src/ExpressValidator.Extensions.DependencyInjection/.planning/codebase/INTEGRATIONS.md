# External Integrations

**Analysis Date:** 2026-04-26

## APIs & External Services

**Validation/Framework Libraries:**
- ExpressValidator - core validation contracts and builders used by `src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`
  - SDK/Client: `ExpressValidator` NuGet package
  - Auth: Not applicable
- FluentValidation - result model types used by `src/ExpressValidator.Extensions.DependencyInjection/ProxyValidator.cs` and `src/ExpressValidator.Extensions.DependencyInjection/ExpressValidatorWithReload.cs`
  - SDK/Client: `FluentValidation` NuGet package
  - Auth: Not applicable

## Data Storage

**Databases:**
- None detected in this module
  - Connection: Not applicable
  - Client: Not applicable

**File Storage:**
- Local filesystem only for docs and test config files (`src/ExpressValidator.Extensions.DependencyInjection/docs/NuGet.md`, `tests/ExpressValidator.Extensions.DependencyInjection.Tests/appsettings.json`)

**Caching:**
- None detected

## Authentication & Identity

**Auth Provider:**
- None (library-level DI extension; no identity provider integration)
  - Implementation: Not applicable

## Monitoring & Observability

**Error Tracking:**
- None detected in this module

**Logs:**
- No explicit logging framework integration in `src/ExpressValidator.Extensions.DependencyInjection/*.cs`

## CI/CD & Deployment

**Hosting:**
- NuGet package publication target implied by packaging properties in `src/ExpressValidator.Extensions.DependencyInjection/ExpressValidator.Extensions.DependencyInjection.csproj`

**CI Pipeline:**
- Not detected from this module directory

## Environment Configuration

**Required env vars:**
- None required by the library code itself

**Secrets location:**
- Not applicable for this module

## Webhooks & Callbacks

**Incoming:**
- In-process callback via `IOptionsMonitor<TOptions>.OnChange` in `src/ExpressValidator.Extensions.DependencyInjection/OptionsMonitorContext.cs`

**Outgoing:**
- None detected

---

*Integration audit: 2026-04-26*
