using System.Runtime.Versioning;
using PdfForge;

[assembly: SupportedOSPlatform("linux")]

using PdfForgeDocument document = new("test.pdf");

for (int i = 0; i < document.TotalPages; i++)
{
    document.SavePageToPngFile(i, $"test{i + 1}.png");
}