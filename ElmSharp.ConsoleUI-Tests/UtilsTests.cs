using ElmSharp.ConsoleUI;
using System.Collections.Immutable;

using static ElmSharp.ConsoleUI.UIElement;
using static System.ConsoleColor;

namespace ElmSharp.ConsoleUI_Tests;

public class UtilsTests
{
    [Theory]
    [MemberData(nameof(ParagraphReflowTests_Data))]
    public void ParagraphReflowTests(ColoredText[] input, uint maxWidth, UIElement.ColoredText[][] expected) 
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
            new object[] { new ColoredText[] { "" }, 100u, new ColoredText[][] { new ColoredText[] { new ("") } } },

            new object[] { new ColoredText[] { "abcdef" }, 3u, new ColoredText[][] { new ColoredText[] { new("abc") }, new ColoredText[] { new("def") } } },
            new object[] { new ColoredText[] { "abcdef".WithColor(Red) }, 3u, new ColoredText[][] { new ColoredText[] { new("abc", Red) }, new ColoredText[] { new("def", Red) } } },

            new object[] { new ColoredText[] { "abcdef" }, 2u, new ColoredText[][] { new ColoredText[] { new("ab") }, new ColoredText[] { new("cd") }, new ColoredText[] { new("ef") } } },
            new object[] { new ColoredText[] { "abcdef".WithColor(Red) }, 2u, new ColoredText[][] { new ColoredText[] { new("ab", Red) }, new ColoredText[] { new("cd", Red) }, new ColoredText[] { new("ef", Red) } } },

            new object[] { new ColoredText[] { ("ab cd ef") }, 2u, new ColoredText[][] { new ColoredText[] { new("ab") }, new ColoredText[] { new("cd") }, new ColoredText[] { new("ef") } } },
            new object[] { new ColoredText[] { "ab cd ef".WithColor(Red) }, 2u, new ColoredText[][] { new ColoredText[] { new("ab", Red) }, new ColoredText[] { new("cd", Red) }, new ColoredText[] { new("ef", Red) } } },

            new object[] { new ColoredText[] { "ab cd ef" }, 3u, new ColoredText[][] { new ColoredText[] { new("ab ") }, new ColoredText[] { new("cd ") }, new ColoredText[] { new("ef") } } },
            new object[] { new ColoredText[] { "ab cd ef".WithColor(Red) }, 3u, new ColoredText[][] { new ColoredText[] { new("ab ", Red) }, new ColoredText[] { new("cd ", Red) }, new ColoredText[] { new("ef", Red) } } },

            new object[] { new ColoredText[] { "ab cd ef" }, 4u, new ColoredText[][] { new ColoredText[] { new("ab ") }, new ColoredText[] { new("cd ") }, new ColoredText[] { new("ef") } } },
            new object[] { new ColoredText[] { "ab cd ef".WithColor(Red) }, 4u, new ColoredText[][] { new ColoredText[] { new("ab ", Red) }, new ColoredText[] { new("cd ", Red) }, new ColoredText[] { new("ef", Red) } } },

            new object[] { new ColoredText[] { "This is a normal sentence" }, 10u, new ColoredText[][] { new ColoredText[] { new("This is a ") }, new ColoredText[] { new("normal ") }, new ColoredText[] { new("sentence") } } },
            new object[] { new ColoredText[] { "This is a normal sentence".WithColor(Red) }, 10u, new ColoredText[][] { new ColoredText[] { new ("This is a ", Red) }, new ColoredText[] { new("normal ", Red) }, new ColoredText[] { new("sentence", Red) } } },

            new object[] { new ColoredText[] { "Simple ", "Red ".WithColor(Red), "Cyan".WithColor(Cyan), }, 100u, new ColoredText[][] { new ColoredText[] { new("Simple "), new("Red ", Red), new("Cyan", Cyan), } } },
        };
}