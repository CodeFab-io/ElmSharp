using System.Collections.Immutable;
using System.Text;

using static ElmSharp.ConsoleUI.UIElement;

namespace ElmSharp.ConsoleUI;

internal sealed record Context(
    uint AvailableWidth = 640);

internal static class Utils
{
    internal static ImmutableList<string> Reflow(string input, Context context)
    {
        var acc = input
            .Aggregate(
                (Output: ImmutableList<string>.Empty, CurrentLine: new StringBuilder(), CurrentWord: new StringBuilder()),
                (acc, chr) =>
                {
                    if (chr != '\n')
                        acc.CurrentWord.Append(chr == '\t' ? ' ' : chr);

                    if (chr == ' ')
                    {
                        // We have reached the end of the CurrentWord
                        // We must now determine if it will fit in the current line, or needs to be placed in the next line

                        // The current line would exceed the maximum length with this word
                        if (acc.CurrentLine.Length + acc.CurrentWord.Length > context.AvailableWidth)
                            return (acc.Output.Add(acc.CurrentLine.ToString()),
                                    acc.CurrentLine.Clear().Append(acc.CurrentWord),
                                    acc.CurrentWord.Clear());

                        return (acc.Output,
                                acc.CurrentLine.Append(acc.CurrentWord),
                                acc.CurrentWord.Clear());
                    }

                    // We are dealing with a word that is longer than the available width
                    if (chr == '\n' || acc.CurrentWord.Length > context.AvailableWidth)
                        return (acc.Output.Add(acc.CurrentLine.Append(acc.CurrentWord).ToString()),
                                acc.CurrentLine.Clear(),
                                acc.CurrentWord.Clear());

                    return (acc.Output, acc.CurrentLine, acc.CurrentWord);
                });

        return (acc.CurrentLine.Length > 0 || acc.CurrentWord.Length > 0)
            ? acc.Output.Add(acc.CurrentLine.Append(acc.CurrentWord).ToString())
            : acc.Output;
    }

    internal static ImmutableList<string> Render(UIElement element, Context context) => element switch
    {
        Label el => RenderLabel(el, context),
    };

    internal static ImmutableList<string> RenderLabel(Label label, Context context)
    {
        if (label.Attributes.Border is not Border.NoBorder border)
            // Reserve space for the border |_TEXT_|
            context = context with { AvailableWidth = context.AvailableWidth - 4 };

        var reflownText = Reflow(label.Text, context);
        var maxWidth = reflownText.Max(line => line.Length);

        Func<string, string> alignLine = (str) => label.Attributes.TextAlign switch
        {
            TextAlign.LeftTextAlign => str.PadRight(maxWidth),
            TextAlign.CenterTextAlign => $"{new(' ', ((maxWidth - str.Length) / 2) + (maxWidth - str.Length) % 2)}{str}{new(' ', (maxWidth - str.Length) / 2)}",
            TextAlign.RightTextAlign => str.PadLeft(maxWidth),
        };

        return label.Attributes.Border switch
        {
            Border.ThinBorder =>
                ImmutableList<string>
                    .Empty
                    .Add($"┌─{new('─', maxWidth)}─┐")
                    .AddRange(reflownText.Select(line => $"│ {alignLine(line)} │"))
                    .Add($"└─{new('─', maxWidth)}─┘"),
            Border.DoubleBorder =>
                ImmutableList<string>
                    .Empty
                    .Add($"╔═{new('═', maxWidth)}═╗")
                    .AddRange(reflownText.Select(line => $"║ {alignLine(line)} ║"))
                    .Add($"╚═{new('═', maxWidth)}═╝"),
            _ => reflownText,
        };
    }
}
