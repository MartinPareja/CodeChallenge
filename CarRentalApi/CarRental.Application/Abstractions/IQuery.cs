using MediatR;

namespace CarRental.Application.Abstractions;

public interface IQuery<out TResponse> : IRequest<TResponse> { }