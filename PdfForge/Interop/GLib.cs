using System.Runtime.InteropServices;

namespace PdfForge.Interop;

internal static partial class GLib
{
    private const string LibGObject = "gobject-2.0";
    
    [StructLayout(LayoutKind.Sequential)]
    public struct GError
    {
        public uint Domain;
        public int Code;
        public IntPtr Message;
        
        public string? ManagedMessage => Marshal.PtrToStringUTF8(Message);
    }

    public sealed class GObject() : SafeHandle(IntPtr.Zero, true)
    {
        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            Console.WriteLine($"{nameof(GObject)} is disposed");
            GObjectUnref(handle);
            return true;
        }
    }

    [LibraryImport(LibGObject, EntryPoint = "g_object_unref")]
    public static partial void GObjectUnref(IntPtr gObject);
    
    [LibraryImport(LibGObject, EntryPoint = "g_error_free")]
    public static partial void GErrorFree(IntPtr gError);
}