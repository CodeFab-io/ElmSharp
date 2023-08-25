using static GuessingGame_Tests.Helpers;

namespace GuessingGame_Tests;

public sealed class ViewTests 
{
    [Fact]
    public void View_WithoutGuess_DoesntStateTheGuess() 
    {
        var (model, _) = Init();
        var view = View(model, VoidDispatch);
        var viewStr = Assert.IsType<string>(view);

        Assert.DoesNotContain("Your guess", viewStr);
    }

    [Fact]
    public void View_WithLowerGuess_StatesTooLow() 
    {
        var (model, _) = 
            Init()
            .ApplyMessage(SecretNumberPicked(4))
            .ApplyMessage(PlayerPressedKey(2));

        var view = View(model, VoidDispatch);
        var viewStr = Assert.IsType<string>(view);

        Assert.Contains("too low", viewStr);
    }

    [Fact]
    public void View_WithHigherGuess_StatesTooHigh()
    {
        var (model, _) =
            Init()
            .ApplyMessage(SecretNumberPicked(4))
            .ApplyMessage(PlayerPressedKey(6));

        var view = View(model, VoidDispatch);
        var viewStr = Assert.IsType<string>(view);

        Assert.Contains("too high", viewStr);
    }

    [Fact]
    public void View_WithCorrectGuess_StatesCongratulations()
    {
        var (model, _) =
            Init()
            .ApplyMessage(SecretNumberPicked(5))
            .ApplyMessage(PlayerPressedKey(5));

        var view = View(model, VoidDispatch);
        var viewStr = Assert.IsType<string>(view);

        Assert.Contains("Congratulations", viewStr);
    }

    static readonly Action<Message> VoidDispatch = msg => { };
}
