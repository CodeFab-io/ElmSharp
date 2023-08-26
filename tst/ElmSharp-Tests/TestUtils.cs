namespace ElmSharp_Tests;

public static class TestUtils 
{
    public static async Task Repeat(this Func<Task> test, uint howManyTimes) 
    {
        for (var iterarion = 0; iterarion < howManyTimes; iterarion++)
            await test();
    }
}