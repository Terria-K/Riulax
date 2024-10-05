using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using Riulax.ViewModels;

namespace Riulax.Views;

public partial class MusicImporterWindow : ReactiveWindow<MusicImporterViewModel>
{
    public MusicImporterWindow()
    {
        InitializeComponent();
    }

    private async void LocateFolder_Clicked(object sender, RoutedEventArgs args)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        var folder = await topLevel!.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Open a folder",
            AllowMultiple = false
        });

        if (folder.Count < 1)
        {
            return;
        }
        var path = folder[0].TryGetLocalPath();
        if (path is null)
        {
            return;
        }
        var model = ((MusicImporterViewModel)ViewModel!);

        var newModel = new MusicFolderViewModel(new Models.MusicFolder(Ulid.NewUlid(), path, false));

        if (await AppState.Database.CheckMusicFolder(newModel))
        {
            return;
        }
        newModel.Delete += model.Delete;

        model.Folders.Add(newModel);

        await AppState.Database.AddMusicFolder(newModel);
    }
}