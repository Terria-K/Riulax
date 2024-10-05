using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Riulax.Models;
using Riulax.ViewModels;

namespace Riulax;

public class Database 
{
    private const string MigrationNum = "1";
    private const string MusicFolder = "music_folder" + MigrationNum;
    private const string PlaylistSongs = "playlist_songs" + MigrationNum;
    private const string Playlist= "playlist" + MigrationNum;
    private const string Song = "song" + MigrationNum;
    public string DBSource { get; }
    public Database(string dbPath) 
    {
        DBSource = $"Data Source={dbPath}";
    }

    public void InitAllTable() 
    {
        using (var connection = new SqliteConnection(DBSource)) 
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = 
            $"""
            CREATE TABLE if not exists {MusicFolder} (
                id      TEXT NOT NULL PRIMARY KEY, 
                path    TEXT NOT NULL,
                indexed INTEGER 
            );
            """;

            command.ExecuteNonQuery();

            command = connection.CreateCommand();

            command.CommandText = 
            $"""
            CREATE TABLE if not exists {Song} (
                id          TEXT NOT NULL PRIMARY KEY, 
                path	    TEXT NOT NULL,
                title	    TEXT NOT NULL,
                artist	    TEXT,
                date        TEXT,
                artwork_url	TEXT,
                folder_id	TEXT NOT NULL,
                FOREIGN KEY (folder_id) REFERENCES {MusicFolder} (id)
            );
            """;
            command.ExecuteNonQuery();

            command = connection.CreateCommand();

            command.CommandText = 
            $"""
            CREATE TABLE if not {PlaylistSongs} (
                id          TEXT NOT NULL PRIMARY KEY, 
                playlist_id TEXT NOT NULL,
                song_id     TEXT NOT NULL,
                FOREIGN KEY (playlist_id) REFERENCES {Playlist} (id)
                FOREIGN KEY (song_id) REFERENCES {Song} (id)
            );
            """;

            command.ExecuteNonQuery();

            command = connection.CreateCommand();

            command.CommandText = 
            $"""
            CREATE TABLE if not {Playlist} (
                id          TEXT NOT NULL PRIMARY KEY, 
                name        TEXT NOT NULL
            );
            """;

            command.ExecuteNonQuery();
        }
    }
#region Song
    public async Task<SqliteConnection> StartAddSongConnection() 
    {
        var connection = new SqliteConnection(DBSource);
        await connection.OpenAsync();
        return connection;
    }

    public async ValueTask AddSongPrepare(SqliteConnection connection, MusicFolderViewModel folder, SongViewModel song) 
    {
        var command = connection.CreateCommand();
        command.CommandText = $"""
        INSERT INTO {Song}
        VALUES ($id, $path, $title, $artist, $date, $artwork_url, $folder_id)
        """;
        command.Parameters.AddWithValue("$id", song.ID.ToString());
        command.Parameters.AddWithValue("$path", song.Path);
        command.Parameters.AddWithValue("$title", song.Title);
        command.Parameters.AddWithValue("$artist", song.Artist);
        command.Parameters.AddWithValue("$date", song.Date);
        command.Parameters.AddWithValue("$artwork_url", song.ArtworkURL);
        command.Parameters.AddWithValue("$folder_id", folder.ID.ToString());
        await command.ExecuteNonQueryAsync();
    }

    public async ValueTask EndAddSongConnection(SqliteConnection connection) 
    {
        await connection.CloseAsync();
    }

    public async ValueTask AddSong(MusicFolderViewModel folder, SongViewModel song) 
    {
        using (var connection = new SqliteConnection(DBSource)) 
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = $"""
            INSERT INTO {Song}
            VALUES ($id, $path, $title, $artist, $date, $artwork_url, $folder_id)
            """;
            command.Parameters.AddWithValue("$id", song.ID.ToString());
            command.Parameters.AddWithValue("$path", song.Path);
            command.Parameters.AddWithValue("$title", song.Title);
            command.Parameters.AddWithValue("$artist", song.Artist);
            command.Parameters.AddWithValue("$date", song.Date);
            command.Parameters.AddWithValue("$artwork_url", song.ArtworkURL);
            command.Parameters.AddWithValue("$folder_id", folder.ID.ToString());
            await command.ExecuteNonQueryAsync();
        }
    }

    public async ValueTask RemoveSongsFromFolder(MusicFolderViewModel folder) 
    {
        using (var connection = new SqliteConnection(DBSource)) 
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = 
            $"""
            DELETE FROM {Song}
            WHERE folder_id=$folder_id
            """;
            command.Parameters.AddWithValue("$folder_id", folder.ID.ToString());
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task<List<SongViewModel>> GetAllSong() 
    {
        var songs = new List<SongViewModel>();
        using (var connection = new SqliteConnection(DBSource)) 
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = $"""
            SELECT * FROM {Song}
            ORDER BY title ASC
            """;

            using var reader = await command.ExecuteReaderAsync();
            while (reader.Read()) 
            {
                songs.Add(new SongViewModel(
                    new Song(
                        Ulid.Parse(reader.GetString(0)), 
                        reader.GetString(1), 
                        reader.GetString(2), 
                        reader.GetString(3),
                        reader.GetString(4),
                        reader.GetString(5),
                        Ulid.Parse(reader.GetString(6))
                    )
                ));
            }
        }
        return songs;
    }
#endregion

#region MusicFolder
    public async ValueTask IndexFolder(MusicFolderViewModel viewModel) 
    {
        using (var connection = new SqliteConnection(DBSource)) 
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = $"""
            UPDATE {MusicFolder}
            SET indexed = 1
            WHERE id=$id 
            """;
            command.Parameters.AddWithValue("$id", viewModel.ID.ToString());
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task<List<MusicFolderViewModel>> GetAllMusicFolder() 
    {
        List<MusicFolderViewModel> folders = new List<MusicFolderViewModel>();
        using (var connection = new SqliteConnection(DBSource)) 
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = $"""
            SELECT * FROM {MusicFolder}
            """;
            using var reader = await command.ExecuteReaderAsync();
            while (reader.Read()) 
            {
                folders.Add(new MusicFolderViewModel(
                    new MusicFolder(Ulid.Parse(reader.GetString(0)), reader.GetString(1), reader.GetBoolean(2))));
            }
        }
        return folders;
    }

    public async ValueTask AddMusicFolder(MusicFolderViewModel musicFolder) 
    {
        using (var connection = new SqliteConnection(DBSource)) 
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = 
            $"""
            INSERT INTO {MusicFolder} (id, path, indexed)
            VALUES ($id, $path, $indexed)
            """;
            command.Parameters.AddWithValue("$id", musicFolder.ID.ToString());
            command.Parameters.AddWithValue("$path", musicFolder.Path);
            command.Parameters.AddWithValue("$indexed", 0);

            await command.ExecuteNonQueryAsync();
        }
    }

    public async ValueTask<bool> CheckMusicFolder(MusicFolderViewModel musicFolder) 
    {
        using (var connection = new SqliteConnection(DBSource)) 
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = $"""
            SELECT COUNT(1) FROM {MusicFolder}
            WHERE path=$path
            """;
            command.Parameters.AddWithValue("$path", musicFolder.Path);
            int rowCount = Convert.ToInt32(await command.ExecuteScalarAsync());

            return rowCount != 0;
        }
    }

    public async ValueTask RemoveMusicFolder(MusicFolderViewModel musicFolder) 
    {
        using (var connection = new SqliteConnection(DBSource)) 
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = 
            $"""
            DELETE FROM {MusicFolder}
            WHERE path=$path
            """;
            command.Parameters.AddWithValue("$path", musicFolder.Path);

            await command.ExecuteNonQueryAsync();
        }
    }
#endregion
}