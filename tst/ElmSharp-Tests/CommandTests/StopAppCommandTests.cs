using ElmSharp;
using static System.Security.Cryptography.RandomNumberGenerator;
using Cmd = ElmSharp.ElmSharp<ElmSharp_Tests.CommandTests.StopAppCommandTests.TestModel, ElmSharp_Tests.CommandTests.StopAppCommandTests.TestMessage>.Command;
using Sub = ElmSharp.ElmSharp<ElmSharp_Tests.CommandTests.StopAppCommandTests.TestModel, ElmSharp_Tests.CommandTests.StopAppCommandTests.TestMessage>.Subscription;

namespace ElmSharp_Tests.CommandTests;

public sealed class StopAppCommandTests 
{ 
    public sealed record TestModel();

    public abstract record TestMessage 
    {
        public sealed record StopAppMessage : TestMessage;
    }

    [Fact]
    public async Task StopAppCommand_FromInit_ReturnsWithExitCode() 
    {
        var expectedExitCode = GetInt32(0, 10);
        var stopAppCommand = Cmd.StopAppWithCode(expectedExitCode);

        var exitCode = await ElmSharp<TestModel, TestMessage>.Run(
            init: () => (new TestModel(), stopAppCommand),
            update: (msg, model) => (model, Cmd.None),
            view: (model, dispatcher) => new { },
            subscriptions: model => Sub.None);

        Assert.Equal(expectedExitCode, exitCode);
    }

    [Fact]
    public async Task StopAppCommand_FromUpdate_ReturnsWithExitCode()
    {
        var expectedExitCode = GetInt32(0, 10);
        var stopAppCommand = Cmd.StopAppWithCode(expectedExitCode);

        var exitCode = await ElmSharp<TestModel, TestMessage>.Run(
            init: () => (new TestModel(), Cmd.None),
            update: (msg, model) => (model, stopAppCommand),
            view: (model, dispatcher) => { dispatcher(new TestMessage.StopAppMessage()); return new {}; },
            subscriptions: model => Sub.None);

        Assert.Equal(expectedExitCode, exitCode);
    }
}
