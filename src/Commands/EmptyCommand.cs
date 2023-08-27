namespace ElmSharp;

public static partial class ElmSharp<TModel, TMessage>
{
    public abstract partial class Command 
    {
        /// <summary>
        /// A command to be returned from either Init or Update when there is nothing to be done.
        /// Since there is no data on this command, there is a Singleton instance available in
        /// <see cref="Command.None"/>.
        /// </summary>
        sealed class EmptyCommand : Command { }
    }
}
