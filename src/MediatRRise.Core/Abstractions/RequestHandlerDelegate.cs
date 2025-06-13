namespace MediatRRise.Core.Abstractions;

/// <summary>
/// Delegate that represents the next step in the request pipeline.
/// Typically, this is the handler itself or the next behavior.
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();