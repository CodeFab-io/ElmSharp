namespace ElmSharp.ConsoleUI
{
    public abstract record UIElement()
    {
        public sealed record Label(string Text, Label.LabelAttributes Attributes) : UIElement()
        {
            public sealed record LabelAttributes(
                ConsoleColor? ForegroundColor,
                ConsoleColor? BackgroundColor,
                Border Border,
                TextAlign TextAlign)
            {
                public static readonly LabelAttributes Default =
                    new(ForegroundColor: default,
                        BackgroundColor: default,
                        Border: Border.None,
                        TextAlign: TextAlign.Left);
            }
        }
    }

    public abstract record Border
    {
        public static readonly Border None = new NoBorder();

        public static readonly Border Thin = new ThinBorder();

        public static readonly Border Double = new DoubleBorder();

        internal sealed record NoBorder : Border { }

        internal sealed record ThinBorder : Border { }

        internal sealed record DoubleBorder : Border { }
    }

    public abstract record TextAlign
    {
        public static readonly TextAlign Left = new LeftTextAlign();
        public static readonly TextAlign Center = new CenterTextAlign();
        public static readonly TextAlign Right = new RightTextAlign();

        internal sealed record LeftTextAlign : TextAlign { }

        internal sealed record CenterTextAlign : TextAlign { }

        internal sealed record RightTextAlign : TextAlign { }
    }
}