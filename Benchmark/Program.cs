namespace Benchmark;

using System.Collections;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Bogus;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

[MemoryDiagnoser]
public static class Program
{
    public static void Main(string[] args)
    {
        var test = new Benchmark();
        var redisGetString = test.Redis_Get_string();
        var redisGetObject = test.Redis_Get_object();
        var redisGetSetString = test.Redis_GetSet_string();
        var redisGetSetObject = test.Redis_GetSet_object();
        var memoryCacheGetOrCreate = test.MemoryCache_Get();
        var lazyCacheGet = test.LazyCache_Get();
        BenchmarkRunner.Run<Benchmark>();
    }
}

public class Benchmark
{
    // private readonly Random _random;
    private readonly string _key;
    private readonly BenchmarkSessionDto _value;
    private readonly string _valueString;
    private readonly IAppCache _lazyCache;
    private readonly IMemoryCache _memoryCache;
    private readonly IDatabase _redis;

    public Benchmark()
    {
        _key = Guid.NewGuid().ToString();
        _value = new BenchmarkSessionDto(
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            false,
            "1234567890");
        _valueString = JsonSerializer.Serialize(_value);

        _lazyCache = new CachingService();
        _lazyCache.GetOrAdd(_key, () => _value);

        _memoryCache = new MemoryCache(
            new MemoryCacheOptions
            {
            });
        _memoryCache.GetOrCreate(_key, entry => _value);

        _redis = ConnectionMultiplexer.Connect(
                new ConfigurationOptions
                {
                    EndPoints = new EndPointCollection
                    {
                        "localhost"
                    }
                })
            .GetDatabase();
        var stringSet = _redis.StringSetAsync(_key, JsonSerializer.Serialize(_value), flags: CommandFlags.FireAndForget);

        // _random = new Random();
    }


    [Benchmark]
    public BenchmarkSessionDto MemoryCache_Get()
    {
        return _memoryCache.Get<BenchmarkSessionDto>(_key);
    }

    // [Benchmark]
    // public BenchmarkSessionDto MemoryCache_GetOrCreate()
    // {
    //     return _memoryCache.GetOrCreate(Guid.NewGuid().ToString(), entry => _value);
    // }

    [Benchmark]
    public BenchmarkSessionDto LazyCache_Get()
    {
        return _lazyCache.Get<BenchmarkSessionDto>(_key);
    }

    // [Benchmark]
    // public BenchmarkSessionDto LazyCache_GetOrAdd()
    // {
    //     return _lazyCache.GetOrAdd(Guid.NewGuid().ToString(), () => _value);
    // }

    [Benchmark]
    public string Redis_Get_string()
    {
        return _redis.StringGet(_key)!;
    }

    [Benchmark]
    public string Redis_GetSet_string()
    {
        return _redis.StringGetSet(_key, _valueString)!;
    }

    [Benchmark]
    public BenchmarkSessionDto Redis_Get_object()
    {
        var stringGet = _redis.StringGet(_key);
        return JsonSerializer.Deserialize<BenchmarkSessionDto>(stringGet!)!;
    }

    [Benchmark]
    public BenchmarkSessionDto Redis_GetSet_object()
    {
        var stringGetSet = _redis.StringGetSet(_key, JsonSerializer.Serialize(_value));

        return JsonSerializer.Deserialize<BenchmarkSessionDto>(stringGetSet!)!;
    }
}

public record BenchmarkSessionDto(
    Guid SessionId,
    DateTime ExpirationDate,
    bool UserIsDisabled,
    string CasinoSignatureKey);