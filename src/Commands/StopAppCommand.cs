namespace ElmSharp;

public static partial class ElmSharp<TModel, TMessage>
{
    public abstract partial class Command 
    {
        public sealed class StopAppCommand : Command
        {
            public int ExitCode { get; init; }

            internal StopAppCommand(int exitCode) =>
                ExitCode = exitCode;
        }
    }
}
