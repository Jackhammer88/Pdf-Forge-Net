using PdfForge.Interop;

namespace PdfForge;

public class PdfForgeDocument : IDisposable
{
    private readonly object _lock = new();
    private readonly Poppler.PopplerDocumentHandle _doc;

    public PdfForgeDocument(string filename)
    {
        if (!File.Exists(filename))
            throw new FileNotFoundException("Could not open document.", filename);
        
        // Преобразуем путь в формат URL
        string fileUrl = ConvertPathToFileUrl(filename);

        _doc = Poppler.poppler_document_new_from_file(fileUrl, "", out _);
        if (_doc.IsInvalid)
        {
            throw new InvalidOperationException("Error: Could not open document.");
        }

        TotalPages = _doc.TotalPages;
    }
    
    public int TotalPages { get; }
    
    private static string ConvertPathToFileUrl(string path) => $"file:///{Path.GetFullPath(path).Replace("\\", "/")}";

    public void SavePageToPngFile(int pageNumber, string filename, double scale = 1.0)
    {
        lock (_lock)
        {
            using var page = _doc.GetPage(pageNumber);
            if (page.IsInvalid)
            {
                throw new InvalidOperationException("Error: Could not get page.");
            }
        
            page.ToPngFile(filename, scale);
        }
    }

    public Memory<byte> GetPageBytes(int pageNumber, double scale = 1.0)
    {
        lock (_lock)
        {
            using var page = _doc.GetPage(pageNumber);
            if (page.IsInvalid)
            {
                throw new InvalidOperationException("Error: Could not get page.");
            }
        
            return page.GetPngBytes(scale);
        }
    }

    public void Dispose()
    {
        try
        {
            if (!_doc.IsClosed)
            {
                _doc.Dispose();
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
        }
    }
}