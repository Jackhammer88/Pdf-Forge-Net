using System.Runtime.InteropServices;

namespace PdfForge.Interop;

internal static partial class Cairo
{
    private const string LibCairo = "cairo";
    
    internal enum CairoFormat {
        Invalid = -1,
        Argb32 = 0,
        Rgb24 = 1,
        A8 = 2,
        A1 = 3,
        Rgb16565 = 4,
        Rgb30 = 5,
    }
    
    internal enum CairoStatus : int 
    {
        CairoStatusSuccess = 0,
        CairoStatusNoMemory,
        CairoStatusInvalidRestore,
        CairoStatusInvalidPopGroup,
        CairoStatusNoCurrentPoint,
        CairoStatusInvalidMatrix,
        CairoStatusInvalidStatus,
        CairoStatusNullPointer,
        CairoStatusInvalidString,
        CairoStatusInvalidPathData,
        CairoStatusReadError,
        CairoStatusWriteError,
        CairoStatusSurfaceFinished,
        CairoStatusSurfaceTypeMismatch,
        CairoStatusPatternTypeMismatch,
        CairoStatusInvalidContent,
        CairoStatusInvalidFormat,
        CairoStatusInvalidVisual,
        CairoStatusFileNotFound,
        CairoStatusInvalidDash,
        CairoStatusInvalidDscComment,
        CairoStatusInvalidIndex,
        CairoStatusClipNotRepresentable,
        CairoStatusTempFileError,
        CairoStatusInvalidStride,
        CairoStatusFontTypeMismatch,
        CairoStatusUserFontImmutable,
        CairoStatusUserFontError,
        CairoStatusNegativeCount,
        CairoStatusInvalidClusters,
        CairoStatusInvalidSlant,
        CairoStatusInvalidWeight,
        CairoStatusInvalidSize,
        CairoStatusUserFontNotImplemented,
        CairoStatusDeviceTypeMismatch,
        CairoStatusDeviceError,
        CairoStatusInvalidMeshConstruction,
        CairoStatusDeviceFinished,
        CairoStatusJbig2GlobalMissing,
        CairoStatusPngError,
        CairoStatusFreetypeError,
        CairoStatusWin32GdiError,
        CairoStatusTagError,
        CairoStatusLastStatus,
    }
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate CairoStatus CairoWriteFunction(IntPtr cVoid, IntPtr buffer, uint bufferSize);
    
    [LibraryImport(LibCairo, EntryPoint = "cairo_image_surface_create")]
    public static partial CairoSurfaceT ImageSurfaceCreate(CairoFormat format, int width, int height);

    [LibraryImport(LibCairo, EntryPoint = "cairo_surface_status")]
    public static partial CairoStatus cairo_surface_status(CairoSurfaceT surface);
    
    [LibraryImport(LibCairo, EntryPoint = "cairo_status")]
    public static partial CairoStatus cairo_status(CairoT cairo);

    [LibraryImport(LibCairo, EntryPoint = "cairo_status_to_string",
        StringMarshalling = StringMarshalling.Utf8)]
    public static partial string cairo_status_to_string(CairoStatus status);

    [LibraryImport(LibCairo, EntryPoint = "cairo_create")]
    public static partial CairoT cairo_create(CairoSurfaceT surface);

    [LibraryImport(LibCairo, EntryPoint = "cairo_surface_destroy")]
    public static partial void cairo_surface_destroy(IntPtr surface);
    
    [LibraryImport(LibCairo, EntryPoint = "cairo_destroy")]
    public static partial void cairo_destroy(IntPtr cairo);

    [LibraryImport(LibCairo, EntryPoint = "cairo_scale")]
    public static partial void cairo_scale(CairoT cairo, double sx, double sy);

    [LibraryImport(LibCairo, EntryPoint = "cairo_surface_flush")]
    public static partial void cairo_surface_flush(CairoSurfaceT surface);

    [LibraryImport(LibCairo, EntryPoint = "cairo_surface_write_to_png",
        StringMarshalling = StringMarshalling.Utf8)]
    public static partial CairoStatus cairo_surface_write_to_png(
        CairoSurfaceT surface, string filename);
    
    [LibraryImport(LibCairo, 
        EntryPoint = "cairo_surface_write_to_png_stream")]
    public static partial CairoStatus cairo_surface_write_to_png_stream(
        CairoSurfaceT surface,
        CairoWriteFunction writeFunc,
        IntPtr closure);
}