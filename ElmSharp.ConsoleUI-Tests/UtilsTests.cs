using ElmSharp.ConsoleUI;

namespace ElmSharp.ConsoleUI_Tests;

public class UtilsTests
{
    [Theory]
    [InlineData("", 100, "")]
    [InlineData("abcdef", 3, "abc\ndef")]
    [InlineData("abcdef", 2, "ab\ncd\nef")]
    [InlineData("ab cd ef", 2, "ab\ncd\nef")]
    [InlineData("ab cd ef", 3, "ab \ncd \nef")]
    [InlineData("ab cd ef", 4, "ab \ncd \nef")]
    [InlineData("This is a normal sentence", 10, "This is a \nnormal \nsentence")]
    public void ReflowTests(string example, uint maxWidth, string expected) =>
        Assert.Equal(expected.Split("\n"), Utils.Reflow(example, context: new(AvailableWidth: maxWidth)));
}