﻿using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Data.Sqlite;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using Projektanker.Icons.Avalonia.MaterialDesign;
using System;

namespace AvaloniaTest;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) 
    {
        using (var connection = new SqliteConnection("Data Source=music.db")) 
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = 
            """
            CREATE TABLE if not exists music (
                id      INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
                path    TEXT NOT NULL
            );
            """;

            command.ExecuteNonQuery();
        }
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp() 
    {
        IconProvider.Current.Register<FontAwesomeIconProvider>().Register<MaterialDesignIconProvider>();

        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
    }
}
