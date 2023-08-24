// 📃 Program.cs
using ElmSharp;
using GuessingGame;

await ElmSharp<Model, Message>.Run(
    init: ElmFuncs.Init,
    update: ElmFuncs.Update,
    view: ElmFuncs.View,
    subscriptions: ElmFuncs.Subscriptions);