using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Riulax.ViewModels;

namespace Riulax;

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
