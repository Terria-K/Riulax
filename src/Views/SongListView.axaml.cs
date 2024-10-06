using System;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Riulax.ViewModels;

namespace Riulax.Views;

public partial class SongListView : ReactiveUserControl<MainWindowViewModel>
{
    public SongListView()
    {
        InitializeComponent();
    }


    public void PlaySong(object? sender, SelectionChangedEventArgs args) 
    {
        var model = (MainWindowViewModel)ViewModel!;
        model.PlaySong((SongViewModel)args.AddedItems[0]!);
    }

    public async void OpenAddToPlaylist(object? sender, SongEventArgs args)  
    {
        var model = (MainWindowViewModel)ViewModel!;
        await model.AddToPlaylistDialog(args.SongViewModel);
    }
}