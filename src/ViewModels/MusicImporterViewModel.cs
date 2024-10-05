using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Riulax.ViewModels;

public partial class MusicImporterViewModel : ViewModelBase 
{
    public ObservableCollection<MusicFolderViewModel> Folders { get; } = new();

    public MusicImporterViewModel() 
    {
    }

    public async Task Delete(MusicFolderViewModel model) 
    {
        await AppState.Database.RemoveSongsFromFolder(model);
        await AppState.Database.RemoveMusicFolder(model);
        Folders.Remove(model);
    }
}
