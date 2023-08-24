namespace ElmSharp;

public static partial class ElmSharp<TModel, TMessage>
{
    public abstract partial class Subscription
    {
        public sealed class TimeSubscription : Subscription
        {
            TimeSpan Every { get; init; }
            Func<TMessage> OnTick { get; init; }

            public TimeSubscription(TimeSpan every, Func<TMessage> onTick) =>
                (Every, OnTick) =
                (every, onTick);

            public override Task Subscribe(Action<TMessage> dispatcher, CancellationToken cancellationToken)
            {
                Task.Run(async () =>
                {
                    while (true)
                    {
                        await Task.Delay(Every, cancellationToken);
                        dispatcher(OnTick());
                    }
                }, cancellationToken);

                return Task.CompletedTask;
            }
        }
    }
}
