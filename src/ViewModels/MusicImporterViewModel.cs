using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Riulax.ViewModels;

public partial class MusicImporterViewModel : ViewModelBase 
{
    public ObservableCollection<MusicFolderViewModel> Folders { get; } = new();

    public MusicImporterViewModel() 
    {
    }

    public async Task Delete(MusicFolderViewModel model) 
    {
        using (var connection = new SqliteConnection("Data Source=music.db")) 
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = 
            """
            DELETE FROM music
            WHERE path=$path
            """;
            command.Parameters.AddWithValue("$path", model.Path);

            await command.ExecuteNonQueryAsync();
        }
        Folders.Remove(model);
    }
}
