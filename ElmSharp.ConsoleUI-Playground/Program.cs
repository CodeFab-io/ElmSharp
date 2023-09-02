using ElmSharp;
using ElmSharp.ConsoleUI;
using ElmSharp.ConsoleUI_Playground;

using static ElmSharp.ConsoleUI.Border;
using static ElmSharp.ConsoleUI.TextAlign;
using static ElmSharp.ConsoleUI.UIElement;
using static ElmSharp.ConsoleUI.UIElement.Paragraph.ParagraphAttributes;
using static ElmSharp.ConsoleUI.UIElement.Row.RowAttributes;
using static System.ConsoleColor;

await ElmSharp<Model, Message>.RunWithFlags<Flags>(
    init: (flags) =>
        (new Model(ConsoleWidth: (uint)flags.ConsoleSize.Width), Cmd.None),

    update: (msg, model) =>
        msg switch { Message.ConsoleSizeChanged consoleSizeChangedMsg => (model with { ConsoleWidth = (uint)consoleSizeChangedMsg.ConsoleSize.Width }, Cmd.None) },

    view:
        ViewFuncs.View<Model, Message>(View),

    subscriptions: (model) => Sub.None.Add(
        nameof(Sub.ConsoleBufferSizeChangedSubscription),
        new Sub.ConsoleBufferSizeChangedSubscription(
            refreshRate: TimeSpan.FromMilliseconds(50),
            onConsoleSizeChanged: newSize => new Message.ConsoleSizeChanged(ConsoleSize: newSize))),
    flags:
        new((Console.BufferWidth, Console.BufferHeight)));

static UIElement View(Model model) =>
    new Row(RowDefaults with { Border = Thin(Cyan) },
        new Paragraph(
            ParagraphDefaults with { Border = Thin(Yellow), TextAlign = Left, },
            "This text is ", "left".WithColor(Red), " aligned, and coveniently a little bit longer so it can occupy all the space"),
        new Paragraph(
            ParagraphDefaults with { Border = Double(Yellow), TextAlign = Center, },
            "This text has the console default color and then some text in ", "red".WithColor(Red),", ", 
            "cyan".WithColor(Cyan), " and ",  "green".WithColor(Green), "."),
        new Paragraph(
            ParagraphDefaults with { TextAlign = Right },
            "And then some more text, this time ", "right".WithColor(DarkMagenta), " aligned"));
