namespace Benchmark;

using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<Benchmark>();
    }
}

[MemoryDiagnoser()]
public class Benchmark
{
    [Params("asdasdq31")]
    public string value1;

    [Params(" asmc02ld=1,x 3")]
    public string value2;

    [Params(" sadc3cs1,x 3")]
    public string value3;

    [Benchmark]
    public string Join()
    {
        return string.Join("", value1, value2);
    }

    [Benchmark]
    public string Plus()
    {
        return value1 + value2;
    }

    [Benchmark]
    public string Concat()
    {
        return string.Concat(value1, value2);
    }

    [Benchmark]
    public string Format()
    {
        return string.Format("{0}{1}", value1, value2);
    }

    [Benchmark]
    public string Interpolation()
    {
        return $"{value1}{value2}";
    }

    [Benchmark]
    public string StringBuilder()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(value1);
        stringBuilder.Append(value2);
        return stringBuilder.ToString();
    }


    [Benchmark]
    public string Join3()
    {
        return string.Join(
            "",
            value1,
            value2,
            value3);
    }

    [Benchmark]
    public string Plus3()
    {
        return value1 + value2 + value3;
    }

    [Benchmark]
    public string Concat3()
    {
        return string.Concat(value1, value2, value3);
    }

    [Benchmark]
    public string Format3()
    {
        return string.Format(
            "{0}{1}{2}",
            value1,
            value2,
            value3);
    }

    [Benchmark]
    public string Interpolation3()
    {
        return $"{value1}{value2}{value3}";
    }

    [Benchmark]
    public string StringBuilder3()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(value1);
        stringBuilder.Append(value2);
        stringBuilder.Append(value3);
        return stringBuilder.ToString();
    }
}