using System;
using System.Threading;
using LibVLCSharp.Shared;
using ReactiveUI;

namespace Riulax.ViewModels;

public class TrackPlayerViewModel : ViewModelBase 
{
    public enum PlayerShufflingState { Once, NextOnly, Shuffle }
    public LibVLC LibVLC { get; }
    public SongViewModel? SelectedSong 
    {
        get => selectedSong;
        set => this.RaiseAndSetIfChanged(ref selectedSong, value);
    }

    public string PauseIcon 
    {
        get => icon;
        set => this.RaiseAndSetIfChanged(ref icon, value);
    }

    public string StateIcon 
    {
        get => stateIcon;
        set => this.RaiseAndSetIfChanged(ref stateIcon, value);
    }

    public PlayerShufflingState PlayerState 
    {
        get => playerState;
        set 
        {
            this.RaiseAndSetIfChanged(ref playerState, value);
            switch (PlayerState) 
            {
            case PlayerShufflingState.Once:
                StateIcon = "mdi-numeric-1-circle-outline";
                break;
            case PlayerShufflingState.NextOnly:
                StateIcon = "mdi-shuffle-disabled";
                break;
            default:
                StateIcon = "mdi-shuffle";
                break;
            }
        } 
    }

    public bool IsLooping 
    {
        get => isLooping;
        set => this.RaiseAndSetIfChanged(ref isLooping, value);
    }

    public float Time 
    {
        get => (time / length) * 100;
        set 
        {
            float newValue = (value / 100.0f) * length;

            this.RaiseAndSetIfChanged(ref time, newValue);
            if (player != null && isSeeking) 
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
            float val = value;
            Time = (val / length) * 100;
            this.RaisePropertyChanged(nameof(RawTime));
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

            if (PauseIcon == "mdi-play") 
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
    public float Length 
    {
        get => length;
        set => this.RaiseAndSetIfChanged(ref length, value);
    }

    public event Action? NextEvent;
    public event Action? PrevEvent;
    public event Action? RandomEvent;
    private SongViewModel? selectedSong;
    private MediaPlayer? player;
    private float time;
    private float length;
    private bool isSeeking;
    private bool isLooping;
    private bool playAfterStop;
    private string icon = "mdi-pause";
    private string stateIcon = "mdi-numeric-1-circle-outline";
    private PlayerShufflingState playerState;

    public TrackPlayerViewModel(LibVLC libVLC)
    {
        LibVLC = libVLC; 
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
            PauseIcon = "mdi-play";
            return;
        }
        PauseIcon = "mdi-pause";
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
        player.LengthChanged += (sender, e) => ThreadPool.QueueUserWorkItem(_ => Length = player.Length);

        using Media media = new Media(LibVLC, selectedSong.Path);
        player.Play(media);
        PauseIcon = "mdi-pause";
    }

    public void NextSong() 
    {
        NextEvent?.Invoke();
        PlaySong();
    }

    public void PrevSong() 
    {
        PrevEvent?.Invoke();
        PlaySong();
    }

    public void RandomSong() 
    {
        RandomEvent?.Invoke();
        PlaySong();
    }

    public void ChangeState() 
    {
        PlayerState = PlayerState switch {
            PlayerShufflingState.Once => PlayerShufflingState.NextOnly,
            PlayerShufflingState.NextOnly => PlayerShufflingState.Shuffle,
            PlayerShufflingState.Shuffle => PlayerShufflingState.Once,
            _ => PlayerShufflingState.Once
        };
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
            return;
        }
        switch (PlayerState) 
        {
        case PlayerShufflingState.NextOnly:
            NextSong();
            break;
        case PlayerShufflingState.Shuffle:
            RandomSong();
            break;
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