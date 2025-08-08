using SpaceAINet.Screenshot;

public static class TestScreenshot
{
    public static void Main()
    {
        Console.WriteLine("Testing screenshot service...");
        
        // Initialize
        ScreenshotService.Initialize();
        Console.WriteLine("Initialize called");
        
        // Create test data
        char[,] testChars = new char[5, 10];
        ConsoleColor[,] testColors = new ConsoleColor[5, 10];
        
        // Fill with test data
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                testChars[y, x] = (char)('A' + (y * 10 + x) % 26);
                testColors[y, x] = ConsoleColor.White;
            }
        }
        
        // Capture
        ScreenshotService.Capture(testChars, testColors);
        Console.WriteLine("Capture completed");
        
        // Check if file exists (screenshot files are saved as screenshot_0000.png)
        string expectedFile = Path.Combine("screenshots", "screenshot_0000.png");
        if (File.Exists(expectedFile))
        {
            Console.WriteLine($"File created successfully! Size: {new FileInfo(expectedFile).Length} bytes");
        }
        else
        {
            Console.WriteLine("File was not created!");
        }
        
        Console.WriteLine("Test completed. Press any key to continue...");
        Console.ReadKey();
    }
}
