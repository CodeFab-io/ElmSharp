namespace ElmSharp.ConsoleUI_Playground;

internal abstract record Message
{
    public sealed record ConsoleSizeChanged((int Width, int Height) ConsoleSize) : Message { }
}
