namespace ElmSharp;

public static partial class ElmSharp<TModel, TMessage>
{
    public abstract partial class Subscription
    {
        public sealed class ConsoleKeyPressSubscription : Subscription
        {
            Func<ConsoleKeyInfo, TMessage> OnKeyPress { get; init; }

            public ConsoleKeyPressSubscription(Func<ConsoleKeyInfo, TMessage> onKeyPress) =>
                OnKeyPress = onKeyPress;

            public override Task Subscribe(Action<TMessage> dispatcher, CancellationToken cancellationToken)
            {
                Task.Run(() =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var readKey = Console.ReadKey(intercept: true);
                        var message = OnKeyPress(readKey);
                        dispatcher(message);
                    }
                }, cancellationToken);

                return Task.CompletedTask;
            }
        }
    }
}
