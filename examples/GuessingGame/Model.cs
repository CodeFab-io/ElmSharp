// 📃 Model.cs
namespace GuessingGame;

public record Model(
    int NumberToBeGuessed,
    int? CurrentPlayerGuess);
