using System.Runtime.InteropServices;

namespace PdfForge.Interop;

internal sealed class CairoT() : SafeHandle(IntPtr.Zero, true)
{
    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        Cairo.cairo_destroy(handle);
        return true;
    }
}