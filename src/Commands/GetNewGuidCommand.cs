namespace ElmSharp;

public static partial class ElmSharp<TModel, TMessage>
{
    public abstract partial class Command 
    {
        /// <summary>
        /// The command to be used when you need to obtain a NewGuid. Uses <see cref="Guid.NewGuid"/> internally.
        /// </summary>
        public sealed class GetNewGuidCommand : RunnableCommand 
        { 
            public Func<Guid, TMessage> OnNewGuidGenerated { get; init; }

            /// <param name="onNewGuidGenerated">The message constructor capable of carrying a <see cref="Guid"/></param>
            public GetNewGuidCommand(Func<Guid, TMessage> onNewGuidGenerated) =>
                OnNewGuidGenerated = onNewGuidGenerated;

            public override Task<TMessage?> Run()
            {
                var newGuid = Guid.NewGuid();

                var message = OnNewGuidGenerated(newGuid);

                return Task.FromResult<TMessage?>(message);
            }
        }
    }
}
