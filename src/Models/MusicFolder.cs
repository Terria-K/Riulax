namespace Riulax.Models;

public class MusicFolder(string FolderPath, bool HasIndexed) 
{
    public string FolderPath { get; init; } = FolderPath;
    public bool HasIndexed { get; set; } = HasIndexed;
}