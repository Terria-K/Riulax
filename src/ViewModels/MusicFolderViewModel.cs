using System;
using System.Threading.Tasks;
using AvaloniaTest.Models;

namespace AvaloniaTest.ViewModels;

public partial class MusicFolderViewModel : ViewModelBase 
{
    private MusicFolder folder;

    public event Func<MusicFolderViewModel, Task>? Delete;

    public bool HasIndexed => folder.HasIndexed;
    public string Path => folder.FolderPath;


    public MusicFolderViewModel(MusicFolder folder) 
    {
        this.folder = folder;
    }

    public void IndexMusic() 
    {
        folder.HasIndexed = true;
    }

    public async void DeleteFolder(object msg) 
    {
        await Delete?.Invoke(this)!;
    }
}