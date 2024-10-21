using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using PdfForge;
using ReactiveUI;

namespace TestUI.ViewModels;

public class MainWindowViewModel : ViewModelBase, IActivatableViewModel
{
    private readonly BehaviorSubject<double> _scaleBehavior = new(1);
    public MainWindowViewModel()
    {
        PdfDocument = new("test.pdf");

        const double minScaleValue = 0.5;
        const double maxScaleValue = 10;
        _scaleHelper = _scaleBehavior
            .Scan((acc, scaleChange) =>
            {
                double newScale = acc + scaleChange;
                if (newScale < minScaleValue) newScale = minScaleValue;
                if (newScale > maxScaleValue) newScale = maxScaleValue;
                return newScale;
            })
            .Do(v =>
            {
                Scale = v;
            })
            .Throttle(TimeSpan.FromMilliseconds(500))
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, vm => vm.RealScale, 1.0);

        this.WhenAnyValue(vm => vm.RealScale)
            .Subscribe(s => Scale = s);

        var scaleObs = this.WhenAnyValue(vm => vm.RealScale);
        
        DecreaseQualityCommand = ReactiveCommand.Create(() => _scaleBehavior.OnNext(-minScaleValue), 
            scaleObs.Select(s => s > minScaleValue), RxApp.MainThreadScheduler);
        
        IncreaseQualityCommand = ReactiveCommand.Create(() => _scaleBehavior.OnNext(minScaleValue), 
            scaleObs.Select(s => s < maxScaleValue), RxApp.MainThreadScheduler);
        
        int totalPages = PdfDocument.TotalPages;
        var pageNumberObs = this.WhenAnyValue(vm => vm.PageNumber);
        
        NextPageCommand = ReactiveCommand.Create(() =>
        {
            _scaleBehavior.OnNext(1 - RealScale);
            return PageNumber++;
        }, pageNumberObs.Select(n => n < totalPages - 1));
        PrevPageCommand = ReactiveCommand.Create(() =>
        {
            _scaleBehavior.OnNext(1 - RealScale);
            return PageNumber--;
        }, pageNumberObs.Select(n => n > 0));
        
        this.WhenActivated(d =>
        {
            PdfDocument.DisposeWith(d); // It will be disposed anyway =)
        });
    }

    private double _zoom = 1.0;
    public double Zoom
    {
        get => _zoom;
        private set => this.RaiseAndSetIfChanged(ref _zoom, value);
    }

    private PdfForgeDocument? _pdfDocument;

    public PdfForgeDocument? PdfDocument
    {
        get => _pdfDocument;
        private set => this.RaiseAndSetIfChanged(ref _pdfDocument, value);
    }

    private int _pageNumber;

    public int PageNumber
    {
        get => _pageNumber;
        private set => this.RaiseAndSetIfChanged(ref _pageNumber, value);
    }

    private double _scale;

    public double Scale
    {
        get => _scale;
        set => this.RaiseAndSetIfChanged(ref _scale, value);
    }
    
    private ObservableAsPropertyHelper<double> _scaleHelper;
    public double RealScale => _scaleHelper.Value;
    
    public ReactiveCommand<Unit, Unit> DecreaseQualityCommand { get; }
    public ReactiveCommand<Unit, Unit> IncreaseQualityCommand { get; }
    
    public ReactiveCommand<Unit, int> PrevPageCommand { get; }
    public ReactiveCommand<Unit, int> NextPageCommand { get; }
    

    public ViewModelActivator Activator { get; } = new();
}