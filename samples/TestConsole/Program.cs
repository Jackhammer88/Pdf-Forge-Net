
using PdfForge;

using PdfForgeDocument document = new("test.pdf");

string outputDir = "pages";

if (Directory.Exists(outputDir))
{
    Directory.Delete(outputDir, true);
}

Directory.CreateDirectory(outputDir);

for (int i = 0; i < document.TotalPages; i++)
{
    document.SavePageToPngFile(i, Path.Combine(outputDir, $"page-{i+1}.png"));
}

for (int i = 0; i < document.TotalPages; i++)
{
    string filename = Path.Combine(outputDir, $"page-from-bytes-{i + 1}.png");
    byte[] bytes = document.GetPageBytes(i);

    File.WriteAllBytes(filename, bytes);
}

Console.WriteLine("Done");