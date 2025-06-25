# MediatR Rise

**MediatRRise** is a modern, lightweight, and extensible in-process messaging library for .NET.  
Inspired by [MediatR](https://github.com/jbogard/MediatR), it provides a clean and powerful alternative with more flexibility, better testability, and performance-conscious design.

Whether you're building a modular monolith, implementing CQRS, or introducing domain events, **MediatRRise** helps you decouple your code in a clean and maintainable way.

---

## ✨ Features

- ✅ `IRequest<T>` / `IRequestHandler<T, R>` based command & query handling
- ✅ `INotification` & multi-handler pub-sub pattern
- ✅ Customizable pipeline with `IPipelineBehavior<T, R>`
- ✅ Clean DI integration: services.AddMediator(...)
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
### ⚡️ Adding Pipeline Behaviors
MediatRRise supports IPipelineBehavior<TRequest, TResponse> out of the box.
You can define and register your own custom behaviors (such as validation, logging, caching, etc.) in your project.

Example — Validation Behavior Registration:

```csharp
services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MyCustomValidationBehavior<,>));
```

