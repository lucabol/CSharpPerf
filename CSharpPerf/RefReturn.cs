using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

// 1. ref to in, doesn't work

public struct BigValueType
{
    public int _1, _2, _3, _4, _5, _6;
}

[MemoryDiagnoser]
[Config(typeof(Program.ShortConfig))]
public class RefReturn
{
    private BigValueType field;

    [Benchmark(Baseline = true)]
    public ref BigValueType ReturnsByRef() => ref InitializeRef(ref field);

    [Benchmark]
    public BigValueType ReturnsByValue() => Initialize(field);

    private ref BigValueType InitializeRef(ref BigValueType reference)
    {
        reference._1 = 1;
        reference._2 = 2;
        reference._3 = 3;
        reference._4 = 4;
        reference._5 = 5;
        reference._6 = 6;

        return ref reference;
    }

    private BigValueType Initialize(BigValueType value)
    {
        value._1 = 1;
        value._2 = 2;
        value._3 = 3;
        value._4 = 4;
        value._5 = 5;
        value._6 = 6;

        return value;
    }
}
