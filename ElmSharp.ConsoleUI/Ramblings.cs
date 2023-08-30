using System.Collections.Immutable;

namespace ElmSharp.ConsoleUI
{
    public abstract record UIElement()
    {
        //public sealed record Label(Label.LabelAttributes Attributes, string Text) : UIElement()
        //{
        //    public sealed record LabelAttributes(
        //        ConsoleColor? ForegroundColor,
        //        ConsoleColor? BackgroundColor,
        //        Border Border,
        //        TextAlign TextAlign)
        //    {
        //        public static readonly LabelAttributes Default =
        //            new(ForegroundColor: default,
        //                BackgroundColor: default,
        //                Border: Border.None,
        //                TextAlign: TextAlign.Left);
        //    }
        //}

        //public sealed record Row(Row.RowAttributes Attributes, ImmutableList<UIElement> Elements) : UIElement() 
        //{
        //    public Row(RowAttributes Attributes, params UIElement[] Elements) : this(Attributes, Elements.ToImmutableList()) { }

        //    public sealed record RowAttributes(Border Border) 
        //    { 
        //        public static readonly RowAttributes Default =
        //            new(Border: Border.None);
        //    }
        //}

        public sealed record Paragraph(ImmutableList<ColoredText> Elements) : UIElement() 
        {
            public Paragraph(params ColoredText[] Elements) : this(Elements.ToImmutableList()) { }
        }

        public sealed record ColoredText(string Text, ConsoleColor? Color = default) 
        {
            public static implicit operator ColoredText(string text) => new(text);
        }
    }

    //public abstract record Border
    //{
    //    public static readonly Border None = new NoBorder();

    //    public static readonly Border Thin = new ThinBorder();

    //    public static readonly Border Double = new DoubleBorder();

    //    internal sealed record NoBorder : Border { }

    //    internal sealed record ThinBorder : Border { }

    //    internal sealed record DoubleBorder : Border { }
    //}

    //public abstract record TextAlign
    //{
    //    public static readonly TextAlign Left = new LeftTextAlign();
    //    public static readonly TextAlign Center = new CenterTextAlign();
    //    public static readonly TextAlign Right = new RightTextAlign();

    //    internal sealed record LeftTextAlign : TextAlign { }

    //    internal sealed record CenterTextAlign : TextAlign { }

    //    internal sealed record RightTextAlign : TextAlign { }
    //}
}