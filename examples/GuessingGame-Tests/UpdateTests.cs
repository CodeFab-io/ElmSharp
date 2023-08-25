using static System.Security.Cryptography.RandomNumberGenerator;
using static GuessingGame_Tests.Helpers;

namespace GuessingGame_Tests;

public sealed class UpdateTests
{
    [Fact]
    public void Update_WhenSecretNumberPicked_StoresSecretNumber() 
    {
        var secretNumber = GetInt32(0, 10);

        var (model, cmd) = Init().ApplyMessage(new SecretNumberPicked(SecretNumber: secretNumber));

        Assert.Equal(secretNumber, model.NumberToBeGuessed);
        Assert.Equal(cmd, Cmd.None);
    }

    [Fact]
    public void Update_WhenPlayerMakesGuess_RemembersOnModel()
    { 
        var playerGuess = GetInt32(0, 10);

        var message = PlayerPressedKey(playerGuess);

        var (model, _) = Init().ApplyMessage(message);

        Assert.Equal(playerGuess, model.CurrentPlayerGuess);
    }

    [Fact]
    public void Update_WhenPlayerGuessesWrong_ReturnsNoCmd()
    {
        var secretNumber = GetInt32(0, 5);
        var wrongGuess = GetInt32(5, 10);

        var secretNumberPicked = SecretNumberPicked(secretNumber);
        var message = PlayerPressedKey(wrongGuess);

        var (_, cmd) = 
            Init()
            .ApplyMessage(secretNumberPicked)
            .ApplyMessage(message);

        Assert.Equal(Cmd.None, cmd);
    }

    [Fact]
    public void Update_WhenPlayerGuessesCorrectNumber_ReturnsSetTimeoutCmd() 
    {
        var secretNumber = GetInt32(0, 10);

        var secretNumberPicked = SecretNumberPicked(secretNumber);
        var message = PlayerPressedKey(secretNumber);

        var (_, cmd) =
            Init()
            .ApplyMessage(secretNumberPicked)
            .ApplyMessage(message);

        var setTimeoutCommand = Assert.IsType<Cmd.SetTimeoutCommand>(cmd);

        Assert.Equal(TimeSpan.FromSeconds(2), setTimeoutCommand.TimeoutDuration);
        
        // Check that the command produces the desired message type
        Assert.IsType<TimeoutElapsed>(setTimeoutCommand.OnTimeoutElapsed());
    }

    [Fact]
    public void Update_WhenTimeoutElapsed_ReturnsStopAppCommand() 
    {
        var (_, cmd) = Init().ApplyMessage<TimeoutElapsed>();

        var stopAppCmd = Assert.IsType<Cmd.StopAppCommand>(cmd);
        Assert.Equal(0, stopAppCmd.ExitCode);
    }
}