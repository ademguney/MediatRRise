# MediatR Rise

**MediatRRise** is a modern, lightweight, and extensible in-process messaging library for .NET.  
Inspired by [MediatR](https://github.com/jbogard/MediatR), it provides a clean and powerful alternative with more flexibility, better testability, and performance-conscious design.

Whether you're building a modular monolith, implementing CQRS, or introducing domain events, **MediatRRise** helps you decouple your code in a clean and maintainable way.

---

## ✨ Features

- ✅ `IRequest<T>` / `IRequestHandler<T, R>` based command & query handling
- ✅ `INotification` & multi-handler pub-sub pattern
- ✅ Customizable pipeline with `IPipelineBehavior<T, R>`
- ✅ Built-in behaviors like validation, logging, retry (optional)
- ✅ Clean DI integration: `services.AddMediatRRise(...)`
- ✅ High testability with mockable abstractions
- ✅ NuGet-friendly, open-source, MIT licensed

---

## 📦 Installation

👉 [View on NuGet](https://www.nuget.org/packages/MediatRRise)

```bash
dotnet add package MediatRRise
```


## 📁 Project Structure

```bash
📁 MediatRRise/
├── 📁 Core/
│   └──📁 MediatRRise.Core.Abstractions/
│       ├──📄 IMediator.cs
│       ├──📄 IRequest.cs
│       ├──📄 IRequestHandler.cs
│       ├──📄 INotification.cs
│       ├──📄 INotificationHandler.cs
│       ├──📄 IPipelineBehavior.cs
│       ├──📄 ICacheableRequest.cs
│       ├──📄 ICacheService.cs
│       └──📄 RequestHandlerDelegate.cs

├──📁 Behaviors/
│   ├──📁 Caching/
│   │   └──🧩 CachingBehavior.cs
│   ├──📁 Logging/
│   │   └──🧩 LoggingBehavior.cs
│   ├──📁 Retry/
│   │   └──🧩 RetryBehavior.cs
│   ├──📁 Validation/
│   │   └──🧩 ValidationBehavior.cs
│   ├──📁 Performance/
│   │   └──🧩 PerformanceBehavior.cs
│   ├──📁 ExceptionHandling/
│   │   └──🧩 ExceptionHandlingBehavior.cs
│   └──📁 Extensions/
│       ├──🧩 AddCachingBehaviorExtensions.cs
│       ├──🧩 AddLoggingBehaviorExtensions.cs
│       ├──🧩 AddRetryBehaviorExtensions.cs
│       ├──🧩 AddValidationBehaviorExtensions.cs
│       ├──🧩 AddPerformanceBehaviorExtensions.cs
│       ├──🧩 AddExceptionHandlingBehaviorExtensions.cs
│       └──🧩 AddMediatRRisePipelineExtensions.cs

├──📁 Infrastructure/
│   ├── Implemantation/
│   │   └──⚙️ Mediator.cs
│   ├── Pipeline/
│   │   └──⚙️ PipelineExecutor.cs
│   └── Extensions/
│       ├──⚙️ ServiceCollectionExtensions.cs
│       └──⚙️ MediatRRiseOptions.cs

├──📁 Tests/ (optional but recommended)
│   └──🧪 MediatRRise.Tests/
│       ├──🧪 Unit/
│       ├──🧪 Integration/
│       └──🧪 TestFixtures/

├──📄🛠 MediatRRise.csproj
├──📘 README.md
└──📄 LICENSE

```
---


## 🔗 Example Usage — Sample App

A fully working sample project is available here:  
👉 [MediatRRise.SampleApp](https://github.com/ademguney/MediatRRise.SampleApp)

This project demonstrates:

- ASP.NET Core Web API
- Request/Handler implementation
- FluentValidation integration
- Pipeline behaviors in action (logging, validation, performance, caching)
- In-memory EF Core database
- Clean architecture with separated layers

### ▶️ Quick Start

```bash
git clone https://github.com/ademguney/MediatRRise.SampleApp.git
cd MediatRRise.SampleApp.Api
dotnet run
```

## 🧩 Adding MediatRRise to Your Project
To get started with MediatRRise, register it in your dependency injection container:
```csharp
using MediatRRise.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

var services = new ServiceCollection();

// Register MediatRRise handlers from this assembly
services.AddMediator(cfg =>
{
    cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
});
```
### 🔧 Adding Pipeline Behaviors
MediatRRise comes with built-in pipeline behaviors such as:

- Validation
- Logging
- Exception Handling
- Retry
- Performance Monitoring

Caching
✅ Option 1: Register All Behaviors at Once
You can register them individually or all at once:
```csharp
using MediatRRise.Behaviors.Extensions;

services.AddMediatRRisePipeline();
```
This will register all available behaviors in the recommended order:

- LoggingBehavior  
- ExceptionHandlingBehavior  
- ValidationBehavior  
- RetryBehavior  
- PerformanceBehavior  
- CachingBehavior

### ⚙️ Option 2: Register Behaviors Individually
If you prefer to have more control, you can add only the behaviors you need:

```csharp
using MediatRRise.Behaviors.Extensions;

services
    .AddLoggingBehavior()
    .AddExceptionHandlingBehavior()
    .AddValidationBehavior()
    .AddRetryBehavior()
    .AddPerformanceBehavior()
    .AddCachingBehavior();

```

### ⚙️ Available Pipeline Behaviors
MediatRRise includes a set of optional but powerful behaviors. You can plug them into the MediatR pipeline to handle cross-cutting concerns in a clean and modular way.

#### 🔍 ValidationBehavior
Validates incoming requests using FluentValidation before they are handled.

```csharp
services.AddValidationBehavior();
```
To enable validation, register your validators like this:
```csharp
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
```
#### 📄 LoggingBehaviorr
Logs when a request is being handled and when it completes.
```csharp
services.AddLoggingBehavior();
```
#### 🛡️ ExceptionHandlingBehavior
Catches and logs unhandled exceptions during request handling. Prevents your app from crashing due to unexpected failures.
```csharp
services.AddExceptionHandlingBehavior();
```
#### 🔁 RetryBehavior
Retries transient errors (like timeouts or HTTP failures) automatically. Default is 3 retries with 200ms delay.

```csharp
services.AddRetryBehavior();
```
You can customize retry logic by editing RetryBehavior<TRequest, TResponse> directly.

#### ⏱️ PerformanceBehavior
Measures how long each request takes. Logs a warning if it exceeds a certain threshold (default: 500ms).


```csharp
services.AddPerformanceBehavior();
```

#### 🧠 CachingBehavior
Caches the result of requests that implement ICacheableRequest<TResponse>. Requires an ICacheService implementation.

```csharp
services.AddCachingBehavior();

```
Define your query like this:
```csharp
public class GetProductByIdQuery : ICacheableRequest<ProductDto>
{
    public Guid Id { get; init; }

    public string CacheKey => $"product:{Id}";
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
```

#### 🔗 Tip: Register All Behaviors at Once
You can register all behaviors with a single call:


```csharp
services.AddMediatRRisePipeline();

```
