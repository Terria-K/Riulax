using System;
using Riulax.Models;
namespace Riulax.ViewModels;

public partial class SongViewModel : ViewModelBase 
{
    public Ulid ID => song.ID;
    public string Path => song.Path;
    public string Title => song.Title;
    public string Artist => song.Artist;
    public string Date => song.Date;
    public string ArtworkURL => song.ArtworkUrl;

    private Song song;
    public SongViewModel(Song song) 
    {
        this.song = song;
    }
}