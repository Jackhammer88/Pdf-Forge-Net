using System.Runtime.InteropServices;

namespace PdfForge.Interop;

internal sealed class CairoSurfaceT() : SafeHandle(IntPtr.Zero, true)
{
    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        Cairo.cairo_surface_destroy(handle);
        return true;
    }
}