using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Riulax.ViewModels;

namespace Riulax.Views;

public partial class TrackPlayer : UserControl 
{
    public TrackPlayer() 
    {
        InitializeComponent();
        SliderTrack.AddHandler(PointerPressedEvent, TRACK_PointerPressed, RoutingStrategies.Tunnel);
        SliderTrack.AddHandler(PointerReleasedEvent, TRACK_PointerReleased, RoutingStrategies.Tunnel);
    }


    public void TRACK_PointerPressed(object? sender, PointerPressedEventArgs args) 
    {
        var viewModel = (TrackPlayerViewModel)DataContext!;
        viewModel.IsSeeking = true;
    }

    public void TRACK_PointerReleased(object? sender, PointerReleasedEventArgs args) 
    {
        var viewModel = (TrackPlayerViewModel)DataContext!;
        viewModel.IsSeeking = false;
    }
}