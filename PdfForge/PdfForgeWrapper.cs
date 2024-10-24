using System.Runtime.InteropServices;

namespace PdfForge;

public class PdfForgeWrapper
{
    private const string DllName = "pdf_forge";

    [StructLayout(LayoutKind.Sequential)]
    public struct ByteBuffer
    {
        public IntPtr data;
        public UIntPtr size;
    }

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr document_new(string filename);


    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int document_total_pages(IntPtr documentWrapper);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr document_get_page(IntPtr documentWrapper, int pageNumber);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int page_save_as_png(IntPtr pageWrapper, string filename, double scale);


    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr page_to_png_bytes(IntPtr pageWrapper, double scale);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void document_free(IntPtr documentWrapper);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void page_free(IntPtr pageWrapper);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void free_byte_buffer(IntPtr byteBuffer);
}