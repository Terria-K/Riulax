using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using Riulax.ViewModels;
using Microsoft.Data.Sqlite;

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

        if (folder.Count >= 1)
        {
            var path = folder[0].TryGetLocalPath();
            if (path is null)
            {
                return;
            }
            var model = ((MusicImporterViewModel)ViewModel!);

            var newModel = new MusicFolderViewModel(new Models.MusicFolder(path, false));

            if (model.Folders.Contains(newModel)) 
            {
                return;
            }
            newModel.Delete += model.Delete;
            
            model.Folders.Add(newModel);

            using (var connection = new SqliteConnection("Data Source=music.db")) 
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = 
                $"""
                INSERT INTO music (path)
                VALUES ($path)
                """;
                command.Parameters.AddWithValue("$path", path);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}