using System;

namespace Riulax.Models;

public record Song(Ulid ID, string Path, string Title, string Artist, string Date, string ArtworkUrl, Ulid FolderID);
public record Playlist(Ulid ID, string Name);