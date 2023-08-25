namespace GuessingGame_Tests;

public sealed class InitTests
{
    [Fact]
    public void Init_Returns_ExpectedModel()
    {
        var expected = new Model(NumberToBeGuessed: -1, CurrentPlayerGuess: null);
        var (actualModel, _) = Init();
        Assert.Equal(expected, actualModel);
    }

    [Fact]
    public void Init_Returns_ExpectedCmd() 
    {
        var (_, actualCmd) = Init();
        
        var cmd = Assert.IsType<Cmd.GetRandomNumberCommand>(actualCmd);

        Assert.Equal(0, cmd.FromInclusive);
        Assert.Equal(10, cmd.ToExclusive);
        Assert.IsType<SecretNumberPicked>(cmd.OnRandomNumberGenerated(0));
    }
}
