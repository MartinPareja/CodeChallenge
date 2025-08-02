﻿namespace CarRental.Application.Abstractions;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null);
    Task RemoveAsync(string key);
}