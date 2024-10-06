using Avalonia.Data.Converters;
using System;

namespace Riulax;

public static class RiulaxConverter 
{
    public static FuncValueConverter<float, string> IntToTimeString { get; } =
        new FuncValueConverter<float, string>(i => {
            return TimeSpan.FromMilliseconds(i).ToString(@"mm\:ss");
        });
    
    public static FuncValueConverter<string, bool> ValidInput { get; } =
        new FuncValueConverter<string, bool>(i => {
            if (i is null) 
            {
                return false;
            }
            if (string.IsNullOrEmpty(i.Trim())) 
            {
                return false;
            }
            return true;
        });
}
