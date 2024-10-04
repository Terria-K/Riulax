using System;
using System.Threading;
using LibVLCSharp.Shared;
using ReactiveUI;

namespace Riulax.ViewModels;

public class TrackPlayerViewModel : ViewModelBase 
{
    public LibVLC LibVLC { get; }
    public SongViewModel? SelectedSong 
    {
        get => selectedSong;
        set => this.RaiseAndSetIfChanged(ref selectedSong, value);
    }
    private SongViewModel? selectedSong;
    private MediaPlayer? player;
    private float time;
    private bool isSeeking;
    private bool isLooping;
    private bool playAfterStop;

    public TrackPlayerViewModel(LibVLC libVLC)
    {
        LibVLC = libVLC; 
    }

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