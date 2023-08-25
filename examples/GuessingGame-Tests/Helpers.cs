namespace GuessingGame_Tests;

internal static class Helpers
{
    internal static (Model, Cmd) ApplyMessage(this Model model, Message message) =>
        Update(message, model);

    internal static (Model, Cmd) ApplyMessage(this (Model, Cmd) mc, Message message) =>
        Update(message, mc.Item1);

    internal static (Model, Cmd) ApplyMessage<TMessage>(this (Model, Cmd) mc) where TMessage : Message, new() =>
        Update(new TMessage(), mc.Item1);

    internal static SecretNumberPicked SecretNumberPicked(int number) =>
        new(SecretNumber: number);

    internal static PlayerPressedKey PlayerPressedKey(int number) =>
        new(KeyInfo: ConsoleKeyInfo(number));

    internal static ConsoleKeyInfo ConsoleKeyInfo(int number) =>
        new(keyChar: (char)('0' + number),
            key: ConsoleKey.D0 + number,
            shift: false,
            alt: false,
            control: false);
}
