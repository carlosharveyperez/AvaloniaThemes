using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using System.Drawing;
using System.Text.Json;

namespace Avalonia.StormyPixel.Behaviors;

/// <summary>
/// This behavior saves the Left, Top, Width and Height properties of a Window and restore
/// these properties before the window is displayed.
/// There is currently no support for multiple monitors or repositioning a window
/// that is out of bounds of the physical screens.
/// </summary>
public class SaveWindowLocationBehavior : Behavior<Window>
{
    protected override void OnAttached()
    {
        if (AssociatedObject is { })
        {
            AssociatedObject.Closing += Window_Closing;
            ApplyWindowLocation(AssociatedObject);
        }
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject is { })
        {
            AssociatedObject.Closing -= Window_Closing;
        }
    }

    private void Window_Closing(object? sender, WindowClosingEventArgs e)
    {
        if (sender == null) return;
        Window w = (Window)sender;
        if (!IsSavingSupported(w)) return;

        var fileName = GetFileName(w);
        if (IsValidFileAssociated(w))
            File.Delete(fileName);

        var screenPos = w.PointToScreen(w.Bounds.TopLeft);
        var bounds = new Rectangle(screenPos.X, screenPos.Y, (int)w.Width, (int)w.Height);
        string jsonString = JsonSerializer.Serialize(bounds, typeof(Rectangle));
        File.WriteAllText(fileName, jsonString);
    }

    private void ApplyWindowLocation(Window w)
    {
        if (!IsSavingSupported(w)) return;
        if (!IsValidFileAssociated(w)) return;

        var fileName = GetFileName(w);
        string jsonString = File.ReadAllText(fileName);
        var bounds = JsonSerializer.Deserialize<Rectangle>(jsonString);
        w.Position = PixelPoint.FromPoint(new Point(bounds.Left, bounds.Top), 1);
        w.Width = bounds.Width;
        w.Height = bounds.Height;
    }

    private bool IsValidFileAssociated(Window w)
    {
        var fileName = GetFileName(w);
        var fileInfo = new FileInfo(fileName);
        return fileInfo.Exists;
    }

    private bool IsSavingSupported(Window w)
    {
        return w is { WindowStartupLocation: WindowStartupLocation.Manual, WindowState: WindowState.Normal };
    }

    private string GetFileName(Window w)
    {
        var windowName = !string.IsNullOrEmpty(w.Name) ? w.Name : w.GetType().FullName;
        var fileName = $"{windowName}.json";
        return fileName;
    }
}