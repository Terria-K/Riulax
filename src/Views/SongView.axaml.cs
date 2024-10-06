using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Riulax.ViewModels;

namespace Riulax.Views;

public partial class SongView : UserControl 
{
    public static readonly RoutedEvent<SongEventArgs> AddPlaylistEvent =
        RoutedEvent.Register<SongView, SongEventArgs>(nameof(AddPlaylist), RoutingStrategies.Direct);

    public event EventHandler<SongEventArgs> AddPlaylist
    {
        add => AddHandler(AddPlaylistEvent, value);
        remove => RemoveHandler(AddPlaylistEvent, value);
    }

    public SongView()
    {
        InitializeComponent();
    }

    public void OnAddClicked(object? sender, RoutedEventArgs args) 
    {
        SongEventArgs arg = new SongEventArgs((SongViewModel)DataContext!, AddPlaylistEvent);
        RaiseEvent(arg);
    }
}

public sealed class SongEventArgs : RoutedEventArgs
{
    public SongViewModel SongViewModel;

    public SongEventArgs(SongViewModel model)
    {
        SongViewModel = model;
    }

    public SongEventArgs(SongViewModel model, RoutedEvent? routedEvent) : base(routedEvent)
    {
        SongViewModel = model;
    }
}