using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using Riulax.ViewModels;
using ReactiveUI;
using Avalonia.Input;

namespace Riulax.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    private bool moving;
    private PointerPoint point;

    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(async action => {

            action(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync));
            await ViewModel!.Start();
        });
        ControlPane.PointerEntered += EnterHoveredPane;
        ControlPane.PointerExited += ExitedHoveredPane;

        WindowTitle.PointerPressed += PressedWindow;
        WindowTitle.PointerReleased += ReleasedWindow;
        WindowTitle.PointerMoved += MovingWindow;
    }

    private void MovingWindow(object? sender, PointerEventArgs e)
    {
        if (!moving) { return; }
        var currentPoint = e.GetCurrentPoint(this);
        Position = new Avalonia.PixelPoint(
            Position.X + (int)(currentPoint.Position.X - point.Position.X),
            Position.Y + (int)(currentPoint.Position.Y - point.Position.Y));
        
    }

    private void PressedWindow(object? sender, PointerPressedEventArgs e)
    {
        moving = true;
        point = e.GetCurrentPoint(this);
    }

    private void ReleasedWindow(object? sender, PointerReleasedEventArgs e)
    {
        moving = false;
    }

    public void EnterHoveredPane(object? sender, PointerEventArgs args) 
    {
        Pane.IsPaneOpen = true;
    }

    public void ExitedHoveredPane(object? sender, PointerEventArgs args) 
    {
        Pane.IsPaneOpen = false;
    }


    private async Task DoShowDialogAsync(InteractionContext<MusicImporterViewModel, ICollection<MusicFolderViewModel>?> interaction) 
    {
        var dialog = new MusicImporterWindow();

        var folders = await AppState.Database.GetAllMusicFolder();

        foreach (var folder in folders) 
        {
            interaction.Input.Folders.Add(folder);
            folder.Delete += interaction.Input.Delete;
        }

        dialog.DataContext = interaction.Input;

        var result = await dialog.ShowDialog<MusicImporterViewModel?>(this);
        interaction.SetOutput(interaction.Input.Folders);
    }
}