// 📃 Update.cs
using static GuessingGame.Message;

namespace GuessingGame;

public static partial class ElmFuncs
{
    public static (Model, Cmd) Update(Message message, Model model) => message switch
    {
        SecretNumberPicked msg =>
            model.OnSecretNumberPicked(msg.SecretNumber),

        PlayerPressedKey msg =>
            model.OnPlayerPressedKey(pressedKey: msg.KeyInfo.Key),

        TimeoutElapsed =>
            model.OnTimeoutElapsed(),
    };

    internal static (Model, Cmd) OnSecretNumberPicked(this Model model, int secretNumber) =>
        (model with { NumberToBeGuessed = secretNumber }, Cmd.None);

    internal static (Model, Cmd) OnPlayerPressedKey(
        this Model model,
        ConsoleKey pressedKey) => pressedKey switch
        {
            // Q is the key for quitting the app
            ConsoleKey.Q => 
                (model, Cmd.StopAppWithCode(exitCode: 0)),

            // Numbers between 0 and 9
            >= ConsoleKey.D0 and <= ConsoleKey.D9 =>
                model.WithPlayerGuess(pressedKey - ConsoleKey.D0),

            // NumPad numbers between 0 and 9
            >= ConsoleKey.NumPad0 and <=ConsoleKey.NumPad9 =>
                model.WithPlayerGuess(pressedKey - ConsoleKey.NumPad0),

            // No-op for other key-presses
            _ => 
                (model, Cmd.None)
        };

    internal static (Model, Cmd) WithPlayerGuess(this Model model, int playerGuess)
    {
        // The player had already guessed the number, we don't change the model (ignore the new guess)
        if (model.NumberToBeGuessed == model.CurrentPlayerGuess)
            return (model, Cmd.None);

        // We check if the player has guessed the number. If so, we set a timeout to be notified later
        var command = model.NumberToBeGuessed != playerGuess
            ? Cmd.None 
            : new Cmd.SetTimeoutCommand(
                timeoutDuration: TimeSpan.FromSeconds(2),
                onTimeoutElapsed: () => new TimeoutElapsed());

        return (model with { CurrentPlayerGuess = playerGuess }, command);
    }

    internal static (Model, Cmd) OnTimeoutElapsed(
        this Model model) =>
        (model, Cmd.StopAppWithCode(exitCode: 0));
}
