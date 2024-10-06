using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaDialogs.Views;
using Riulax.ViewModels;

namespace Riulax.Views;

public partial class AddToPlaylistView : BaseDialog<(List<PlaylistViewModel>, List<PlaylistViewModel>)>
{
    private List<PlaylistViewModel> addedItems = new List<PlaylistViewModel>();
    private List<PlaylistViewModel> removedItems = new List<PlaylistViewModel>();
    public AddToPlaylistView() 
    {
        InitializeComponent();
    }

    public void Playlist_SelectionChanged(object? sender, SelectionChangedEventArgs args) 
    {
        foreach (PlaylistViewModel item in args.AddedItems) 
        {
            addedItems.Add(item);
        }

        foreach (PlaylistViewModel item in args.RemovedItems) 
        {
            removedItems.Add(item);
        }
    }

    public void Clicked_Close(object? sender, RoutedEventArgs args) 
    {
        Close((addedItems, removedItems));
    }
}
