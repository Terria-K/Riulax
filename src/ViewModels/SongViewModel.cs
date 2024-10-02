using AvaloniaTest.Models;
namespace AvaloniaTest.ViewModels;

public partial class SongViewModel : ViewModelBase 
{
    public string Title => song.Title;
    public string Artist => song.Artist;
    public string Path => song.Path;
    public string ArtworkURL => song.ArtworkUrl;

    private Song song;
    public SongViewModel(Song song) 
    {
        this.song = song;
    }
}