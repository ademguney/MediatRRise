# MediatR Rise

**MediatRRise** is a modern, lightweight, and extensible in-process messaging library for .NET.  
Inspired by [MediatR](https://github.com/jbogard/MediatR), it provides a clean and powerful alternative with more flexibility, better testability, and performance-conscious design.

Whether you're building a modular monolith, implementing CQRS, or introducing domain events, **MediatRRise** helps you decouple your code in a clean and maintainable way.

---

### ✨ Features

- ✅ `IRequest<T>` / `IRequestHandler<T, R>` based command & query handling
- ✅ `INotification` & multi-handler pub-sub pattern
- ✅ Customizable pipeline with `IPipelineBehavior<T, R>`
- ✅ Built-in behaviors like validation, logging, retry (optional)
- ✅ Clean DI integration: `services.AddMediatRRise(...)`
- ✅ High testability with mockable abstractions
- ✅ NuGet-friendly, open-source, MIT licensed

---

### 📦 Installation

```bash
dotnet add package MediatRRise
