# 🍀 Let's build: Guessing game

## The premise of the game

In this application, we'll create a "game" where the computer "thinks" of a number between `0` and `9` and the player tries to guess the number. On each guess, the computer will tell the player if the number is greater or lower than the correct one, or if the player guessed the number. We'll use a console application for this example.

## 🚀 Initial dotnet commands

Let's assume you have the [dotnet CLI installed](https://learn.microsoft.com/en-us/dotnet/core/install/windows) on your machine. Here are the initial commands to setup an ElmSharp application.

```
mkdir GuessingGame
cd GuessingGame
dotnet new console
dotnet add package codefab.io.ElmSharp --prerelease
```

> 🚧 There are plans to create a dotnet tool to help with the initial boilerplate.

You can now open the project with you favorite editor, perhaps [jetbrains Rider](https://www.jetbrains.com/rider/), perhaps [Visual Studio Code](https://code.visualstudio.com/), perhaps [Microsoft Visual Studio](https://visualstudio.microsoft.com/) itself.

If you wish you can clear the default contents of `Program.cs`.

## 🔢 `Model`

Our `Model` will need to hold two values: the number to be guessed, and the player's current guess. We create a new `Model.cs` file with the following:

```csharp
// 📃 Model.cs
namespace GuessingGame;

public record Model(
    int NumberToBeGuessed,
    int? CurrentPlayerGuess);

```

## ✉ `Message`

We now need our initial `Message` declaration. It doesn't have to be complete, as our program will evolve over time. In fact, for now, we won't have any messages, and we'll add them as we go. So we create a `Message.cs` file:

```csharp
// 📃 Message.cs
namespace GuessingGame;

public abstract record Message
{
}
```

## 🌍 Global Usings

Now that we have both a `Model` and a `Message` we can create a `GlobalUsings.cs` file to make future code much easier:

```csharp
// 📃 GlobalUsings.cs
global using Cmd = ElmSharp.ElmSharp<GuessingGame.Model, GuessingGame.Message>.Command;
global using Sub = ElmSharp.ElmSharp<GuessingGame.Model, GuessingGame.Message>.Subscription;
```

## ✨ `Init`

We can now create our `Init` function. For the very first iteration of our code, we'll have the most boring game in the world, where the secret number is always `3`. (🤫 it's our little secret, nobody will know).

Let's create an `Init.cs` file, and due to the fact that csharp doesn't allow top-level functions, we cheat a little and create a `static partial class ElmFuncs`. The `partial` is due to the fact that we'll be using this `ElmFuncs` (or whatever you decide to name it!) for other upcoming functions.

```csharp
// 📃 Init.cs
namespace GuessingGame;

public static partial class ElmFuncs
{
    public static (Model, Cmd) Init() => 
        (InitModel, InitCmd);

    internal static Model InitModel { get; } =
        new (NumberToBeGuessed: 3, CurrentPlayerGuess: null);

    internal static Cmd InitCmd { get; } = 
        Cmd.None;
}
```

As a reminder, `Init()` will be invoked by ElmSharp runtime once, when the application starts. This is where we let ElmSharp know what is the "zero" of our `Model`, and what `Command`s to run. Spoiler alert: for the next iteration of our game, we'll ask ElmSharp to give us a random number, instead of 3, but we'll get there.

## ♻ `Update`

Given that we don't yet have any `Message` defined in our game, our `Update` function will pretty much be a no-op. Nevertheless, that is our starting point, so let's create it in a new file called `Update.cs`. As before, we use the `partial class ElmFuncs` trick:

```csharp
// 📃 Update.cs
namespace GuessingGame;

public static partial class ElmFuncs
{
    public static (Model, Cmd) Update(Message message, Model model) =>
        (model, Cmd.None);
}
```

In a normal (and upcoming) implementation of `Update` we will be doing `message switch { A => ..., B => ... };` but for now we don't have any messages declared yet, so the `Update` is pretty much a constant no-op (meaning: keep the same model, and don't run any `Command`).

## ☎ `Subscriptions`

Subscriptions are what allow us to hook up to "non-view events". Imagine that at some point we want to have a count-down where the player would only have a few seconds to choose a number: we could subscribe to Time and ElmSharp would send us a `Message` whenever a certain amount of time elapses. For now we won't have subscriptions, but we'll pretty soon have our first subscription to KeyPresses. As before, we leverage the `partial class ElmFuncs` trick in a new `Subscriptions.cs` file:

```csharp
// 📃 Subscriptions.cs
using System.Collections.Immutable;

namespace GuessingGame;

public static partial class ElmFuncs
{
    public static ImmutableDictionary<string, Sub> Subscriptions(Model model) =>
        Sub.None;
}
```

## 👀 `View`

The final piece of our game is our `View`. In a normal elm application the `View` function would return the desired Html to show in the browser but that is a luxury we don't have yet. So, the current version of ElmSharp expects the `View` function to return a `string`, otherwise...dragons happen 😅

ElmSharp will do a `Console.Clear()` before rendering the result of the `View` function, so that is something to keep in mind.

Let's create a `View.cs` file and once again use the `partial class ElmFuncs` trick:

```csharp
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
        $"  Please choose a number between 0 and 9 or press [q] to Quit";
}
```

## 💣 `Main`

Okay, it is time to wire everything together and see if we get some output on our console. 🤞 fingers crossed!

If you don't have it already you can create a `Program.cs` file. Otherwise modify the default one. We leverage the power of top-level statements, so it will look like this:

```csharp
// 📃 Program.cs
using ElmSharp;
using GuessingGame;

await ElmSharp<Model, Message>.Run(
    init: ElmFuncs.Init,
    update: ElmFuncs.Update,
    view: ElmFuncs.View,
    subscriptions: ElmFuncs.Subscriptions);
```

We are now able to run the most boring game in the universe, by running `dotnet run` in our console.

As expected, it is a **very** boring game, since we can't guess a number, or even [quit the application](https://stackoverflow.com/questions/11828270/how-do-i-exit-vim). Let's fix that, shall we? 💪 (And, for now you can use `Ctrl+C` to "quit" the game 😅).

## 🚪 Add feature: quit the game

ElmSharp comes with some built-in commands, and the one we are interested in is `StopAppCommand` which can be obtained via `Command.StopAppWithCode(int exitCode)`. This sounds like a perfect candidate for the "quit the game" feature.

However, we must keep two things in mind: a `Command` can only come from the `Init` or `Update` functions. Given that we don't want our application to quit upon start, we must do it in the `Update` function. The hurdle is that `Update` can only be triggered by a `Message`. So we need some kind of message that lets us know the player has pressed a key. For this we can use a `Subscription`.

Our goal will be:

* Create a new `Message`: `PlayerPressedKey`

* Add a `Subscription` that will listen to keyboard key presses and send this `PlayerPressedKey` message

* Adjust the `Update` function to react accordingly to our new `PlayerPressedKey` message

Let's do this 🤘.

## ➕ Creating a new `Message`

We can add the new message to our `Message.cs` file:

```csharp
// 📃 Message.cs
namespace GuessingGame;

public abstract record Message
{
    public sealed record PlayerPressedKey(ConsoleKeyInfo KeyInfo) : Message { }
}
```

Notice that the message carries data with it. In this example the data is a `ConsoleKeyInfo KeyInfo`. The subscription will force, via the constructor, that you specify a `Message` which can carry this particular piece of data with it.

## ➕ Subscribing to `ConsoleKeyPress`

We can now modify our `Subscriptions.cs` file, to return a subscription to ElmSharp:

```csharp
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
```

Okay, wow that's a mouthful piece of code, let's break it down. The way subscriptions work is that ElmSharp will use the dictionary key to manage them for you. The dictionary key is your unique identifier of a particular subscription configuration.

In this piece of code, we are returning a `Dictionary`. Always keep in mind that as a user, your code is pure. So constructing a subscription has no side-effects. It will be ElmSharp itself that keeps track of "Hey, you didn't have a subscription named 'banana' before, so let me wire that up for you. Also, I notice that you no longer returned a subscription named 'cat-alert', so I'll tear it down for you.". Behind the scenes, ElmSharp uses `Task`s and `CancellationToken`s to clean everything up, but as an ElmSharp user this is not something you need to worry about. Just keep in mind that if you keep the same `key`, ElmSharp won't make any subscription management for you.

> 🐉 A potential cause for bugs: if you make adjustments to the subscription instance, but keep the same key, ElmSharp won't do any management for you. Imagine you have a TimeSubscription (which takes a `TimeSpan` as the interval). Yet, you set the internal to be a number in your `Model`. If your code returns the same key, but different instances of the subscription each time, ElmSharp won't notice this, and it will keep the first subscription active and not the latest. 
If you wished to have such a kind of subscription, make sure the key is constructed according to the parameters that make the subscription unique. Almost like the `Vary` parameter on a cache mechanism.

## 🔧 Adjusting the `Update` function

We now have a `Message` to pattern match on, so let's add the code. We'll make it nice by leveraging an extension method on `Model` so that we don't clutter the `Update` function too much.

```csharp
// 📃 Update.cs
using static GuessingGame.Message;

namespace GuessingGame;

public static partial class ElmFuncs
{
    public static (Model, Cmd) Update(Message message, Model model) => message switch
    {
        PlayerPressedKey msg => 
            model.OnPlayerPressedKey(pressedKey: msg.KeyInfo.Key),
    };

    internal static (Model, Cmd) OnPlayerPressedKey(
        this Model model,
        ConsoleKey pressedKey) => pressedKey switch
        {
            ConsoleKey.Q => 
                (model, Cmd.StopAppWithCode(exitCode: 0)),

            // No-op for other key-presses
            _ => 
                (model, Cmd.None)
        };
}
```

## 🚀 Taking the app for a spin

Okay, so to recap, we have setup a `Subscription` to key presses, we have created a `Message` to notify us about this key press, and we have adjusted our `Update` function to return a `StopAppCommand` when we see this `Q` key being pressed. If all goes well, we should be able to start our application and press keys and "nothing should happen" (you might see the console flicker a little depending on your terminal), but once we press `Q` on our keyboard, the app should exit. Like before, you can use `dotnet run` to try out the application.

At least that's how it *works on my machine* 😜

## 🚶‍♂️ On to the next feature: guess a number

As always, before we implement a new feature we should think a little about how we are going to break it down and approach it. We already have the `PlayerPressedKey` message, so that seems like a very good place to adjust our `Model` with the player's guess. Also, we should adjust the `View` function so that it shows to the player whether her guess is too high, too low or just right. For now we'll have unlimited guesses, and once the player guesses the number we should congratulate them and exit the application. Easy peasy, right?

## 🔧 Adjusting the `Update` function

We already have a match on `Q`, let's add the matches on the numbers 0-9. C# has this nice syntax where we can pattern match on range, so we add `>= ConsoleKey.D0 and <= ConsoleKey.D9` to our `Update`/`OnPlayerPressedKey` function. This is also the first time we are changing the model, so we get to leverage the `with {}` syntax from records. Here is how `Update.cs` looks:

```csharp
// 📃 Update.cs
using static GuessingGame.Message;

namespace GuessingGame;

public static partial class ElmFuncs
{
    public static (Model, Cmd) Update(Message message, Model model) => message switch
    {
        PlayerPressedKey msg => 
            model.OnPlayerPressedKey(pressedKey: msg.KeyInfo.Key),
    };

    internal static (Model, Cmd) OnPlayerPressedKey(
        this Model model,
        ConsoleKey pressedKey) => pressedKey switch
        {
            // Q is the key for quitting the app
            ConsoleKey.Q => 
                (model, Cmd.StopAppWithCode(exitCode: 0)),

            // Numbers between 0 and 9
            >= ConsoleKey.D0 and <= ConsoleKey.D9 =>
                (model with { CurrentPlayerGuess = pressedKey - ConsoleKey.D0 }, Cmd.None),

            // No-op for other key-presses
            _ => 
                (model, Cmd.None)
        };
}
```

We should now modify our `View` function, to show to the player what number she has guessed and if her guess is too high or too low.

## 🔧 Adjusting the `View` function

In our `Model` we are currently using `int? CurrentPlayerGuess` to hold either a `null` if the player hasn't guessed a number yet, or an `int` with the player's guess. Therefore we can have some branching logic on the `View` to display this accordingly. Like before, we can leverage extension methods on `Model` to make the code a bit cleaner.

```csharp
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
```

## 🚀 Taking the app for another spin

Okay, it is time to take this phenomenal game for a spin once again and see how it works. `dotnet run` is our friend. We can see that any input other than `0-9` or `Q` has no effect, and if we press the numbers on our keyboard (bug spoiler alert, the ones above the major key section, not the ones on the numpad to the right) we can see the game telling us "too high", "too low" or congratulating us for finding the ""secret"" number (wink wink 😜). We can take note of two improvements that we must make to our app:

* The numbers on the numpad should also work as well

* The game should exit once the player has found the correct number

## 🔧 Allowing the numpad numbers to be used

This one is quite easy, we just need to do a slight adjustment on our `Update` function. It is a good opportunity to refactor some of the code, since we now have two vectors for guessing a number. After a slight refactor, this is how our `Update.cs` looks like now:

```csharp
// 📃 Update.cs
using static GuessingGame.Message;

namespace GuessingGame;

public static partial class ElmFuncs
{
    public static (Model, Cmd) Update(Message message, Model model) => message switch
    {
        PlayerPressedKey msg => 
            model.OnPlayerPressedKey(pressedKey: msg.KeyInfo.Key),
    };

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

    internal static (Model, Cmd) WithPlayerGuess(this Model model, int playerGuess) =>
        (model with { CurrentPlayerGuess = playerGuess }, Cmd.None);
}
```

This refactor created the new `WithPlayerGuess` function, which is invoked from two different places on `OnPlayerPressedKey`.

## 🤔 Exiting the game once the player found the correct number

This is an interesting addition for two reasons:

* If we would just return the `Cmd.StopAppWithCode(exitCode: 0)` from the `Update` function once the player guesses the correct number, then ElmSharp wouldn't render the `View` congratulating the player. Therefore we might want to have a 1 second timeout between the player guessing the number and the application exiting. This provides a nicer experience: a normal game would have some sound or animation to celebrate victory

* If we add a timeout between the correct guessing and the game exiting, there is a chance the player presses another guess. That would be awkward, so we should adjust our `Update` function to no longer take any guesses after we have the correct one (the reason we don't just unsubscribe from keypresses altogether is to allow the player to still use `Q` to quit the game)

Therefore, here we go again. 🔄

## 🚫 No longer taking guesses after the correct guess

For this one we simply need to adjust our `Update` function, specifically the `WithPlayerGuess` function, where we short-circuit early if the guess already matched before:

```csharp
// 📃 Update.cs
// ...
    internal static (Model, Cmd) WithPlayerGuess(this Model model, int playerGuess) =>
        model.NumberToBeGuessed == model.CurrentPlayerGuess 
            // The player had already guessed the number, we don't change the model
            ? (model, Cmd.None) 
            // Otherwise, adjust the model with the player's guess
            : (model with { CurrentPlayerGuess = playerGuess }, Cmd.None);
// ...
```

In this change I am using the [ternary conditional operator](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/conditional-operator) but this is a syntax preference of mine. You could use the more "normal" `if ... return` syntax if you prefer 🙂.

## ⏳ Exiting the game a few seconds after the correct guess

For exiting the game a few seconds after the correct guess, these are the high-level steps that we need to take:

* Create a new `Message` (example: `TimeoutElapsed`) to be triggered after a certain timeout (remember: exiting the application is a `Command` and we can only issue commands as a return of `Init` or `Update`. We need a `Message` to trigger the `Update` hence we need to create a new one

* We need a new handler on the `Update` function to handle this new `TimeoutElapsed` message. This is the handler that will return the stop application command

* Finally, on the `Update` function, when we see the user has correctly guessed the number, we return a `Command` that asks ElmSharp "Please send me the message `TimeoutElapsed` after *n* seconds have elapsed"

Let's do this 💪

## ➕ Creating a new `TimeoutElapsed` message

On our `Message.cs` we add this new `TimeoutElapsed` record:

```csharp
// 📃 Message.cs
namespace GuessingGame;

public abstract record Message
{
    public sealed record PlayerPressedKey(ConsoleKeyInfo KeyInfo) : Message { }

    public sealed record TimeoutElapsed : Message { }
}
```

## 🔧 Adjusting the `Update` function to handle `TimeoutElapsed`

This should be getting a bit easier and more mechanical by now. Here are the changes we need to perform on our `Update.cs` file, to handle the `TimeoutElapsed` message:

```csharp
// 📃 Update.cs
// ...
    public static (Model, Cmd) Update(Message message, Model model) => message switch
    {
        //...
        TimeoutElapsed =>
            model.OnTimeoutElapsed(),
        //...
    };

    //...

    internal static (Model, Cmd) OnTimeoutElapsed(
        this Model model) =>
        (model, Cmd.StopAppWithCode(exitCode: 0));
    
    // ...
// ...
```

## 🔧 Adjusting the `Update` function to return `SetTimeoutCommand` when the player guesses the number

ElmSharp has a few built-in commands, and we are now interested on the `SetTimeoutCommand`. Let's return this command from our `Update` function when we detect that the player has entered the correct guess. For this, we modify the `WithPlayerGuess` function we had before (getting rid of the ternary conditional operator):

```csharp
// 📃 Update.cs
// ...
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
// ...
```

## 🚀 Taking the app for yet another spin

By now our game has a few features (but it's not complete yet!). The player can exit the game. The player can guess a number and the game will let the player know if their guess is too high or too low. And finally, the game congratulates the player for finding the correct number and gracefully exits after a period of time. Let's use `dotnet run` once more.

At least that's how it *works on my machine* 😁

There is only one final thing to do for now: we need a way for the secret number to not always be `3`. Where could we implement such a feature?

Yes, in the `Init` function. But remember, the `Init` function, just like all the others, needs to be **pure**. 

> ℹ Did you notice that all the functions we wrote so far are **pure**? A pure function is a function that given the same inputs will **always** return the same output. Which is a fancy way of saying it has no "tentacles" or dependencies to the external world/state. We accomplish this by not using impure methods, such as `DateTime.Now`, `Random`, `Guid.NewGuid()` etc. 
Every time we need to do such impure business we use a `Command` to do it, and the command generates a pure `Message` so that we can get back to a pure implementation of `Update`. This applies to everything: HTTP requests, randomness, datetime, etc. 
If you think about it, and you have some TDD experience, you will notice that TDD compels you to remove all impurity from your methods, so they can be instrumented and tested. What we have accomplished with ElmSharp is an architecture (the elm architecture) that forces us to stay pure. I guess you can see how testing these pure functions then becomes a trivial matter: no dependency injection, no fancy business: you construct a `Model`, you construct a `Message`, invoke the `Update` function and assert against the output. Same thing goes for the `View` function. You can find the testing examples for the GuessingGame [here](../GuessingGame-Tests/).
Notice how `Update` isn't even `async/await` because it really doesn't have the capability of going out into the world and do ..who knows what... Fun stuff, no? 🙂

So, back to the problem at hand: generating a random number between 0 and 9. Let's use another built-in command: `Cmd.GetRandomNumberCommand` which leverages the `System.Security.Cryptography.RandomNumberGenerator` to do its thing.

Of course this also means we need a new `Message` to get the new secret number into our `Model` (via the `Update` function).

## ➕ Adding a `SecretNumberPicked` message

You know the drill by now 🙂 We add the new message to the `Message.cs` file:

```csharp
// 📃 Message.cs
namespace GuessingGame;

public abstract record Message
{
    public sealed record SecretNumberPicked(int SecretNumber) : Message { }
    // ...
}
```

## 🔧 Adjust the `Init` function to request a random number

We apply the necessary changes on our `Init.cs` file:

```csharp
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
```

## 🔧 Handling the `SecretNumberPicked` in the `Update` function

Hopefully this is getting easier by the minute. Just a few modifications on our `Update.cs` file:

```csharp
// 📃 Update.cs
// ...
    public static (Model, Cmd) Update(Message message, Model model) => message switch
    {
        SecretNumberPicked msg =>
            model.OnSecretNumberPicked(msg.SecretNumber),
        // ...
    };

    internal static (Model, Cmd) OnSecretNumberPicked(this Model model, int secretNumber) =>
        (model with { NumberToBeGuessed = secretNumber }, Cmd.None);
// ...
```

## 🚀 Taking the app for the final spin

For one final time, `dotnet run` allows us to take this app to the races.

Hopefully (unless you are very unlucky) `3` is no longer the secret number. You can now play the game as expected and see how long it takes you to find a random number between 0 and 9.

In the end, the game wasn't the goal: the goal was to get a clearer understanding of the elm architecture and how you can leverage it for the challenges that lie ahead. As said many times throughout the industry, there are no silver bullets. This isn't an answer to all problems, but I do find the constraints quite liberating. I can understand if you have feelings of "boilerplate overkill" due to having to create a `Message` and then modify the `Update` and then the `View` etc, but if you have a honest look, this is also what must be done in any decent size software project.
