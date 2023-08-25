using static GuessingGame_Tests.Helpers;

namespace GuessingGame_Tests;

public sealed class SubscriptionsTests 
{
    [Fact]
    public void Subscriptions_WithAnyModel_ShouldSubscribeToConsoleKeyPress() 
    {
        var (model, _) = Init();
        var subscriptions = Subscriptions(model);

        Assert.True(subscriptions.TryGetValue(nameof(Sub.ConsoleKeyPressSubscription), out var rawSub));
        var subscription = Assert.IsType<Sub.ConsoleKeyPressSubscription>(rawSub);
        
        // Check that the subscription produces the desired message type
        Assert.IsType<PlayerPressedKey>(subscription.OnKeyPress(ConsoleKeyInfo(2)));
    }
}
