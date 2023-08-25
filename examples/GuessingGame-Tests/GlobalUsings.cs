global using GuessingGame;
global using static GuessingGame.ElmFuncs;
global using static GuessingGame.Message;
global using Xunit;

global using Cmd = ElmSharp.ElmSharp<GuessingGame.Model, GuessingGame.Message>.Command;
global using Sub = ElmSharp.ElmSharp<GuessingGame.Model, GuessingGame.Message>.Subscription;