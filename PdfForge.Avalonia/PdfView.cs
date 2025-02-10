using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

namespace PdfForge.Avalonia;

public class PdfView : ContentControl
{
    private Image? PART_Image;
    private ContentControl? PART_LoadingIndicator;
    private ScrollViewer? PART_ScrollViewer;
    private bool _templateApplied;
    private bool _isPanning;
    private Point _lastMousePosition;
    
    static PdfView()
    {
        DocumentProperty.Changed.AddClassHandler<PdfView>((v, e) => v.InitDocument());
        PageNumberProperty.Changed.AddClassHandler<PdfView>((v, e) => v.RenderPageAsync());
        ScaleProperty.Changed.AddClassHandler<PdfView>((v, e) => v.RenderPageAsync());
    }

    public PdfView()
    {
        AddHandler(PointerPressedEvent, MousePressed, RoutingStrategies.Tunnel);
        AddHandler(PointerMovedEvent, MouseMoved, RoutingStrategies.Tunnel);
        AddHandler(PointerReleasedEvent, MouseReleased, RoutingStrategies.Tunnel);
    }

    public static readonly DirectProperty<PdfView, PdfForgeDocument?> DocumentProperty =
        AvaloniaProperty.RegisterDirect<PdfView, PdfForgeDocument?>(nameof(Document), v => v.Document,
            (s, v) => s.Document = v);

    private PdfForgeDocument? _document;

    public PdfForgeDocument? Document
    {
        get => _document;
        set => SetAndRaise(DocumentProperty, ref _document, value);
    }

    public static readonly DirectProperty<PdfView, int> PageNumberProperty =
        AvaloniaProperty.RegisterDirect<PdfView, int>(nameof(PageNumber), v => v.PageNumber,
            (s, v) => s.PageNumber = v);

    private int _pageNumber;

    public int PageNumber
    {
        get => _pageNumber;
        set => SetAndRaise(PageNumberProperty, ref _pageNumber, value);
    }

    public static readonly DirectProperty<PdfView, double> ScaleProperty =
        AvaloniaProperty.RegisterDirect<PdfView, double>(nameof(Scale), v => v.Scale, (s, v) => s.Scale = v);

    private double _scale = 1.0;

    public double Scale
    {
        get => _scale;
        set => SetAndRaise(ScaleProperty, ref _scale, value);
    }

    public static readonly DirectProperty<PdfView, double> ImageWidthProperty =
        AvaloniaProperty.RegisterDirect<PdfView, double>(nameof(ImageWidth), v => v.ImageWidth,
            (s, v) => s.ImageWidth = v);

    private double _imageWidth;

    public double ImageWidth
    {
        get => _imageWidth;
        private set => SetAndRaise(ImageWidthProperty, ref _imageWidth, value);
    }

    public static readonly DirectProperty<PdfView, double> ImageHeightProperty =
        AvaloniaProperty.RegisterDirect<PdfView, double>(nameof(ImageHeight), v => v.ImageHeight,
            (s, v) => s.ImageHeight = v);

    private double _imageHeight;

    public double ImageHeight
    {
        get => _imageHeight;
        private set => SetAndRaise(ImageHeightProperty, ref _imageHeight, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        PART_ScrollViewer = e.NameScope.Find<ScrollViewer>("PART_ScrollViewer");
        PART_Image = e.NameScope.Find<Image>("PART_RenderImage");
        PART_LoadingIndicator = e.NameScope.Find<ContentControl>("PART_LoadingIndicator");

        _templateApplied = true;

        InitDocument();
    }
        
    private void MousePressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isPanning = true;
            _lastMousePosition = e.GetPosition(this);
                
            Cursor = new Cursor(StandardCursorType.Hand);
        }
    }

    private void MouseMoved(object? sender, PointerEventArgs e)
    {
        if (_isPanning && PART_ScrollViewer != null)
        {
            Point currentPosition = e.GetPosition(this);
            Point delta = _lastMousePosition - currentPosition;

            PART_ScrollViewer.Offset = new Vector(
                PART_ScrollViewer.Offset.X + delta.X,
                PART_ScrollViewer.Offset.Y + delta.Y
            );

            _lastMousePosition = currentPosition;
        }
    }

    private void MouseReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
        {
            _isPanning = false;
            Cursor = Cursor.Default;
        }
    }

    private void InitDocument()
    {
        if (_templateApplied && Document != null)
        {
            RenderPageAsync();
        }
    }

    private async Task RenderPageAsync()
    {
        if (Document == null || PART_Image == null) return;

        SetLoading(true);

        await Task.Run(() =>
        {
            if (Dispatcher.UIThread.CheckAccess())
            {
                var pageBytes = Document.GetPageBytes(PageNumber, Scale);
                using MemoryStream stream = new (pageBytes.ToArray());
                Bitmap bitmap = new (stream);
                    
                ImageWidth = bitmap.PixelSize.Width;
                ImageHeight = bitmap.PixelSize.Height;
                PART_Image.Source = bitmap;
            }
            else
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var pageBytes = Document.GetPageBytes(PageNumber, Scale);
                    using MemoryStream stream = new (pageBytes.ToArray());
                    Bitmap bitmap = new (stream);
                        
                    ImageWidth = bitmap.PixelSize.Width;
                    ImageHeight = bitmap.PixelSize.Height;
                    PART_Image.Source = bitmap;
                });
            }
        });

        SetLoading(false);
    }

    private void SetLoading(bool isLoading)
    {
        if (PART_LoadingIndicator == null) return;
            
        if (Dispatcher.UIThread.CheckAccess())
        {
            PART_LoadingIndicator.IsVisible = isLoading;
        }
        else
        {
            Dispatcher.UIThread.InvokeAsync(() => PART_LoadingIndicator.IsVisible = isLoading);
        }
    }
}