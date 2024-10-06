using System;
using Avalonia.Interactivity;
using AvaloniaDialogs.Views;

namespace Riulax.Views;

public partial class CreatePlaylistView : BaseDialog<string>
{
    public CreatePlaylistView() 
    {
        InitializeComponent();
    }

    public void Button_Create(object? sender, RoutedEventArgs args) 
    {
        Close(PlaylistName.Text!.Trim());
    }

    public void Button_Cancel(object? sender, RoutedEventArgs args) 
    {
        Close();
    }
}
