using System.Collections.Immutable;
using System.Threading.Channels;

namespace ElmSharp;

public static partial class ElmSharp<TModel, TMessage>
{
    public static async Task<int> Run(
        Func<(TModel, Command)> init,
        Func<TMessage, TModel, (TModel, Command)> update,
        Func<TModel, Action<TMessage>, object> view,
        Func<TModel, ImmutableDictionary<string, Subscription>> subscriptions,
        CancellationToken cancellationToken = default)
    {
        var currentSubscriptions = ImmutableDictionary<string, CancellationTokenSource>.Empty;

        var mailbox = Channel.CreateUnbounded<TMessage>();
        void dispatcher(TMessage msg) => mailbox.Writer.TryWrite(msg);

        var (model, cmd) = init();

        var desiredSubscriptions = subscriptions(model);

        currentSubscriptions = await ApplyDesiredSubscriptions(currentSubscriptions, desiredSubscriptions, dispatcher);

        while (true)
        {
            if (cmd is Command.StopAppCommand stopAppCmd) 
                return stopAppCmd.ExitCode;

            RunCmd(cmd, dispatcher, cancellationToken);

            await Render(view(model, dispatcher));

            var message = await mailbox.Reader.ReadAsync(cancellationToken);

            (model, cmd) = update(message, model);

            desiredSubscriptions = subscriptions(model);

            currentSubscriptions = await ApplyDesiredSubscriptions(currentSubscriptions, desiredSubscriptions, dispatcher);
        }
    }

    static async Task Render(object viewResult) 
    {
        if (viewResult is string viewResultStr)
        {
            Console.Clear();
            await Console.Out.WriteLineAsync(viewResultStr);
        }
        else 
        {
            await Console.Error.WriteLineAsync($"Unable to render viewResult of type [{viewResult.GetType().Name}]");
        }
    }

    #region Commands

    public abstract partial class Command
    {
        public static Command None { get; } = new EmptyCommand();

        public static StopAppCommand StopAppWithCode(int exitCode) =>
            new(exitCode: exitCode);
    }

    public abstract class RunnableCommand : Command
    {
        public abstract Task<TMessage?> Run();
    }

    static void RunCmd(Command cmd, Action<TMessage> dispatch, CancellationToken cancellationToken)
    {
        if (cmd is RunnableCommand runnableCmd)
            Task.Run(async () =>
            {
                var result = await runnableCmd.Run();
                if (result is not null)
                    dispatch(result);
            }, cancellationToken);
    }

    #endregion

    #region Subscriptions

    public abstract partial class Subscription
    {
        public abstract Task Subscribe(Action<TMessage> dispatcher, CancellationToken cancellationToken);

        public static ImmutableDictionary<string, Subscription> None { get; } = ImmutableDictionary<string, Subscription>.Empty;
    }

    internal static async Task<ImmutableDictionary<string, CancellationTokenSource>> ApplyDesiredSubscriptions
        (ImmutableDictionary<string, CancellationTokenSource> currentSubscriptions,
         ImmutableDictionary<string, Subscription> desiredSubscriptions,
         Action<TMessage> dispatcher)
    {
        var allKeys = currentSubscriptions.Keys.Union(desiredSubscriptions.Keys).ToImmutableHashSet();

        foreach (var subscriptionKey in allKeys)
        {
            var (isCurrent, isDesired) = (currentSubscriptions.ContainsKey(subscriptionKey), desiredSubscriptions.ContainsKey(subscriptionKey));

            // If key is both desired and current, no-op
            if (isCurrent && isDesired)
                continue;

            // If key is desired, this is a new subscription
            if (desiredSubscriptions.TryGetValue(subscriptionKey, out var subscription))
            {
                var cancellationTokenSource = new CancellationTokenSource();
                await subscription.Subscribe(dispatcher, cancellationTokenSource.Token).ConfigureAwait(false);
                currentSubscriptions = currentSubscriptions.Add(subscriptionKey, cancellationTokenSource);
            }

            // If key is not desired, this is a removal of a subscription
            else if (currentSubscriptions.TryGetValue(subscriptionKey, out var subscriptionCancellation)) 
            {
                subscriptionCancellation.Cancel();
                currentSubscriptions = currentSubscriptions.Remove(subscriptionKey);
            }    
        }

        return currentSubscriptions;
    }

    #endregion
}