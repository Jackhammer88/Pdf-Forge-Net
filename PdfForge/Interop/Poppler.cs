using System.Runtime.InteropServices;

namespace PdfForge.Interop;

internal static partial class Poppler
{
    private const string LibPoppler = "poppler-glib";
    
    public sealed class PopplerDocumentHandle() : SafeHandle(IntPtr.Zero, true)
    {
        private readonly object _lock = new();
        public override bool IsInvalid => handle == IntPtr.Zero;

        public PopplerPageHandle GetPage(int pageIndex)
        {
            if (IsInvalid || IsClosed)
            {
                throw new ObjectDisposedException(nameof(PopplerDocumentHandle));
            }

            lock (_lock)
            {
                return poppler_document_get_page(this, pageIndex);
            }
        }

        public int TotalPages => poppler_document_get_n_pages(this);

        protected override bool ReleaseHandle()
        {
            GLib.GObjectUnref(handle);
            return true;
        }
    }

    public sealed class PopplerPageHandle() : SafeHandle(IntPtr.Zero, true)
    {
        public override bool IsInvalid => handle == IntPtr.Zero;

        public void ToPngFile(string filename, double scale = 1)
        {
            var (surface, context) = RenderPageToSurface(scale);

            using var cairoSurface = surface;
            using var cairoContext = context;

            var writingStatus = Cairo.cairo_surface_write_to_png(cairoSurface, filename);
            if (writingStatus != Cairo.CairoStatus.CairoStatusSuccess)
            {
                string statusString = Cairo.cairo_status_to_string(writingStatus);
                throw new InvalidOperationException("Failed to create Cairo surface: status: " + statusString);
            }
        }

        public Memory<byte> GetPngBytes(double scale = 1)
        {
            var (surface, context) = RenderPageToSurface(scale);

            using var cairoSurface = surface;
            using var cairoContext = context;

            // Создаем MemoryStream для накопления PNG-данных
            using var ms = new MemoryStream();
            // Упаковываем поток в GCHandle, чтобы получить стабильный указатель для замыкания
            GCHandle gcHandle = GCHandle.Alloc(ms);

            try
            {
                IntPtr closure = GCHandle.ToIntPtr(gcHandle);

                // Вызываем нативную функцию, которая будет вызывать наш WriteToMemory
                Cairo.CairoStatus writingStatus =
                    Cairo.cairo_surface_write_to_png_stream(cairoSurface, WriteToMemory, closure);
                if (writingStatus != Cairo.CairoStatus.CairoStatusSuccess)
                {
                    string errorString = Cairo.cairo_status_to_string(writingStatus);
                    throw new InvalidOperationException($"Ошибка записи PNG в память: {errorString}");
                }

                // После успешной записи возвращаем накопленные данные
                return
                    ms.TryGetBuffer(out ArraySegment<byte> segment)
                        ? segment.AsMemory(0, segment.Count)
                        : ms.ToArray();
            }
            finally
            {
                gcHandle.Free();
            }
        }


        private (CairoSurfaceT, CairoT) RenderPageToSurface(double scale)
        {
            // Получение размеров страницы.
            poppler_page_get_size(this, out var width, out var height);

            // Увеличение размеров страницы на заданный масштаб.
            int scaledWidth = (int)Math.Ceiling(width * scale);
            int scaledHeight = (int)Math.Ceiling(height * scale);

            // Создание cairo-поверхности.
            var surface = Cairo.ImageSurfaceCreate(Cairo.CairoFormat.Argb32, scaledWidth, scaledHeight);
            if (surface.IsInvalid)
            {
                throw new InvalidOperationException("Failed to create Cairo surface: surface is null.");
            }

            var surfaceStatus = Cairo.cairo_surface_status(surface);
            if (surfaceStatus != Cairo.CairoStatus.CairoStatusSuccess)
            {
                throw new InvalidOperationException("Failed to create Cairo surface: status: " + surfaceStatus);
            }

            // Создание контекста cairo.
            var cairoContext = Cairo.cairo_create(surface);
            var contextStatus = Cairo.cairo_status(cairoContext);
            if (contextStatus != Cairo.CairoStatus.CairoStatusSuccess)
            {
                throw new InvalidOperationException("Failed to create cairo context: " + cairoContext);
            }

            Cairo.cairo_scale(cairoContext, scale, scale);
            poppler_page_render(this, cairoContext);
            Cairo.cairo_surface_flush(surface);

            return (surface, cairoContext);
        }

        private static Cairo.CairoStatus WriteToMemory(IntPtr closure, IntPtr data, uint length)
        {
            // Извлекаем GCHandle из closure и получаем объект (MemoryStream)
            GCHandle handle = GCHandle.FromIntPtr(closure);
            if (handle.Target == null)
            {
                return Cairo.CairoStatus.CairoStatusNullPointer;
            }

            MemoryStream stream = (MemoryStream)handle.Target;

            // Создаем управляемый массив нужного размера
            byte[] buffer = new byte[length];
            // Копируем данные из нативной памяти в управляемый массив
            Marshal.Copy(data, buffer, 0, (int)length);
            // Записываем данные в поток
            stream.Write(buffer, 0, (int)length);

            return Cairo.CairoStatus.CairoStatusSuccess;
        }

        protected override bool ReleaseHandle()
        {
            GLib.GObjectUnref(handle);
            return true;
        }
    }

    [LibraryImport(LibPoppler, EntryPoint = "poppler_document_new_from_file",
        StringMarshalling = StringMarshalling.Utf8)]
    public static partial PopplerDocumentHandle poppler_document_new_from_file(string uri, string password,
        out IntPtr errorPtr);

    [LibraryImport(LibPoppler, EntryPoint = "poppler_document_get_page")]
    public static partial PopplerPageHandle poppler_document_get_page(PopplerDocumentHandle document, int index);

    [LibraryImport(LibPoppler, EntryPoint = "poppler_document_get_n_pages")]
    private static partial int poppler_document_get_n_pages(PopplerDocumentHandle document);


    [LibraryImport(LibPoppler, EntryPoint = "poppler_page_get_size")]
    private static partial void poppler_page_get_size(PopplerPageHandle page, out double width, out double height);

    [LibraryImport(LibPoppler, EntryPoint = "poppler_page_render")]
    private static partial void poppler_page_render(PopplerPageHandle page, CairoT cairo);
}