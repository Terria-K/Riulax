using System;

namespace Riulax.Models;

public class MusicFolder(Ulid id, string folderPath, bool hasIndexed) 
{
    public Ulid ID { get; init; } = id;
    public string FolderPath { get; init; } = folderPath;
    public bool HasIndexed { get; set; } = hasIndexed;
}