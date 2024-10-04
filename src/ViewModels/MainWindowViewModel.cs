using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Riulax.Models;
using LibVLCSharp.Shared;
using ReactiveUI;
using Microsoft.Data.Sqlite;
using System.Threading;
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
    private Random random = new Random();
    private Queue<int> predefinedRandom = new Queue<int>();
    private bool randoming;



    public MainWindowViewModel() 
    {
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
            ShowDialog.Handle(importer).Subscribe(x => {
                if (x == null) 
                {
                    return;
                }
                Songs.Clear();
                AddLibrary(x);
            });
        });
    }

    public async Task Start() 
    {
        List<MusicFolderViewModel> folders = new List<MusicFolderViewModel>();
        using (var connection = new SqliteConnection("Data Source=music.db")) 
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = """
            SELECT path FROM music
            """;
            using var reader = await command.ExecuteReaderAsync();
            while (reader.Read()) 
            {
                folders.Add(new MusicFolderViewModel(new MusicFolder(reader.GetString(0), false)));
            }
        }
        AddLibrary(folders);
    }

    private void AddLibrary(ICollection<MusicFolderViewModel> library) 
    {
        foreach (var folder in library)
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
    }

    public void PlaySong() 
    {
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
            Console.WriteLine(res);
        }
        else 
        {
            InternalPredefinedRandom(0);
        }
    }
}
