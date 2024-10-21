using System.Runtime.InteropServices;

namespace PdfForge;

public class PdfForgeWrapper
{
#if OS_LINUX
    private const string DllName = "runtimes/linux-x64/native/libpdf_forge.so";
#elif OS_WINDOWS
    private const string DllName = "runtimes/win-x64/native/libpdf_forge.dll";
#else
#endif

    [StructLayout(LayoutKind.Sequential)]
    public struct ByteBuffer
    {
        public IntPtr data; // Указатель на данные
        public IntPtr size; // Размер данных
        public IntPtr capacity; // Емкость буфера
    }

    // Импорт функции для создания документа
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr document_new(string filename);

    // Импорт функции для получения общего количества страниц
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int document_total_pages(IntPtr doc);

    // Импорт функции для получения страницы
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr document_get_page(IntPtr doc, int page_number, double scale);

    // Импорт функции для сохранения страницы как PNG
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int page_save_as_png(IntPtr page, string filename);

    // Импорт функции для конвертации страницы в PNG байты
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr page_to_png_bytes(IntPtr page);

    // Импорт функции для освобождения ресурсов документа
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void document_free(IntPtr doc);

    // Импорт функции для освобождения ресурсов страницы
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void page_free(IntPtr page);

    // Импорт функции для получения данных буфера
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_byte_buffer_data(IntPtr buffer);

    // Импорт функции для получения размера буфера
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int get_byte_buffer_size(IntPtr buffer);

    // Импорт функции для получения емкости буфера
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int get_byte_buffer_capacity(IntPtr buffer);

    // Импорт функции для освобождения памяти буфера
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void free_byte_buffer(IntPtr buffer);
}