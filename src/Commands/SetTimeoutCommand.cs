namespace ElmSharp;

public static partial class ElmSharp<TModel, TMessage>
{
    public abstract partial class Command 
    {
        public sealed class SetTimeoutCommand : RunnableCommand
        {
            public TimeSpan TimeoutDuration { get; init; }
            public Func<TMessage> OnTimeoutElapsed { get; init; }

            public SetTimeoutCommand(
                TimeSpan timeoutDuration,
                Func<TMessage> onTimeoutElapsed) =>
                    (TimeoutDuration, OnTimeoutElapsed) =
                    (timeoutDuration, onTimeoutElapsed);

            public override async Task<TMessage?> Run()
            {
                await Task.Delay(TimeoutDuration);
                return OnTimeoutElapsed();
            }
        }
    }
}
