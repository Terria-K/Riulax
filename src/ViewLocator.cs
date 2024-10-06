using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ReactiveUI;
using Riulax.ViewModels;
using Riulax.Views;

namespace Riulax;

public class AppViewLocator : ReactiveUI.IViewLocator
{
    public IViewFor? ResolveView<T>(T? viewModel, string? contract = null) 
    {
        if (viewModel is MainWindowViewModel model) 
        {
            return model.ViewRoute switch 
            {
                MainWindowViewModel.Route.Home => new SongListView() { DataContext = model },
                MainWindowViewModel.Route.Songs => new SongListView() { DataContext = model },
                MainWindowViewModel.Route.Playlist => new PlaylistListView() { DataContext = model },
                _ => null
            };
        }
        return null;
    }
}

public class ViewLocator : IDataTemplate
{
    private static Dictionary<Type, Func<Control>> Registration = new Dictionary<Type, Func<Control>>();
    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        var type = data.GetType();
        
        if (Registration.TryGetValue(type, out var factory)) 
        {
            return factory();
        }
        else 
        {
            return new TextBlock { Text = "Not Found: " + type };
        }
        
    }

    public static void Register<TViewModel, TView>() where TView : Control, new() 
    {
        Registration.Add(typeof(TViewModel), () => new TView());
    }

    public static void Register<TViewModel, TView>(Func<TView> factory) where TView : Control, new() 
    {
        Registration.Add(typeof(TViewModel), factory);
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
