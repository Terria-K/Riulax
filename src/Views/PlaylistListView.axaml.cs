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
        if (args.AddedItems.Count == 0) { return; } // yeah, crashes if the playlist is deleted 
        var model = (MainWindowViewModel)ViewModel!;
        await model.SelectPlaylist((PlaylistViewModel)args.AddedItems[0]!);
    }

    public async void DeletePlaylist(object? sender, PlaylistEventArgs args) 
    {
        var model = (MainWindowViewModel)ViewModel!;
        await model.DeletePlaylist(args.PlaylistViewModel);
    }
}