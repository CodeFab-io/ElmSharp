// 📃 Subscriptions.cs
using System.Collections.Immutable;

namespace GuessingGame;

public static partial class ElmFuncs
{
    public static ImmutableDictionary<string, Sub> Subscriptions(Model model) =>
        ImmutableDictionary<string, Sub>
            .Empty
            .Add(nameof(Sub.ConsoleKeyPressSubscription),
                 new Sub.ConsoleKeyPressSubscription(
                     onKeyPress: keyInfo => new Message.PlayerPressedKey(keyInfo)));
}
