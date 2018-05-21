using System.Reflection;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Horology;

class Program
{
    static string[] tests =
    {
        //"RefReturn",
        //"StackAlloc",
        //"SpanSplicing",
        "ChessEngine"
    };

    static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(Program).GetTypeInfo().Assembly).Run(tests);
    }

    private static int launchCount = 2;
    private static int targetCount = 10;
    private static int warmupCount = 15;
    private static int iterationTime = 100;

    public class ShortConfig : ManualConfig
    {
        public ShortConfig()
        {
            Add(Job.Default
                .WithLaunchCount(launchCount)
                .WithTargetCount(targetCount)
                .WithWarmupCount(warmupCount)
                .WithIterationTime(TimeInterval.Millisecond * iterationTime));
        }
    }

    public class DontForceGcCollectionsConfig : ManualConfig
    {
        public DontForceGcCollectionsConfig()
        {
            Add(Job.Default
                .WithGcForce(false)
                .WithLaunchCount(launchCount)
                .WithTargetCount(targetCount)
                .WithWarmupCount(warmupCount)
                .WithIterationTime(TimeInterval.Millisecond * iterationTime));
        }
    }

}


