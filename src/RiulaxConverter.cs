using Avalonia.Data.Converters;
using System;

namespace Riulax;

public static class RiulaxConverter 
{
    public static FuncValueConverter<float, string> IntToTimeString { get; } =
        new FuncValueConverter<float, string>(i => {
            return TimeSpan.FromMilliseconds(i).ToString(@"mm\:ss");
        });
}
