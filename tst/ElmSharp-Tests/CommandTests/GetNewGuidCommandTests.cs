using ElmSharp;

namespace ElmSharp_Tests.CommandTests;

public sealed class GetNewGuidCommandTests 
{ 
    sealed record TestModel();

    abstract record TestMessage
    {
        internal sealed record NewGuidGenerated(Guid NewGuid) : TestMessage {}
    }

    [Fact]
    public async Task GetNewGuidCommand_Generates_NewGuid() 
    {
        var sut = new ElmSharp<TestModel, TestMessage>.Command.GetNewGuidCommand(
            onNewGuidGenerated: guid => new TestMessage.NewGuidGenerated(guid));
        
        var result = await sut.Run();

        var msg = Assert.IsType<TestMessage.NewGuidGenerated>(result);
        Assert.NotEqual(Guid.Empty, msg.NewGuid);
    }
}
