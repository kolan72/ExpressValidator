
This folder contains **sample applications** demonstrating different ways to use **ExpressValidator.Extensions.DependencyInjection** in real-world scenarios.

---

## 🎯 Basic Guess Validation (`/guess`)

This sample demonstrates the **simplest usage** of `ExpressValidator.Extensions.DependencyInjection`.

> The application tries to **guess a number** for you, validates it, and returns a response indicating whether the guess is correct or not.

### What it shows

* Registering an `IExpressValidator<T>` using `AddExpressValidator`
* Defining validation rules at application startup
* Injecting and using the validator in a consuming service
* Returning validation results from an API endpoint

**Endpoint**

```
GET /guess
```

**Best for**

* Static validation rules
* Quick setup
* Learning the basics of ExpressValidator
---

## 🧩 Validation Using `ValidatorConfigurator<T>`

This sample demonstrates how to define validation rules in a **dedicated configurator class** instead of configuring them inline at startup.

> The application validates the guessed number using an `IExpressValidator<T>` that is configured through a `ValidatorConfigurator<T>` implementation and registered automatically via assembly scanning.

### What it shows

* Creating a class that inherits from `ValidatorConfigurator<T>`
* Centralizing validation rules in a reusable and testable component
* Registering validators using `AddExpressValidation`
* Keeping `Program.cs` clean and focused on application wiring

### How it works

1. A `ValidatorConfigurator<T>` defines all validation rules for the target type.
2. The application registers all configurators from the assembly at startup.
3. An `IExpressValidator<T>` is resolved from DI and used in the service.
4. The endpoint returns a validation result based on the configured rules.

### Endpoint

```
GET /guess
```

*(Uses the same endpoint as the basic sample, but with a different validator registration strategy.)*

### Best for

* Clean architecture and separation of concerns
* Larger projects with many validators
* Shared validation logic across multiple services

---

## ⚙️ Advanced Validation with Options (`/complexguess`)

This sample demonstrates **dynamic validation rules** driven by runtime options.

> The `/complexguess` endpoint validates the number using
> `IExpressValidatorBuilder<ObjToValidate, ValidationParametersOptions>`,
> where validation parameters can change dynamically.

### What it shows

* Using `AddExpressValidatorBuilder`
* Building validators per request using options
* Injecting `IOptions<T>` into services
* Adjusting validation behavior without redeploying code

**Endpoint**

```
GET /complexguess
```

**Best for**

* Configurable business rules
* Feature flags
* Per-request validation logic

---

## 🔄 Validation with Live Configuration Reload

### (`/guesswithreload`, `/guesswithreloadasync`)

This sample demonstrates **real-time validation updates** when configuration changes.

> Use `/guesswithreload` or `/guesswithreloadasync` to change the parameters of the FluentValidation validators **in real time**, without restarting the application.

### What it shows

* Using `AddExpressValidatorWithReload`
* Automatically reloading validation rules on configuration changes
* Sync and async validation APIs
* Integration with `IOptionsMonitor<T>`

**Endpoints**

```
GET /guesswithreload
GET /guesswithreloadasync
```

**Best for**

* Live configuration updates

## ▶️ Running the Samples

1. Navigate to the desired sample project
2. Run the application:

   ```
   dotnet run
   ```
3. Call the endpoints using a browser, curl, or Swagger UI
