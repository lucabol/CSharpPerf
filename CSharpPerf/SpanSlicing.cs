using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using System;
using System.Linq;

[MemoryDiagnoser]
[Config(typeof(Program.DontForceGcCollectionsConfig))]
public class SpanSplicing
{
    private string Text;

    [Params(10, 1000)]
    public int CharactersCount { get; set; }

    [GlobalSetup]
    public void Setup() => Text = new string(Enumerable.Repeat('a', CharactersCount).ToArray());

    [Benchmark]
    public string Substring() => Text.Substring(0, Text.Length / 2);

    [Benchmark(Baseline = true)]
    public ReadOnlySpan<char> Slice() => Text.AsSpan().Slice(0, Text.Length / 2);
}
