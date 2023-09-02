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
        //Row row => RenderRow(row, context),
    };

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

    internal static ImmutableList<ImmutableList<ColoredText>> RenderParagraph(Paragraph paragraph, Context originalContext)
    {
        var context = paragraph.Attributes.Border is Border.NoBorder ? originalContext
            : originalContext with { AvailableWidth = originalContext.AvailableWidth - 4 };

        var reflownParagraph = ParagraphReflow(paragraph.Elements, context);

        var widestLine = reflownParagraph.Max(line => line.Sum(word => word.Text.Length));

        return Border.Map(
            paragraph.Attributes.Border,
            whenNoBorder: () => reflownParagraph,
            whenThinBorder: borderInfo => makeBorder(borderInfo.Color, '┌', '─', '┐', '│', '└', '┘'),
            whenDoubleBorder: borderInfo => makeBorder(borderInfo.Color, '╔', '═', '╗', '║', '╚', '╝'));

        ImmutableList<ColoredText> alignLine(ImmutableList<ColoredText> line) =>
            TextAlign.Map(
                paragraph.Attributes.TextAlign,
                whenLeft: () =>
                    line.Add(new(Text: new(' ', widestLine - line.Sum(word => word.Text.Length)))),
                whenCenter: () =>
                    line
                    .Map(line => new { line, lineLength = line.Sum(word => word.Text.Length) })
                    .Map(info => info.line
                        .Insert(0, new(Text: new(' ', (widestLine - info.lineLength) / 2 + (widestLine - info.lineLength) % 2)))
                        .Add(new(Text: new(' ', (widestLine - info.lineLength) / 2)))),
                whenRight: () =>
                    line
                    .Insert(0, new(Text: new(' ', widestLine - line.Sum(word => word.Text.Length)))));

        ImmutableList<ImmutableList<ColoredText>> makeBorder(ConsoleColor? color, char lt, char h, char rt, char v, char lb, char rb) =>
            ImmutableList<ImmutableList<ColoredText>>
                .Empty
                .Add(ImmutableList<ColoredText>.Empty.Add(new($"{lt}{h}{new(h, widestLine)}{h}{rt}", color)))
                .AddRange(reflownParagraph.Select(line =>
                    ImmutableList<ColoredText>.Empty
                        .Add(new($"{v} ", color))
                        .AddRange(alignLine(line))
                        .Add(new($" {v}", color))))
                .Add(ImmutableList<ColoredText>.Empty.Add(new($"{lb}{h}{new(h, widestLine)}{h}{rb}", color)));
    }

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

    internal static TOut Map<TIn, TOut>(this TIn subject, Func<TIn, TOut> func) =>
        func(subject);

    public static ColoredText WithColor(this string text, ConsoleColor color) =>
        new(Text: text, Color: color);
}
