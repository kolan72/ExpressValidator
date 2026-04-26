# Technology Stack

**Analysis Date:** 2026-04-26

## Languages

**Primary:**
- C# - Library code in `src/ExpressValidator.Extensions.DependencyInjection/*.cs`

**Secondary:**
- XML/MSBuild - Project configuration in `src/ExpressValidator.Extensions.DependencyInjection/ExpressValidator.Extensions.DependencyInjection.csproj`
- Markdown - Package docs in `src/ExpressValidator.Extensions.DependencyInjection/README.md` and `src/ExpressValidator.Extensions.DependencyInjection/docs/NuGet.md`

## Runtime

**Environment:**
- .NET multi-target package: `netstandard2.0` and `net8.0` in `src/ExpressValidator.Extensions.DependencyInjection/ExpressValidator.Extensions.DependencyInjection.csproj`

**Package Manager:**
- NuGet (`PackageReference` in the library project)
- Lockfile: Not detected (`packages.lock.json` not present in this module)

## Frameworks

**Core:**
- `ExpressValidator` 0.15.0 - Validation builder/contracts consumed by extension methods
- `Microsoft.Extensions.DependencyInjection` 10.0.6 - DI container registration API
- `Microsoft.Extensions.Options.ConfigurationExtensions` 10.0.6 - options binding and named section reload support
- `FluentValidation` 11.11.0 (`netstandard2.0`) / 12.1.1 (`net8.0`) - result types and validator ecosystem compatibility

**Testing:**
- NUnit 4.4.0 in `tests/ExpressValidator.Extensions.DependencyInjection.Tests/ExpressValidator.Extensions.DependencyInjection.Tests.csproj`

**Build/Dev:**
- SDK-style library build via `Microsoft.NET.Sdk` in `src/ExpressValidator.Extensions.DependencyInjection/ExpressValidator.Extensions.DependencyInjection.csproj`
- Legacy non-SDK test project file in `tests/ExpressValidator.Extensions.DependencyInjection.Tests/ExpressValidator.Extensions.DependencyInjection.Tests.csproj`

## Key Dependencies

**Critical:**
- `ExpressValidator` 0.15.0 - defines `IExpressValidator<>`, builders, and profile base types used throughout `src/ExpressValidator.Extensions.DependencyInjection/*.cs`
- `Microsoft.Extensions.DependencyInjection` 10.0.6 - enables registration methods in `src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`

**Infrastructure:**
- `Microsoft.Extensions.Options*` 10.0.6 - dynamic configuration monitoring used by `src/ExpressValidator.Extensions.DependencyInjection/OptionsMonitorContext.cs`

## Configuration

**Environment:**
- No environment variables are required by library code in this module
- Options section names are passed explicitly (for reloadable validators) in `src/ExpressValidator.Extensions.DependencyInjection/ServiceCollectionExtensions.cs`

**Build:**
- Packaging metadata and multi-targeting in `src/ExpressValidator.Extensions.DependencyInjection/ExpressValidator.Extensions.DependencyInjection.csproj`
- Local solution entry in `src/ExpressValidator.Extensions.DependencyInjection/ExpressValidator.Extensions.DependencyInjection.sln`

## Platform Requirements

**Development:**
- .NET SDK capable of building `net8.0` and `netstandard2.0`
- NuGet restore for project dependencies

**Production:**
- Distributed as a NuGet package (`GeneratePackageOnBuild=true`) from `src/ExpressValidator.Extensions.DependencyInjection/ExpressValidator.Extensions.DependencyInjection.csproj`

---

*Stack analysis: 2026-04-26*
