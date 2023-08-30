using ElmSharp.ConsoleUI;

using System.Collections.Immutable;

using static ElmSharp.ConsoleUI.UIElement.ColoredText;

namespace ElmSharp.ConsoleUI_Tests;

public class UtilsTests
{
    //[Theory]
    //[InlineData("", 100, "")]
    //[InlineData("abcdef", 3, "abc\ndef")]
    //[InlineData("abcdef", 2, "ab\ncd\nef")]
    //[InlineData("ab cd ef", 2, "ab\ncd\nef")]
    //[InlineData("ab cd ef", 3, "ab \ncd \nef")]
    //[InlineData("ab cd ef", 4, "ab \ncd \nef")]
    //[InlineData("This is a normal sentence", 10, "This is a \nnormal \nsentence")]
    //public void ReflowTests(string example, uint maxWidth, string expected) =>
    //    Assert.Equal(expected.Split("\n"), Utils.StringReflow(example, context: new(AvailableWidth: maxWidth)));

    [Theory]
    [MemberData(nameof(ParagraphReflowTests_Data))]
    public void ParagraphReflowTests(UIElement.ColoredText[] input, uint maxWidth, UIElement.ColoredText[][] expected) 
    {
        var result = Utils.ParagraphReflow(
            input: input.ToImmutableList(),
            context: new(AvailableWidth: maxWidth));

        Assert.Equal(expected.Length, result.Count);

        for (var lineIndex = 0; lineIndex < result.Count; lineIndex++)
            Assert.Equal(expected[lineIndex], result[lineIndex]);
    }

    public static IEnumerable<object[]> ParagraphReflowTests_Data =>
        new[] {
            new object[] { new[] { Simple("") }, 100u, new UIElement.ColoredText[][] { new[] { Simple("") } } },

            new object[] { new[] { Simple("abcdef") }, 3u, new UIElement.ColoredText[][] { new[] { Simple("abc") }, new[] { Simple("def") } } },
            new object[] { new[] { Red("abcdef") }, 3u, new UIElement.ColoredText[][] { new[] { Red("abc") }, new[] { Red("def") } } },

            new object[] { new[] { Simple("abcdef") }, 2u, new UIElement.ColoredText[][] { new[] { Simple("ab") }, new[] { Simple("cd") }, new[] { Simple("ef") } } },
            new object[] { new[] { Red("abcdef") }, 2u, new UIElement.ColoredText[][] { new[] { Red("ab") }, new[] { Red("cd") }, new[] { Red("ef") } } },

            new object[] { new[] { Simple("ab cd ef") }, 2u, new UIElement.ColoredText[][] { new[] { Simple("ab") }, new[] { Simple("cd") }, new[] { Simple("ef") } } },
            new object[] { new[] { Red("ab cd ef") }, 2u, new UIElement.ColoredText[][] { new[] { Red("ab") }, new[] { Red("cd") }, new[] { Red("ef") } } },

            new object[] { new[] { Simple("ab cd ef") }, 3u, new UIElement.ColoredText[][] { new[] { Simple("ab ") }, new[] { Simple("cd ") }, new[] { Simple("ef") } } },
            new object[] { new[] { Red("ab cd ef") }, 3u, new UIElement.ColoredText[][] { new[] { Red("ab ") }, new[] { Red("cd ") }, new[] { Red("ef") } } },

            new object[] { new[] { Simple("ab cd ef") }, 4u, new UIElement.ColoredText[][] { new[] { Simple("ab ") }, new[] { Simple("cd ") }, new[] { Simple("ef") } } },
            new object[] { new[] { Red("ab cd ef") }, 4u, new UIElement.ColoredText[][] { new[] { Red("ab ") }, new[] { Red("cd ") }, new[] { Red("ef") } } },

            new object[] { new[] { Simple("This is a normal sentence") }, 10u, new UIElement.ColoredText[][] { new[] { Simple("This is a ") }, new[] { Simple("normal ") }, new[] { Simple("sentence") } } },
            new object[] { new[] { Red("This is a normal sentence") }, 10u, new UIElement.ColoredText[][] { new[] { Red("This is a ") }, new[] { Red("normal ") }, new[] { Red("sentence") } } },

            new object[] { new[] { Simple("Simple "), Red("Red "), Cyan("Cyan"), }, 100u, new UIElement.ColoredText[][] { new[] { Simple("Simple "), Red("Red "), Cyan("Cyan"), } } },
        };
}