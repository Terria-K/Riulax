using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Riulax.ViewModels;

namespace Riulax.Views;

public partial class PlaylistListView: ReactiveUserControl<MainWindowViewModel>
{
    public PlaylistListView()
    {
        InitializeComponent();
    }

    public async void SelectPlaylist(object? sender, SelectionChangedEventArgs args) 
    {
        var model = (MainWindowViewModel)ViewModel!;
        await model.SelectPlaylist((PlaylistViewModel)args.AddedItems[0]!);
    }
}