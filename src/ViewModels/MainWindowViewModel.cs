using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AvaloniaTest.Models;
using LibVLCSharp.Shared;
using ReactiveUI;
namespace AvaloniaTest.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private SongViewModel? selectedSong;
    public SongViewModel? SelectedSong 
    {
        get => selectedSong;
        set => this.RaiseAndSetIfChanged(ref selectedSong, value);
    }
    public LibVLC LibVLC { get; }
    public ICommand MyMusicCommand { get; }
    public Interaction<MusicImporterViewModel, ICollection<MusicFolderViewModel>?> ShowDialog { get; }
    public ObservableCollection<SongViewModel> Songs { get; }
    private MediaPlayer? player;
    private float time;
    private bool isSeeking;
    private bool isLooping;
    private bool playAfterStop;

    public bool IsLooping 
    {
        get => isLooping;
        set => this.RaiseAndSetIfChanged(ref isLooping, value);
    }

    public float Time 
    {
        get => (player != null ? time / (float)player.Length : time) * 100;
        set 
        {
            if (player == null) 
            {
                return;
            }
            int newValue = (int)((value / 100) * player.Length);

            this.RaiseAndSetIfChanged(ref time, newValue);
            if (isSeeking) 
            {
                player.SeekTo(TimeSpan.FromMilliseconds(time));
            }
        }
    } 

    public float RawTime 
    {
        get => time;
        set 
        {
            if (player == null) 
            {
                return;
            }
            float val = value;
            Time = (val / (float)player.Length) * 100;
        }
    }


    public bool IsSeeking 
    {
        get => isSeeking;
        set 
        {
            isSeeking = value;
            if (player == null) 
            {
                return;
            }

            if (isSeeking) 
            {
                player.Pause();
                return;
            }
            player.Play();
        }
    }
    

    public MainWindowViewModel() 
    {
        Songs = new ObservableCollection<SongViewModel>();
        LibVLC = new LibVLC("--no-video");
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

    public void PauseOrPlaySong() 
    {
        if (selectedSong == null) 
        {
            return;
        }

        if (player == null) 
        {
            return;
        }
        if (player.State == VLCState.Playing) 
        {
            player.Pause();
            return;
        }
        player.Play();
    }

    public void PlaySong() 
    {
        if (selectedSong == null) 
        {
            return;
        }
        if (player != null && player.State != VLCState.Stopped) 
        {
            player.Stop();
        }


        player = new MediaPlayer(LibVLC);
        player.Stopped += (sender, e) => ThreadPool.QueueUserWorkItem(_ => Stopped(sender, e));
        player.TimeChanged += TimeChanged;
        player.EndReached += (sender, e) => ThreadPool.QueueUserWorkItem(_ => EndReached(sender, e));


        using Media media = new Media(LibVLC, selectedSong.Path);
        player.Play(media);
    }

    private void EndReached(object? sender, EventArgs e) 
    {
        if (player == null) 
        {
            return;
        }

        if (IsLooping) 
        {
            playAfterStop = true;
            player.Play();
        }
    }

    private void TimeChanged(object? sender, MediaPlayerTimeChangedEventArgs e)
    {
        RawTime = e.Time;
    }

    private void Stopped(object? sender, EventArgs e)
    {
        player?.Dispose();
        player = null;
        if (playAfterStop) 
        {
            playAfterStop = false;
            PlaySong();
        }
    }
}

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