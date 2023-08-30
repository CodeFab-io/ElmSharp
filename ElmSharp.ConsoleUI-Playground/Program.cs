using ElmSharp.ConsoleUI;

using ElmSharp;
using static ElmSharp.ConsoleUI.UIElement.Label;
using static ElmSharp.ConsoleUI.UIElement.Row;
using ElmSharp.ConsoleUI_Playground;

await ElmSharp<Model, Message>.RunWithFlags<Flags>(
    init: (flags) => 
        (new Model(ConsoleWidth: (uint)flags.ConsoleSize.Width), Cmd.None), 
    update: (msg, model) => 
        msg switch { Message.ConsoleSizeChanged consoleSizeChangedMsg => (model with { ConsoleWidth = (uint)consoleSizeChangedMsg.ConsoleSize.Width }, Cmd.None) },
    view: (model, _) => 
        string.Join(Environment.NewLine,
        Utils.Render(
            element: new UIElement.Row(
                Attributes: RowAttributes.Default with { Border = Border.Double }
                , new UIElement.Label(LabelAttributes.Default with { TextAlign = TextAlign.Center, Border = Border.None, }, "Palavras Palavras Palavras Palavras Palavras Palavras Palavras Palavras Palavras ")
                , new UIElement.Label(LabelAttributes.Default with { TextAlign = TextAlign.Left, Border = Border.None, }, "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")
                , new UIElement.Label(LabelAttributes.Default with { TextAlign = TextAlign.Right, Border = Border.None, }, "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")
                ),
            context: new(AvailableWidth: model.ConsoleWidth))),

    subscriptions: (model) => Sub.None.Add(
        nameof(Sub.ConsoleBufferSizeChangedSubscription),
        new Sub.ConsoleBufferSizeChangedSubscription(
            refreshRate: TimeSpan.FromMilliseconds(50),
            onConsoleSizeChanged: newSize => new Message.ConsoleSizeChanged(ConsoleSize: newSize))),
    flags: new((Console.BufferWidth, Console.BufferHeight)));



//Console.WriteLine("Label tests\n");

//Console.WriteLine(
//    string.Join(Environment.NewLine,
//        Utils.Render(
//            element: new UIElement.Label(
//                LabelAttributes.Default with
//                {
//                    Border = Border.Double,
//                    TextAlign = TextAlign.Left,
//                },
//                //"This is a somewhat long text that I wish it had a box around it, for some reason. 2x newline:\n\nAnd this should be in a new line"),
//                "This is the label on the center, and a bit longer text to see what happens"),
//            context: new(AvailableWidth: 20))));

//Console.WriteLine("\nRow tests\n");

//string.Join(Environment.NewLine,
//    Utils.Render(
//        element: new UIElement.Row(
//            Attributes: RowAttributes.Default with { Border = Border.Thin },
//            new UIElement.Label(LabelAttributes.Default with { TextAlign = TextAlign.Left, Border = Border.Thin, }, "This is the label on the left, which has quite some text"),
//            new UIElement.Label(LabelAttributes.Default with { TextAlign = TextAlign.Center, Border = Border.None, }, "This is the label on the center, and a bit longer text to see what happens"),
//            new UIElement.Label(LabelAttributes.Default with { TextAlign = TextAlign.Right, Border = Border.Thin, }, "This is the label on the right")),
//        context: new(AvailableWidth: (uint)Console.WindowWidth)));
