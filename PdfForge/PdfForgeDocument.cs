using System.Runtime.InteropServices;

namespace PdfForge;

public class PdfForgeDocument : IDisposable
{
    private readonly IntPtr _doc;
    private readonly int _totalPages;

    public PdfForgeDocument(string filename)
    {
        if (!File.Exists(filename))
            throw new FileNotFoundException("Could not open document.", filename);
        
        // Преобразуем путь в формат URL
        string fileUrl = ConvertPathToFileUrl(filename);

        _doc = PdfForgeWrapper.document_new(fileUrl);
        if (_doc == IntPtr.Zero)
        {
            throw new InvalidOperationException("Error: Could not open document.");
        }

        _totalPages = PdfForgeWrapper.document_total_pages(_doc);
    }
    
    private static string ConvertPathToFileUrl(string path) => $"file:///{Path.GetFullPath(path).Replace("\\", "/")}";


    public void SavePageToPngFile(int pageNumber, string filename, double scale = 1.0)
    {
        // Получаем первую страницу и сохраняем ее как PNG
        IntPtr pagePtr = PdfForgeWrapper.document_get_page(_doc, pageNumber, scale);

        if (pagePtr == IntPtr.Zero)
        {
            throw new InvalidOperationException("Error: Could not get page.");
        }

        int resultCode = PdfForgeWrapper.page_save_as_png(pagePtr, filename);

        if (resultCode != 0)
        {
            throw new InvalidOperationException("Error: Could not save page.");
        }
    }

    public byte[] GetPageBytes(int pageNumber, double scale = 1.0)
    {
        IntPtr pagePtr = PdfForgeWrapper.document_get_page(_doc, pageNumber, scale);

        if (pagePtr == IntPtr.Zero)
        {
            throw new InvalidOperationException("Error: Could not get page.");
        }

        IntPtr bufferPtr = PdfForgeWrapper.page_to_png_bytes(pagePtr);
        if (bufferPtr == IntPtr.Zero)
        {
            throw new InvalidOperationException("Failed to convert page to PNG.");
        }

        try
        {
            int size = PdfForgeWrapper.get_byte_buffer_size(bufferPtr);
            IntPtr dataPtr = PdfForgeWrapper.get_byte_buffer_data(bufferPtr);

            // Копирование данных в C# массив
            byte[] data = new byte[size];
            Marshal.Copy(dataPtr, data, 0, size);

            return data;
        }
        finally
        {
            if (pagePtr != IntPtr.Zero)
            {
                PdfForgeWrapper.page_free(pagePtr);
            }
            if (bufferPtr != IntPtr.Zero)
            {
                PdfForgeWrapper.free_byte_buffer(bufferPtr);
            }
        }
    }

    public void Dispose()
    {
        try
        {
            PdfForgeWrapper.document_free(_doc);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
        }
    }

    public int TotalPages => _totalPages;
}