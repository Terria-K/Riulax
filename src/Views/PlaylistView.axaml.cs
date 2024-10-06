using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Riulax.ViewModels;

namespace Riulax.Views;

public partial class PlaylistView: UserControl 
{
    public static readonly RoutedEvent<SongEventArgs> DeletePlaylistEvent =
        RoutedEvent.Register<SongView, SongEventArgs>(nameof(DeletePlaylist), RoutingStrategies.Direct);

    public event EventHandler<PlaylistEventArgs> DeletePlaylist
    {
        add => AddHandler(DeletePlaylistEvent, value);
        remove => RemoveHandler(DeletePlaylistEvent, value);
    }

    public PlaylistView()
    {
        InitializeComponent();
    }

    public void OnDelete(object? sender, RoutedEventArgs args) 
    {
        PlaylistEventArgs arg = new PlaylistEventArgs((PlaylistViewModel)DataContext!, DeletePlaylistEvent);
        RaiseEvent(arg);
    }
}

public sealed class PlaylistEventArgs : RoutedEventArgs
{
    public PlaylistViewModel PlaylistViewModel;

    public PlaylistEventArgs(PlaylistViewModel model)
    {
        PlaylistViewModel = model;
    }

    public PlaylistEventArgs(PlaylistViewModel model, RoutedEvent? routedEvent) : base(routedEvent)
    {
        PlaylistViewModel = model;
    }
}