namespace ElmSharp;

public static partial class ElmSharp<TModel, TMessage>
{
    public abstract partial class Command 
    {
        public sealed class GetNewGuidCommand : RunnableCommand 
        { 
            public Func<Guid, TMessage> OnNewGuidGenerated { get; init; }

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
