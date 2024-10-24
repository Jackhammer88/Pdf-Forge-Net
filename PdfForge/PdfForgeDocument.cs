using System.Runtime.InteropServices;
using static PdfForge.PdfForgeWrapper;

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

        _doc = document_new(fileUrl);
        if (_doc == IntPtr.Zero)
        {
            throw new InvalidOperationException("Error: Could not open document.");
        }

        _totalPages = document_total_pages(_doc);
    }
    
    private static string ConvertPathToFileUrl(string path) => $"file:///{Path.GetFullPath(path).Replace("\\", "/")}";


    public void SavePageToPngFile(int pageNumber, string filename, double scale = 1.0)
    {
        // Получаем первую страницу и сохраняем ее как PNG
        IntPtr pagePtr = document_get_page(_doc, pageNumber);

        if (pagePtr == IntPtr.Zero)
        {
            throw new InvalidOperationException("Error: Could not get page.");
        }

        int resultCode = page_save_as_png(pagePtr, filename, scale);

        if (resultCode != 0)
        {
            throw new InvalidOperationException("Error: Could not save page.");
        }
    }

    public byte[] GetPageBytes(int pageNumber, double scale = 1.0)
    {
        IntPtr pagePtr = document_get_page(_doc, pageNumber);

        if (pagePtr == IntPtr.Zero)
        {
            throw new InvalidOperationException("Error: Could not get page.");
        }

        IntPtr bufferPtr = page_to_png_bytes(pagePtr, scale);
        if (bufferPtr == IntPtr.Zero)
        {
            throw new InvalidOperationException("Failed to convert page to PNG.");
        }

        try
        {
            ByteBuffer buffer = Marshal.PtrToStructure<ByteBuffer>(bufferPtr);

            ulong size = buffer.size.ToUInt64();

            if (buffer.data == IntPtr.Zero || size == 0)
            {
                throw new InvalidOperationException("Data buffer is empty");
            }

            byte[] data = new byte[size];
            Marshal.Copy(buffer.data, data, 0, (int)size);

            return data;
        }
        finally
        {
            if (pagePtr != IntPtr.Zero)
            {
                page_free(pagePtr);
            }
            if (bufferPtr != IntPtr.Zero)
            {
                free_byte_buffer(bufferPtr);
            }
        }
    }

    public void Dispose()
    {
        try
        {
            document_free(_doc);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
        }
    }

    public int TotalPages => _totalPages;
}