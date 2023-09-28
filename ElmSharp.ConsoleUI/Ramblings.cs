using System.Collections.Immutable;

using static ElmSharp.ConsoleUI.UIElement.Paragraph;
using static ElmSharp.ConsoleUI.UIElement.Row;

namespace ElmSharp.ConsoleUI
{
    public abstract record UIElement()
    {
        public sealed record Row(Row.RowAttributes Attributes, ImmutableList<RowElement> Elements) : UIElement()
        {
            public Row(RowAttributes Attributes, params RowElement[] Elements) : this(Attributes, Elements.ToImmutableList()) { }

            public sealed record RowAttributes(Border Border)
            {
                public static readonly RowAttributes RowDefaults =
                    new(Border: Border.None);
            }

            public sealed record RowElement(HorizontalAlignment HorizontalAlignment, UIElement Element) { }
        }

        public sealed record Paragraph(ParagraphAttributes Attributes, ImmutableList<ColoredText> Elements) : UIElement()
        {
            public Paragraph(ParagraphAttributes Attributes, params ColoredText[] Elements) : this(Attributes, Elements.ToImmutableList()) { }

            public sealed record ParagraphAttributes(
                ConsoleColor? BackgroundColor,
                Border Border,
                TextAlignment TextAlign)
            {
                public static readonly ParagraphAttributes ParagraphDefaults =
                    new(BackgroundColor: default,
                        Border: Border.None,
                        TextAlign: TextAlignment.Left);
            }
        }

        public sealed record ColoredText(string Text, ConsoleColor? Color = default)
        {
            public static implicit operator ColoredText(string text) => new(text);

            public static implicit operator ColoredText(char text) => new(text.ToString());
        }

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        internal static T Map<T>(
            UIElement element,
            Func<Row, T> whenRow,
            Func<Paragraph, T> whenParagraph) => element switch
            {
                Row row => whenRow(row),
                Paragraph paragraph => whenParagraph(paragraph)
            };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
    }

    public abstract record Border
    {
        public static readonly Border None = new NoBorder();

        public static Border Thin(ConsoleColor? color = default) => new ThinBorder(Color: color);

        public static Border Double(ConsoleColor? color = default) => new DoubleBorder(Color: color);

        internal sealed record NoBorder : Border { }

        internal sealed record ThinBorder(ConsoleColor? Color) : Border { }

        internal sealed record DoubleBorder(ConsoleColor? Color) : Border { }

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
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
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
    }

    public abstract record TextAlignment
    {
        public static readonly TextAlignment Left = new LeftTextAlign();
        public static readonly TextAlignment Center = new CenterTextAlign();
        public static readonly TextAlignment Right = new RightTextAlign();

        internal sealed record LeftTextAlign : TextAlignment { }

        internal sealed record CenterTextAlign : TextAlignment { }

        internal sealed record RightTextAlign : TextAlignment { }
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        internal static T Map<T>(TextAlignment textAlign,
            Func<T> whenLeft,
            Func<T> whenCenter,
            Func<T> whenRight) => textAlign switch
            {
                LeftTextAlign => whenLeft(),
                CenterTextAlign => whenCenter(),
                RightTextAlign => whenRight(),
            };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
    }

    public abstract record HorizontalAlignment
    {
        public static readonly HorizontalAlignment Left = new HorizontalLeftAlign();
        public static readonly HorizontalAlignment Center = new HorizontalCenterAlign();
        public static readonly HorizontalAlignment Right = new HorizontalRightAlign();

        internal sealed record HorizontalLeftAlign : HorizontalAlignment { }

        internal sealed record HorizontalCenterAlign : HorizontalAlignment { }

        internal sealed record HorizontalRightAlign : HorizontalAlignment { }

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        internal static T Map<T>(HorizontalAlignment horizontalAlignment,
            Func<T> whenLeft,
            Func<T> whenCenter,
            Func<T> whenRight) => horizontalAlignment switch
            {
                HorizontalLeftAlign => whenLeft(),
                HorizontalCenterAlign => whenCenter(),
                HorizontalRightAlign => whenRight(),
            };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
    }
}