using System.Collections.Immutable;
using System.Text;

using static ElmSharp.ConsoleUI.UIElement;

namespace ElmSharp.ConsoleUI;

internal sealed record Context(
    uint AvailableWidth = 640);

internal static class Utils
{
    internal static ImmutableList<ImmutableList<ColoredText>> Render(UIElement element, Context context) => element switch
    {
        Paragraph p => RenderParagraph(p, context),
        //Label el => RenderLabel(el, context),
        //Row row => RenderRow(row, context),
    };

    //internal static ImmutableList<string> StringReflow(string input, Context context)
    //{
    //    if (string.IsNullOrEmpty(input))
    //        return ImmutableList<string>.Empty.Add(input);

    //    var (Output, CurrentLine, CurrentWord) =
    //        $"{input} "
    //        .Aggregate(
    //            (Output: ImmutableList<string>.Empty, CurrentLine: new StringBuilder(), CurrentWord: new StringBuilder()),
    //            (acc, chr) =>
    //            {
    //                if (chr != '\n')
    //                    acc.CurrentWord.Append(chr == '\t' ? ' ' : chr);

    //                if (chr == ' ')
    //                {
    //                    // We have reached the end of the CurrentWord
    //                    // We must now determine if it will fit in the current line, or needs to be placed in the next line

    //                    // The current line would exceed the maximum length with this word
    //                    if (acc.CurrentLine.Length + acc.CurrentWord.Length > context.AvailableWidth)
    //                        return (acc.Output.Add(acc.CurrentLine.ToString()),
    //                                acc.CurrentLine.Clear().Append(acc.CurrentWord.Length == 1 ? string.Empty : acc.CurrentWord),
    //                                acc.CurrentWord.Clear());

    //                    return (acc.Output,
    //                            acc.CurrentLine.Append(acc.CurrentWord.Length == 1 ? string.Empty : acc.CurrentWord),
    //                            acc.CurrentWord.Clear());
    //                }

    //                // We are dealing with a word that is longer than the available width
    //                if (chr == '\n' || acc.CurrentWord.Length >= context.AvailableWidth)
    //                    return (acc.Output.Add(acc.CurrentLine.Append(acc.CurrentWord.Length == 1 ? string.Empty : acc.CurrentWord).ToString()),
    //                            acc.CurrentLine.Clear(),
    //                            acc.CurrentWord.Clear());

    //                return (acc.Output, acc.CurrentLine, acc.CurrentWord);
    //            });

    //    var toReturn = (CurrentLine.Length > 0 || CurrentWord.Length > 1)
    //        ? Output.Add(CurrentLine.Append(CurrentWord).ToString())
    //        : Output;

    //    var (rowsToReturn, lastRow) = (toReturn.Count, toReturn.Last());

    //    // Clean up the added whitespace
    //    if (lastRow == " ")
    //        return toReturn.GetRange(0, rowsToReturn - 1);

    //    if (lastRow.EndsWith(" "))
    //        return toReturn.SetItem(rowsToReturn - 1, lastRow[..^1]);

    //    return toReturn;
    //}

    internal static ImmutableList<ImmutableList<ColoredText>> ParagraphReflow(ImmutableList<ColoredText> input, Context context)
    {
        if (input.IsEmpty || input.All(str => string.IsNullOrEmpty(str.Text)))
            return ImmutableList<ImmutableList<ColoredText>>.Empty.Add(input);

        var (Output, CurrentLine, CurrentWord) = 
            input
            .SelectMany(token => token.Text.Select(chr => (chr, token.Color)))
            .Append((' ', null))
            .Aggregate(
                seed: (Output: ImmutableList<ImmutableList<(char, ConsoleColor?)>>.Empty,
                       CurrentLine: ImmutableList<(char, ConsoleColor?)>.Empty,
                       CurrentWord: ImmutableList<(char, ConsoleColor?)>.Empty),
                func: (acc, token) =>
                {
                    var (chr, color) = token;

                    if (chr != '\n')
                        acc.CurrentWord = acc.CurrentWord.Add((chr == '\t' ? ' ' : chr, color));

                    if (chr == ' ')
                    {
                        if (acc.CurrentLine.Count + acc.CurrentWord.Count > context.AvailableWidth)
                            return (acc.Output.Add(acc.CurrentLine),
                                    acc.CurrentWord.Count == 1 ? acc.CurrentLine.Clear() : acc.CurrentLine.Clear().AddRange(acc.CurrentWord),
                                    acc.CurrentWord.Clear());

                        return (acc.Output,
                                acc.CurrentWord.Count == 1 ? acc.CurrentLine : acc.CurrentLine.AddRange(acc.CurrentWord),
                                acc.CurrentWord.Clear());
                    }

                    if (chr == '\n' || acc.CurrentWord.Count >= context.AvailableWidth)
                        return (acc.Output.Add(acc.CurrentWord.Count == 1 ? acc.CurrentLine : acc.CurrentLine.AddRange(acc.CurrentWord)),
                                acc.CurrentLine.Clear(),
                                acc.CurrentWord.Clear());

                    return acc;
                }
            );

        var toReturn = 
            ((CurrentLine.Count > 0 || CurrentWord.Count > 1)
            ? Output.Add(CurrentLine.AddRange(CurrentWord))
            : Output)
            .Select(MergeContiguousColors)
            .ToImmutableList();

        // Clean up the added whitespace
        var (rowsToReturn, lastRow, lastWord) = (toReturn.Count, toReturn.Last(), toReturn.Last().Last());


        if (lastWord.Text == " ")
            return lastRow.Count == 1
                ? toReturn.GetRange(0, rowsToReturn - 1)
                : toReturn.SetItem(rowsToReturn - 1, lastRow.RemoveAt(lastRow.Count - 1));

        if (lastWord.Text.EndsWith(" "))
            return toReturn.SetItem(rowsToReturn - 1, lastRow.SetItem(lastRow.Count - 1, lastWord with { Text = lastWord.Text[..^1] }));

        return toReturn;

        static ImmutableList<ColoredText> MergeContiguousColors(ImmutableList<(char Chr, ConsoleColor? Color)> line)
        {
            var (Output, CurrentWord, LastColor) =
                line.Aggregate(
                    seed: (Output: ImmutableList<ColoredText>.Empty, CurrentWord: new StringBuilder(), CurrentColor: line.FirstOrDefault().Color),
                    func: (acc, token) =>
                    {
                        if (token.Color == acc.CurrentColor)
                            return (acc.Output, acc.CurrentWord.Append(token.Chr), token.Color);

                        if (acc.CurrentWord.Length > 1)
                            return (acc.Output.Add(new(acc.CurrentWord.ToString(), acc.CurrentColor)), acc.CurrentWord.Clear().Append(token.Chr), token.Color);

                        return (acc.Output, acc.CurrentWord.Append(token.Chr), token.Color);
                    });

            if (CurrentWord.Length > 0)
                return Output.Add(new(CurrentWord.ToString(), LastColor));

            return Output;
        }
    }

    
    internal static ImmutableList<ImmutableList<ColoredText>> RenderParagraph(Paragraph paragraph, Context context) {
        return ParagraphReflow(paragraph.Elements, context);
    }

    //internal static ImmutableList<string> RenderLabel(Label label, Context context)
    //{
    //    if (label.Attributes.Border is not Border.NoBorder border)
    //        // Reserve space for the border |_TEXT_|
    //        context = context with { AvailableWidth = context.AvailableWidth - 4 };

    //    var reflownText = R   eflow(label.Text, context);
    //    var maxWidth = reflownText.Max(line => line.Length);
    //    System.Diagnostics.Debug.Assert(maxWidth <= context.AvailableWidth);

    //    Func<string, string> alignLine = (str) => label.Attributes.TextAlign switch
    //    {
    //        TextAlign.LeftTextAlign => str.PadRight(maxWidth),
    //        TextAlign.CenterTextAlign => $"{new(' ', ((maxWidth - str.Length) / 2) + (maxWidth - str.Length) % 2)}{str}{new(' ', (maxWidth - str.Length) / 2)}",
    //        TextAlign.RightTextAlign => str.PadLeft(maxWidth),
    //    };

    //    return label.Attributes.Border switch
    //    {
    //        Border.ThinBorder =>
    //            ImmutableList<string>
    //                .Empty
    //                .Add($"┌─{new('─', maxWidth)}─┐")
    //                .AddRange(reflownText.Select(line => $"│ {alignLine(line)} │"))
    //                .Add($"└─{new('─', maxWidth)}─┘"),
    //        Border.DoubleBorder =>
    //            ImmutableList<string>
    //                .Empty
    //                .Add($"╔═{new('═', maxWidth)}═╗")
    //                .AddRange(reflownText.Select(line => $"║ {alignLine(line)} ║"))
    //                .Add($"╚═{new('═', maxWidth)}═╝"),
    //        _ => reflownText
    //                .Select(alignLine)
    //                .ToImmutableList(),
    //    };
    //}

    //internal static ImmutableList<string> RenderRow(Row row, Context context) 
    //{
    //    if (row.Elements.Count == 0)
    //        return ImmutableList<string>.Empty;

    //    var availableWidthAfterBorder = context.AvailableWidth - (row.Attributes.Border is Border.NoBorder ? 0 : row.Elements.Count + 1);

    //    // First let's determine how wide each element needs to be (for now, fair sizing)
    //    var elementWidth = (int)Math.Floor(decimal.Divide(availableWidthAfterBorder, row.Elements.Count));

    //    var elementRows = row.Elements.Select(el => Render(el, context with { AvailableWidth = (uint)elementWidth })).ToImmutableList();

    //    var columnSizes = elementRows.Select(rows => rows.Max(row => row.Length)).ToImmutableList();

    //    Func<char, char, char, char, string> makeLine = (left, line, col, right) =>
    //        columnSizes
    //        .Aggregate(new StringBuilder().Append(left), (acc, colSize) => acc.Append(new string(line, colSize)).Append(col))
    //        .Do(strBuilder => strBuilder.Remove(strBuilder.Length - 1, 1))
    //        .Append(right)
    //        .ToString();

    //    var (headerLine, columnLine, bottomLine) = row.Attributes.Border switch
    //    {
    //        Border.ThinBorder   => (makeLine('┌', '─', '┬', '┐'), "│", makeLine('└', '─', '┴', '┘')),
    //        Border.DoubleBorder => (makeLine('╔', '═', '╦', '╗'), "║", makeLine('╚', '═', '╩', '╝')),
    //        _ => (string.Empty, string.Empty, string.Empty),
    //    };

    //    var toReturn = 
    //        Enumerable
    //        .Range(0, elementRows.Max(r => r.Count))
    //        .Aggregate(
    //            ImmutableList<string>.Empty, 
    //            (acc, rowIndex) => { 
    //                var lineBuilder = new StringBuilder();

    //                for (var colIndex = 0; colIndex < row.Elements.Count; colIndex++) 
    //                    lineBuilder
    //                        .Append(columnLine)
    //                        .Append(
    //                            elementRows[colIndex].Count <= rowIndex
    //                            ? new string(' ', columnSizes[colIndex])
    //                            : elementRows[colIndex][rowIndex]);

    //                return acc.Add(lineBuilder.Append(columnLine).ToString());
    //            });

    //    if (!string.IsNullOrWhiteSpace(headerLine))
    //        return toReturn.Insert(0, headerLine).Add(bottomLine);

    //    return toReturn;
    //}

    internal static T Do<T>(this T subject, Action<T> action)
    {
        if (subject != null && action != null)
            action(subject);

        return subject;
    }

    public static ColoredText WithColor(this string text, ConsoleColor color) => 
        new(Text: text, Color: color);
}
