# Testing Patterns

**Analysis Date:** 2026-04-26

## Test Framework

**Runner:**
- NUnit 4.4.0
- Config: `tests/ExpressValidator.Extensions.DependencyInjection.Tests/ExpressValidator.Extensions.DependencyInjection.Tests.csproj`

**Assertion Library:**
- NUnit assertions (`Assert.That`, `Assert.Throws`, `Assert.DoesNotThrow`)

**Run Commands:**
```bash
dotnet test tests/ExpressValidator.Extensions.DependencyInjection.Tests/ExpressValidator.Extensions.DependencyInjection.Tests.csproj
dotnet test src/ExpressValidator.Extensions.DependencyInjection/ExpressValidator.Extensions.DependencyInjection.sln
dotnet test ExpressValidator.Extensions.DependencyInjection.sln
```

## Test File Organization

**Location:**
- Separate test project in `tests/ExpressValidator.Extensions.DependencyInjection.Tests/`.

**Naming:**
- Test files end with `Tests.cs`.
- Large fixture split via partial class files (`ServiceCollectionExtensionsTests.cs`, `ServiceCollectionExtensionsTests.AddExpressValidation.cs`).

**Structure:**
```
tests/ExpressValidator.Extensions.DependencyInjection.Tests/
|-- *Tests.cs
|-- ObjectsToTests.cs
|-- appsettings.json
```

## Test Structure

**Suite Organization:**
```csharp
[TestFixture]
public class AddExpressValidationIntegrationTests
{
    [SetUp] public void SetUp() { ... }
    [TearDown] public void TearDown() { ... }
    [Test] public void Should_ResolveValidator_WhenConfiguratorIsRegistered() { ... }
}
```

**Patterns:**
- Setup builds a fresh `ServiceCollection`.
- Tests resolve services through `BuildServiceProvider`.
- Assertions check lifetime, registration, nullability, and behavior outcomes.

## Mocking

**Framework:** Hand-rolled test doubles (no dedicated mocking library detected).

**Patterns:**
```csharp
public class TestOptionsMonitor : IOptionsMonitor<ObjectToValidateOptions>
{
    public ObjectToValidateOptions CurrentValue { get; }
    public IDisposable OnChange(Action<ObjectToValidateOptions, string> listener) => new EmptyDisposable();
}
```

**What to Mock:**
- `IOptionsMonitor<T>` and ambient configuration primitives when testing options update behavior.

**What NOT to Mock:**
- DI container registration paths; tests use real `ServiceCollection`.

## Fixtures and Factories

**Test Data:**
```csharp
var testPerson = new TestPersonModel { Name = "John", Age = 25 };
var services = new ServiceCollection();
```

**Location:**
- Shared test models/options in `tests/ExpressValidator.Extensions.DependencyInjection.Tests/ObjectsToTests.cs`.
- Additional per-suite model types inside fixture files.

## Coverage

**Requirements:** No explicit coverage threshold file detected.

**View Coverage:**
```bash
dotnet test /p:CollectCoverage=true
```

## Test Types

**Unit Tests:**
- Registration and helper class behavior in `tests/ExpressValidator.Extensions.DependencyInjection.Tests/ServiceCollectionExtensionsTests.cs` and `tests/ExpressValidator.Extensions.DependencyInjection.Tests/OptionsMonitorContextTests.cs`.

**Integration Tests:**
- Service resolution and lifetimes across DI container in `tests/ExpressValidator.Extensions.DependencyInjection.Tests/AddExpressValidationIntegrationTests.cs`.

**E2E Tests:**
- Not used in this module.

## Common Patterns

**Async Testing:**
```csharp
var result = await validator.ValidateAsync(testPerson);
Assert.That(result, Is.Not.Null);
```

**Error Testing:**
```csharp
Assert.Throws<InvalidOperationException>(() =>
    _serviceProvider.GetRequiredService<IExpressValidator<NonExistentModel>>());
```

---

*Testing analysis: 2026-04-26*
