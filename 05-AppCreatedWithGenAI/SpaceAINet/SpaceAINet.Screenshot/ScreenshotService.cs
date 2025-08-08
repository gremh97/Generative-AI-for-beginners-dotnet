using System.Text;

namespace SpaceAINet.Screenshot;

public static class ScreenshotService
{
    private static readonly string Folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "screenshots");
    private static int _counter = 0;

    public static void Initialize()
    {
        if (Directory.Exists(Folder))
            Directory.Delete(Folder, true);
        Directory.CreateDirectory(Folder);
        _counter = 0;
    }

    public static void Capture(char[,] chars, ConsoleColor[,] colors)
    {
        int rows = chars.GetLength(0);
        int cols = chars.GetLength(1);
        
        string baseName = $"screenshot_{_counter++:D4}";
        
        // Save the frame as a plain text file
        string textPath = Path.Combine(Folder, baseName + ".txt");
        using (var writer = new StreamWriter(textPath))
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    writer.Write(chars[y, x]);
                }
                writer.WriteLine();
            }
        }

        // Note: Colored version removed to keep it simple
        // Only plain text screenshot is saved: {baseName}.txt
    }

    public static byte[] GetJpegBytes(char[,] chars, ConsoleColor[,] colors)
    {
        // For cross-platform compatibility, return UTF-8 encoded text instead of JPEG
        int rows = chars.GetLength(0);
        int cols = chars.GetLength(1);
        
        var sb = new StringBuilder();
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                sb.Append(chars[y, x]);
            }
            sb.AppendLine();
        }
        
        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
