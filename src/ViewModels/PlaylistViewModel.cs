using System;
using Riulax.Models;
namespace Riulax.ViewModels;

public partial class PlaylistViewModel : ViewModelBase 
{
    public Ulid ID => playlist.ID;
    public string Name => playlist.Name;
    private Playlist playlist;
    public PlaylistViewModel(Playlist playlist) 
    {
        this.playlist = playlist;
    }
}