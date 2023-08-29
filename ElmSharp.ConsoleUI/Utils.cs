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
        if (string.IsNullOrEmpty(input))
            return ImmutableList<string>.Empty.Add(input);

        var acc = 
            $"{input} "
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
                                    acc.CurrentLine.Clear().Append(acc.CurrentWord.Length == 1 ? string.Empty : acc.CurrentWord),
                                    acc.CurrentWord.Clear());

                        return (acc.Output,
                                acc.CurrentLine.Append(acc.CurrentWord.Length == 1 ? string.Empty : acc.CurrentWord),
                                acc.CurrentWord.Clear());
                    }

                    // We are dealing with a word that is longer than the available width
                    if (chr == '\n' || acc.CurrentWord.Length >= context.AvailableWidth)
                        return (acc.Output.Add(acc.CurrentLine.Append(acc.CurrentWord.Length == 1 ? string.Empty : acc.CurrentWord).ToString()),
                                acc.CurrentLine.Clear(),
                                acc.CurrentWord.Clear());

                    return (acc.Output, acc.CurrentLine, acc.CurrentWord);
                });

        var toReturn = (acc.CurrentLine.Length > 0 || acc.CurrentWord.Length > 1)
            ? acc.Output.Add(acc.CurrentLine.Append(acc.CurrentWord).ToString())
            : acc.Output;

        var (rowsToReturn, lastRow) = (toReturn.Count, toReturn.Last());

        // Clean up the added whitespace
        if (lastRow == " ")
            return toReturn.GetRange(0, rowsToReturn - 1);

        if (lastRow.EndsWith(" "))
            return toReturn.SetItem(rowsToReturn - 1, lastRow[..^1]);

        return toReturn;
    }

    internal static ImmutableList<string> Render(UIElement element, Context context) => element switch
    {
        Label el => RenderLabel(el, context),
        Row row => RenderRow(row, context),
    };

    internal static ImmutableList<string> RenderLabel(Label label, Context context)
    {
        if (label.Attributes.Border is not Border.NoBorder border)
            // Reserve space for the border |_TEXT_|
            context = context with { AvailableWidth = context.AvailableWidth - 4 };

        var reflownText = Reflow(label.Text, context);
        var maxWidth = reflownText.Max(line => line.Length);
        System.Diagnostics.Debug.Assert(maxWidth <= context.AvailableWidth);

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
            _ => reflownText
                    .Select(alignLine)
                    .ToImmutableList(),
        };
    }

    internal static ImmutableList<string> RenderRow(Row row, Context context) {
        // First let's determine how wide each element needs to be (for now, fair sizing)
        var elementWidth = (int)(context.AvailableWidth / row.Elements.Count);

        var elementRows = row.Elements.Select(el => Render(el, context with { AvailableWidth = (uint)elementWidth })).ToImmutableList();

        var columnSizes = elementRows.Select(rows => rows.Max(row => row.Length)).ToImmutableList();

        return Enumerable
            .Range(0, elementRows.Max(r => r.Count))
            .Aggregate(
                ImmutableList<string>.Empty, 
                (acc, rowIndex) => { 
                    var lineBuilder = new StringBuilder();

                    for (var colIndex = 0; colIndex < row.Elements.Count; colIndex++) {
                        lineBuilder.Append(
                            elementRows[colIndex].Count <= rowIndex
                            ? new string(' ', columnSizes[colIndex])
                            : elementRows[colIndex][rowIndex]);
                    }

                    return acc.Add(lineBuilder.ToString());
                });
    }
}
