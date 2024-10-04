using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Riulax.Models;
using LibVLCSharp.Shared;
using ReactiveUI;
namespace Riulax.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public LibVLC LibVLC { get; }
    public SongViewModel? SelectedSong 
    {
        get => selectedSong;
        set 
        {
            this.RaiseAndSetIfChanged(ref selectedSong, value);
            trackPlayerViewModel.SelectedSong = value!;
        }
    }
    public ICommand MyMusicCommand { get; }
    public Interaction<MusicImporterViewModel, ICollection<MusicFolderViewModel>?> ShowDialog { get; }
    public ObservableCollection<SongViewModel> Songs { get; }
    public TrackPlayerViewModel TrackPlayerViewModel 
    {
        get => trackPlayerViewModel;
        set => this.RaiseAndSetIfChanged(ref trackPlayerViewModel, value);
    }

    private TrackPlayerViewModel trackPlayerViewModel;
    private SongViewModel? selectedSong;



    public MainWindowViewModel() 
    {
        LibVLC = new LibVLC("--no-video");
        trackPlayerViewModel = new TrackPlayerViewModel(LibVLC);
        Songs = new ObservableCollection<SongViewModel>();
        ShowDialog = new Interaction<MusicImporterViewModel, ICollection<MusicFolderViewModel>?>();
        MyMusicCommand = ReactiveCommand.Create(() => 
        {
            var importer = new MusicImporterViewModel();
            ShowDialog.Handle(importer).Subscribe(x => {
                if (x == null) 
                {
                    return;
                }
                foreach (var folder in x)
                {
                    var files = Directory.GetFiles(folder.Path);
                    foreach (var file in files) 
                    {
                        using Media media = new Media(LibVLC, file);
                        var task = Task.Run(async () => await media.Parse(MediaParseOptions.ParseLocal)); 
                        task.Wait();
                        var title = media.Meta(MetadataType.Title) ?? "";
                        var artist = media.Meta(MetadataType.Artist) ?? "";
                        var url = media.Meta(MetadataType.ArtworkURL) ?? "";
                        Songs.Add(new SongViewModel(new Song(title, artist, url, file)));
                        media.ParseStop();
                    }
                }
            });
        });
    }

    public void PlaySong() 
    {
        trackPlayerViewModel.PlaySong();
    }
}
