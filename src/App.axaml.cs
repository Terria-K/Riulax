using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Riulax.ViewModels;
using Riulax.Views;
using HotAvalonia;

namespace Riulax;

public partial class App : Application
{
    public override void Initialize()
    {
        this.EnableHotReload();
        AvaloniaXamlLoader.Load(this);

        ViewLocator.Register<AlbumViewModel, AlbumView>();
        ViewLocator.Register<SongViewModel, SongView>();
        ViewLocator.Register<MusicFolderViewModel, MusicFolderView>();
        ViewLocator.Register<PlaylistViewModel, PlaylistView>();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
