using ElmSharp.ConsoleUI;

using static ElmSharp.ConsoleUI.UIElement.Label;

Console.Write(
    string.Join(Environment.NewLine,
        Utils.Render(
            element: new UIElement.Label(
                "This is a somewhat long text that I wish it had a box around it, for some reason. 2x newline:\n\nAnd this should be in a new line",
                LabelAttributes.Default with { 
                    Border = Border.Double, 
                    TextAlign = TextAlign.Left, 
                }),
            context: new(AvailableWidth: 20))));