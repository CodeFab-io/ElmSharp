using ElmSharp;
using static System.Security.Cryptography.RandomNumberGenerator;

namespace ElmSharp_Tests.CommandTests;

public sealed class GetRandomNumberCommandTests
{
    sealed record TestModel();

    abstract record TestMessage 
    {
        internal sealed record RandomNumberGenerated(int randomNumber) : TestMessage {}
    }

    [Fact]
    public async Task GetRandomNumberCommand_NumberBetweenFromInclusive_and_ToExclusive()
    {
        var test = async () =>
        {
            var fromInclusive = GetInt32(0, 100);
            var toExclusive = fromInclusive + GetInt32(1, 10);

            var sut = new ElmSharp<TestModel, TestMessage>.Command.GetRandomNumberCommand(
                fromInclusive: fromInclusive,
                toExclusive: toExclusive,
                onRandomNumberGenerated: num => new TestMessage.RandomNumberGenerated(num));

            var result = await sut.Run();

            var msg = Assert.IsType<TestMessage.RandomNumberGenerated>(result);
            Assert.InRange(msg.randomNumber, low: fromInclusive, high: toExclusive - 1);
        };

        await test.Repeat(1000);
    }
}
