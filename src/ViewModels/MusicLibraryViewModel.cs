using System.Collections.ObjectModel;
using Riulax.Models;
using ReactiveUI;

namespace Riulax.ViewModels;

public partial class MusicLibraryViewModel : ViewModelBase
{
    public ObservableCollection<AlbumViewModel> SearchResults { get; } = new();
    public string? SearchText 
    {
        get => searchText;
        set => this.RaiseAndSetIfChanged(ref searchText, value);
    }

    public bool IsBusy 
    {
        get => isBusy;
        set => this.RaiseAndSetIfChanged(ref isBusy, value);
    }

    public AlbumViewModel? SelectedAlbum 
    {
        get => selectedAlbum;
        set => this.RaiseAndSetIfChanged(ref selectedAlbum, value);
    }

    private AlbumViewModel? selectedAlbum;
    private string? searchText;
    private bool isBusy;

    public MusicLibraryViewModel() 
    {
        SearchResults.Add(new AlbumViewModel(new Album("Planet", "Spica")));
        SearchResults.Add(new AlbumViewModel(new Album("I love", "Pizza")));
        SearchResults.Add(new AlbumViewModel(new Album("Laptop", "Mobile")));
    }
}
