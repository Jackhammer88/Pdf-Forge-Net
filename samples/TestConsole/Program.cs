using PdfForge;

namespace TestConsole;

public static class Program
{
    public static void Main(string[] args)
    {
        using var document = new PdfForgeDocument("test.pdf");

        for (int j = 0; j < 1; j++)
        {
            for (int i = 0; i < document.TotalPages; i++)
            {
                var bytes = document.GetPageBytes(i);
                _ = bytes.Length;
            }
        }
    }
}