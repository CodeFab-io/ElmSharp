namespace ElmSharp;

public static partial class ElmSharp<TModel, TMessage>
{
    public abstract partial class Subscription
    {
        public sealed class ConsoleBufferSizeChangedSubscription : Subscription
        {
            public TimeSpan RefreshRate { get; init; }

            public Func<(int Width, int Height), TMessage> OnConsoleSizeChanged { get; init; }

            public ConsoleBufferSizeChangedSubscription(TimeSpan refreshRate, Func<(int Width, int Height), TMessage> onConsoleSizeChanged)
            {
                RefreshRate = TimeSpan.FromMilliseconds(Math.Max(
                    TimeSpan.FromMilliseconds(50).TotalMilliseconds,
                    refreshRate.TotalMilliseconds));

                OnConsoleSizeChanged = onConsoleSizeChanged;
            }

            public override Task Subscribe(Action<TMessage> dispatcher, CancellationToken cancellationToken)
            {
                Task.Run(async () =>
                {
                    var previousSize = (Width: Console.BufferWidth, Height: Console.BufferHeight);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var currentSize = (Width: Console.BufferWidth, Height: Console.BufferHeight);

                        if (previousSize != currentSize)
                        {
                            previousSize = currentSize;
                            var message = OnConsoleSizeChanged(currentSize);
                            dispatcher(message);
                        }

                        await Task.Delay(RefreshRate, cancellationToken);
                    }
                }, cancellationToken);

                return Task.CompletedTask;
            }
        }
    }
}
