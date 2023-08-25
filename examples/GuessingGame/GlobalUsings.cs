// 📃 GlobalUsings.cs
global using Cmd = ElmSharp.ElmSharp<GuessingGame.Model, GuessingGame.Message>.Command;
global using Sub = ElmSharp.ElmSharp<GuessingGame.Model, GuessingGame.Message>.Subscription;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo($"{nameof(GuessingGame)}-Tests")]
[assembly: CLSCompliant(false)]