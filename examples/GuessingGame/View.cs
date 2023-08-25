// 📃 View.cs
namespace GuessingGame;

public static partial class ElmFuncs
{
    public static object View(Model model, Action<Message> dispatch) =>
        $"\n" +
        $"  ╔═══════════════╗\n" +
        $"  ║ Guessing game ║\n" +
        $"  ╚═══════════════╝\n" +
        $"\n" +
        $"  Please choose a number between 0 and 9 or press [q] to Quit\n" +
        $"{model.PlayerGuessView()}";

    internal static string PlayerGuessView(this Model model) 
    {
        // The player hasn't made a guess yet
        if (model.CurrentPlayerGuess is not int playerGuess) 
            return string.Empty;
        
        var guessQuality =
            (playerGuess < model.NumberToBeGuessed) ? "too low." :
            (playerGuess > model.NumberToBeGuessed) ? "too high." :
            "perfect! Congratulations! (★‿★)";

        return $"\n  You guessed [{playerGuess}]. Your guess is {guessQuality}\n";
    }
}
