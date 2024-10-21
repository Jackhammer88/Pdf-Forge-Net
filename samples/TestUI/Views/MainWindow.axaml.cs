using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using ReactiveUI;
using TestUI.ViewModels;

namespace TestUI.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            if (ViewModel is null) return;
            
            Observable.FromEventPattern<PointerWheelEventArgs>(
                    h => PointerWheelChanged += h,
                    h => PointerWheelChanged -= h)
                .Select(e => e.EventArgs.Delta.Y)
                .Subscribe(deltaY =>
                {
                    if (deltaY > 0)
                    {
                        ViewModel?.IncreaseQualityCommand.Execute().Subscribe();
                    }
                    else if (deltaY < 0)
                    {
                        ViewModel?.DecreaseQualityCommand.Execute().Subscribe();
                    }
                })
                .DisposeWith(d);
        });
    }
}