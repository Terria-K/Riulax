using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Riulax.Models;
using LibVLCSharp.Shared;
using ReactiveUI;
using DynamicData;

namespace Riulax.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IScreen, IRoutableViewModel
{
    public enum Route { Home, Songs, Playlist }
    public Route ViewRoute = Route.Songs;
    public RoutingState Router { get; } = new RoutingState();
    public LibVLC LibVLC { get; }
    public SongViewModel? SelectedSong 
    {
        get => selectedSong;
        set 
        {
            trackPlayerViewModel.SelectedSong = value!;
            this.RaiseAndSetIfChanged(ref selectedSong, value);
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

    public string? UrlPathSegment { get; } = Ulid.NewUlid().ToString();

    public IScreen HostScreen { get; }

    private TrackPlayerViewModel trackPlayerViewModel;
    private SongViewModel? selectedSong;
    private Random random = new Random();
    private Queue<int> predefinedRandom = new Queue<int>();
    private bool randoming;



    public MainWindowViewModel() 
    {
        HostScreen = this;
        LibVLC = new LibVLC("--no-video");
        trackPlayerViewModel = new TrackPlayerViewModel(LibVLC);
        trackPlayerViewModel.NextEvent += Next;
        trackPlayerViewModel.PrevEvent += Prev;
        trackPlayerViewModel.RandomEvent += Random;
        Songs = new ObservableCollection<SongViewModel>();
        ShowDialog = new Interaction<MusicImporterViewModel, ICollection<MusicFolderViewModel>?>();
        MyMusicCommand = ReactiveCommand.Create(() => 
        {
            var importer = new MusicImporterViewModel();
            ShowDialog.Handle(importer).Subscribe(async x => {
                if (x == null) 
                {
                    return;
                }
                Songs.Clear();
                await IndexLibrary(x);
                await Start();
            });
        });
    }

    public async Task Start() 
    {
        List<SongViewModel> songs = await AppState.Database.GetAllSong();
        Songs.AddRange(songs);
    }

    public void ChangeView(object msg) 
    {
        if (msg is "Playlist") 
        {
            ViewRoute = Route.Playlist;
            Router.Navigate.Execute(this);
            return;
        }
        if (msg is "Songs") 
        {
            ViewRoute = Route.Songs;
            Router.Navigate.Execute(this);
        }
    }

    private async Task IndexLibrary(ICollection<MusicFolderViewModel> library) 
    {
        var connection = await AppState.Database.StartAddSongConnection();
        foreach (var folder in library)
        {
            if (folder.HasIndexed) { continue; }
            var files = Directory.GetFiles(folder.Path);
            foreach (var file in files) 
            {
                using Media media = new Media(LibVLC, file);
                var task = Task.Run(async () => await media.Parse(MediaParseOptions.ParseLocal)); 
                task.Wait();
                var title = media.Meta(MetadataType.Title) ?? "";
                var artist = media.Meta(MetadataType.Artist) ?? "";
                var url = media.Meta(MetadataType.ArtworkURL) ?? "";
                var date = media.Meta(MetadataType.Date) ?? "";
                SongViewModel model = new SongViewModel(new Song(Ulid.NewUlid(), file, title, artist, date, url, Ulid.NewUlid()));
                media.ParseStop();

                await AppState.Database.AddSongPrepare(connection, folder, model);
            }

            await AppState.Database.IndexFolder(folder);
        }
        await AppState.Database.EndAddSongConnection(connection);
    }

    public void PlaySong(SongViewModel song) 
    {
        SelectedSong = song;
        trackPlayerViewModel.PlaySong();
        if (randoming) 
        {
            randoming = false;
            return;
        }
        int index = Songs.IndexOf(trackPlayerViewModel.SelectedSong!);
        InternalPredefinedRandom(index);
    }

    private void InternalPredefinedRandom(int index) 
    {
        predefinedRandom.Clear();
        for (int i = 1; i < Songs.Count; i++) 
        {
            int rand = index;
            while (rand == index || predefinedRandom.Contains(rand)) 
            {
                rand = random.Next(0, Songs.Count);
            }

            predefinedRandom.Enqueue(rand);
        }
    }

    private void Next() 
    {
        if (trackPlayerViewModel.SelectedSong == null) 
        {
            return;
        }
        int index = Songs.IndexOf(trackPlayerViewModel.SelectedSong);
        if (index < Songs.Count - 1) 
        {
            SelectedSong = Songs[index + 1];
            return;
        }
    }

    private void Prev() 
    {
        if (trackPlayerViewModel.SelectedSong == null) 
        {
            return;
        }
        int index = Songs.IndexOf(trackPlayerViewModel.SelectedSong);
        if (index == 0) 
        {
            return;
        }
        SelectedSong = Songs[index - 1]; 
    }

    private void Random() 
    {
        if (trackPlayerViewModel.SelectedSong == null) 
        {
            return;
        }
        randoming = true;
        if (predefinedRandom.TryDequeue(out int res)) 
        {
            SelectedSong = Songs[res]; 
        }
        else 
        {
            InternalPredefinedRandom(0);
        }
    }
}
