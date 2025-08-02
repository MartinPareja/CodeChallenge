﻿using MediatR;

namespace CarRental.Application.Abstractions;

public interface ICommand : IRequest { }

public interface ICommand<out TResponse> : IRequest<TResponse> { }