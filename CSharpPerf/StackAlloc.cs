using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;

[MemoryDiagnoser]
[Config(typeof(Program.DontForceGcCollectionsConfig))]
public class StackAlloc
{

    [Benchmark]
    public byte[] AllocateHeap()
    {
        return new byte[10000];
    }

    [Benchmark (Baseline = true)]
    public unsafe void AllocateWithStackalloc()
    {
        var array = stackalloc byte[10000];
        Consume(array);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static unsafe void Consume(byte* input)
    {
    }
}
