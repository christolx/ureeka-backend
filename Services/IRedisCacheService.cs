﻿namespace ureeka_backend.Services;

public interface IRedisCacheService
{
    T? GetData<T>(string key);
    void SetData<T>(string key, T value);
}