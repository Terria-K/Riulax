using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Riulax.ViewModels;
using Microsoft.Data.Sqlite;
using ReactiveUI;

namespace Riulax.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(async action => {

            action(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync));
            await ViewModel!.Start();
        });
    }

    public void PlaySong(object? sender, SelectionChangedEventArgs args) 
    {
        var model = (MainWindowViewModel)ViewModel!;
        model.PlaySong();
    }

    private async Task DoShowDialogAsync(InteractionContext<MusicImporterViewModel, ICollection<MusicFolderViewModel>?> interaction) 
    {
        var dialog = new MusicImporterWindow();

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
                var musicFolder = new MusicFolderViewModel(new Models.MusicFolder(reader.GetString(0), false));
                interaction.Input.Folders.Add(musicFolder);

                musicFolder.Delete += interaction.Input.Delete;
            }
        }

        dialog.DataContext = interaction.Input;

        var result = await dialog.ShowDialog<MusicImporterViewModel?>(this);
        interaction.SetOutput(interaction.Input.Folders);
    }
}