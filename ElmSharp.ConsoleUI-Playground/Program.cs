﻿using ElmSharp.ConsoleUI;

using static ElmSharp.ConsoleUI.UIElement.Label;
using static ElmSharp.ConsoleUI.UIElement.Row;

Console.WriteLine("Label tests\n");

Console.WriteLine(
    string.Join(Environment.NewLine,
        Utils.Render(
            element: new UIElement.Label(
                LabelAttributes.Default with
                {
                    Border = Border.Double,
                    TextAlign = TextAlign.Left,
                },
                //"This is a somewhat long text that I wish it had a box around it, for some reason. 2x newline:\n\nAnd this should be in a new line"),
                "This is the label on the center, and a bit longer text to see what happens"),
            context: new(AvailableWidth: 20))));

Console.WriteLine("\nRow tests\n");

Console.WriteLine(string.Join(Environment.NewLine,
    Utils.Render(
        element: new UIElement.Row(RowAttributes.Default with { Border = Border.Thin }, 
            new UIElement.Label(LabelAttributes.Default with { TextAlign = TextAlign.Left, Border = Border.Thin, }, "This is the label on the left, which has quite some text"),
            new UIElement.Label(LabelAttributes.Default with { TextAlign = TextAlign.Center, Border = Border.None, }, "This is the label on the center, and a bit longer text to see what happens"),
            new UIElement.Label(LabelAttributes.Default with { TextAlign = TextAlign.Right, Border = Border.Thin, }, "This is the label on the right")),
        context: new(AvailableWidth: (uint)Console.WindowWidth)
        )));
