using ElmSharp;
using ElmSharp.ConsoleUI;
using ElmSharp.ConsoleUI_Playground;

using static ElmSharp.ConsoleUI.Border;
using static ElmSharp.ConsoleUI.TextAlign;
using static ElmSharp.ConsoleUI.UIElement.Paragraph.ParagraphAttributes;
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
    new UIElement.Paragraph(
        Default with { Border = Double(Yellow), TextAlign = Center, },
        "This text has the console default color and then some text in ", "red".WithColor(Red),", ", 
        "cyan".WithColor(Cyan), " and ",  "green".WithColor(Green), ".");
