using System.Collections.Immutable;

using static ElmSharp.ConsoleUI.UIElement.Paragraph;

namespace ElmSharp.ConsoleUI
{
    public abstract record UIElement()
    {
        //public sealed record Row(Row.RowAttributes Attributes, ImmutableList<UIElement> Elements) : UIElement() 
        //{
        //    public Row(RowAttributes Attributes, params UIElement[] Elements) : this(Attributes, Elements.ToImmutableList()) { }

        //    public sealed record RowAttributes(Border Border) 
        //    { 
        //        public static readonly RowAttributes Default =
        //            new(Border: Border.None);
        //    }
        //}

        public sealed record Paragraph(ParagraphAttributes Attributes, ImmutableList<ColoredText> Elements) : UIElement() 
        {
            public Paragraph(ParagraphAttributes Attributes, params ColoredText[] Elements) : this(Attributes, Elements.ToImmutableList()) { }

            public sealed record ParagraphAttributes(
                ConsoleColor? BackgroundColor,
                Border Border,
                TextAlign TextAlign)
            {
                public static readonly ParagraphAttributes Default =
                    new(BackgroundColor: default,
                        Border: Border.None,
                        TextAlign: TextAlign.Left);
            }
        }

        public sealed record ColoredText(string Text, ConsoleColor? Color = default) 
        {
            public static implicit operator ColoredText(string text) => new(text);

            public static implicit operator ColoredText(char text) => new(text.ToString());
        }
    }

    public abstract record Border
    {
        public static readonly Border None = new NoBorder();

        public static Border Thin(ConsoleColor? color = default) => new ThinBorder(Color: color);

        public static Border Double(ConsoleColor? color = default) => new DoubleBorder(Color: color);

        internal sealed record NoBorder : Border { }

        internal sealed record ThinBorder(ConsoleColor? Color) : Border { }

        internal sealed record DoubleBorder(ConsoleColor? Color) : Border { }

        internal static T Map<T>(
            Border border,
            Func<T> whenNoBorder,
            Func<ThinBorder, T> whenThinBorder,
            Func<DoubleBorder, T> whenDoubleBorder) => border switch
            {
                NoBorder => whenNoBorder(),
                ThinBorder borderInfo => whenThinBorder(borderInfo),
                DoubleBorder borderInfo => whenDoubleBorder(borderInfo),
            };
    }

    public abstract record TextAlign
    {
        public static readonly TextAlign Left = new LeftTextAlign();
        public static readonly TextAlign Center = new CenterTextAlign();
        public static readonly TextAlign Right = new RightTextAlign();

        internal sealed record LeftTextAlign : TextAlign { }

        internal sealed record CenterTextAlign : TextAlign { }

        internal sealed record RightTextAlign : TextAlign { }

        internal static T Map<T>(TextAlign textAlign,
            Func<T> whenLeft,
            Func<T> whenCenter,
            Func<T> whenRight) => textAlign switch 
            { 
                LeftTextAlign => whenLeft(),
                CenterTextAlign => whenCenter(),
                RightTextAlign => whenRight(),
            };
    }
}