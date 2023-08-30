namespace ElmSharp.ConsoleUI;

public static class ViewFuncs
{
    public static Action<TModel, Action<TMessage>> View<TModel, TMessage>(Func<TModel, UIElement> consoleUiView) => (model, _) =>
    {
        var uiElement = consoleUiView(model);
        var renderResult = Utils.Render(uiElement, context: new(AvailableWidth: (uint)Console.BufferWidth));

        Console.Clear();
        var (initialFG, initialBG) = (Console.ForegroundColor, Console.BackgroundColor);
        var (currentFG, currentBG) = ((ConsoleColor?)null, (ConsoleColor?)null);

        foreach (var line in renderResult) 
        {
            foreach (var word in line) 
            {
                var desiredFG = word.Color is ConsoleColor color ? color : initialFG;
                if (currentFG != desiredFG) 
                    currentFG = Console.ForegroundColor = desiredFG;
                Console.Write(word.Text);
            }
            Console.WriteLine();
        }

        Console.ResetColor();
    };
}
