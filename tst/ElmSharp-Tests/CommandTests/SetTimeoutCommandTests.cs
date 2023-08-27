using ElmSharp;

namespace ElmSharp_Tests.CommandTests;

public sealed class SetTimeoutCommandTests
{
    sealed record TestModel();

    abstract record TestMessage
    {
        internal sealed record TimeoutElapsed : TestMessage { }
    }

    [Fact]
    public async Task SetTimeoutCommand_WaitsAndTriggersTimeoutMessage() 
    {
        var expectedDuration = TimeSpan.FromSeconds(1);
        var sut = new ElmSharp<TestModel, TestMessage>.Command.SetTimeoutCommand(
            timeoutDuration: expectedDuration,
            onTimeoutElapsed: () => new TestMessage.TimeoutElapsed());
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await sut.Run();
        var actualDuration = stopwatch.Elapsed;

        var msg = Assert.IsType<TestMessage.TimeoutElapsed>(result);
        Assert.InRange(actualDuration, expectedDuration, expectedDuration.Add(TimeSpan.FromMilliseconds(100)));
    }

    [Fact]
    public async Task SetTimeoutCommand_WithNegativeTimeout_DoesntDelay()
    {
        var expectedDuration = TimeSpan.Zero;
        var invalidDuration = TimeSpan.FromSeconds(-1);

        var sut = new ElmSharp<TestModel, TestMessage>.Command.SetTimeoutCommand(
            timeoutDuration: invalidDuration,
            onTimeoutElapsed: () => new TestMessage.TimeoutElapsed());

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await sut.Run();
        var actualDuration = stopwatch.Elapsed;

        var msg = Assert.IsType<TestMessage.TimeoutElapsed>(result);
        Assert.InRange(actualDuration, expectedDuration, expectedDuration.Add(TimeSpan.FromMilliseconds(10)));
    }
}
