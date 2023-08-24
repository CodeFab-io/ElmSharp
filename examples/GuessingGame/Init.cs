// 📃 Init.cs
namespace GuessingGame;

public static partial class ElmFuncs
{
    public static (Model, Cmd) Init() => 
        (InitModel, InitCmd);

    internal static Model InitModel { get; } =
        // We'll use -1 as init. This will be updated by the result of GetRandomNumberCommand
        new(NumberToBeGuessed: -1, 
            CurrentPlayerGuess: null);

    internal static Cmd InitCmd { get; } = 
        new Cmd.GetRandomNumberCommand(
            fromInclusive: 0, 
            toExclusive: 10,
            onRandomNumberGenerated: number => new Message.SecretNumberPicked(number));
}
