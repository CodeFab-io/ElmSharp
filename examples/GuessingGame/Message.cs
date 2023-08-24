// 📃 Message.cs
namespace GuessingGame;

public abstract record Message
{
    public sealed record SecretNumberPicked(int SecretNumber) : Message { }

    public sealed record PlayerPressedKey(ConsoleKeyInfo KeyInfo) : Message { }

    public sealed record TimeoutElapsed : Message { }
}
