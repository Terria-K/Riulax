using AvaloniaTest.Models;

namespace AvaloniaTest.ViewModels;

public partial class AlbumViewModel : ViewModelBase
{
    public string Title => album.Title;
    public string Artist => album.Artist;
    private Album album;
    public AlbumViewModel(Album album) 
    {
        this.album = album;
    }
}