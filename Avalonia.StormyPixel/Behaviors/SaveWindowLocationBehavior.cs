using Avalonia.Controls;
using System.Drawing;
using System.Text.Json;

namespace Avalonia.StormyPixel.Behaviors;

/// <summary>
/// This behavior saves the Left, Top, Width and Height properties of a Window and restore
/// these properties before the window is displayed.
/// There is currently no support for multiple monitors or reposition a window
/// that is out of bounds of the physical screens.
/// </summary>
public class SaveWindowLocationBehavior
{
    public static bool GetIsSet(Window window) => window.GetValue(IsSetProperty);

    public static void SetIsSet(Window window, bool value) => window.SetValue(IsSetProperty, value);

    public static readonly AttachedProperty<bool> IsSetProperty =
        AvaloniaProperty.RegisterAttached<SaveWindowLocationBehavior, Window, bool>("IsSet");

    static SaveWindowLocationBehavior()
    {
        IsSetProperty.Changed.Subscribe(OnIsSetChanged);
    }

    private static void OnIsSetChanged(AvaloniaPropertyChangedEventArgs<bool> args)
    {
        Window w = (Window)args.Sender;
        bool value = args.NewValue.Value;

        if (value)
        {
            ApplyWindowLocation(w);
            w.Closing += Window_Closing;
        }
        else
        {
            w.Closing -= Window_Closing;
        }
    }

    private static void Window_Closing(object? sender, WindowClosingEventArgs e)
    {
        if (sender == null) return;
        Window w = (Window)sender;
        if (!IsSavingSupported(w)) return;

        var fileName = GetFileName(w);
        var fileInfo = new FileInfo(fileName);
        if (fileInfo.Exists) File.Delete(fileName);

        var screenPos = w.PointToScreen(w.Bounds.TopLeft);
        var bounds = new Rectangle(screenPos.X, screenPos.Y, (int)w.Width, (int)w.Height);
        string jsonString = JsonSerializer.Serialize(bounds, typeof(Rectangle));
        File.WriteAllText(fileName, jsonString);
    }

    // We apply the saved settings before the window is displayed to avoid 
    // the jarring effect of moving the window after it was displayed.
    private static void ApplyWindowLocation(Window w)
    {
        if (!IsSavingSupported(w)) return;

        var fileName = GetFileName(w);
        var fileInfo = new FileInfo(fileName);
        if (!fileInfo.Exists) return;

        string jsonString = File.ReadAllText(fileName);
        var bounds = JsonSerializer.Deserialize<Rectangle>(jsonString);
        w.Position = PixelPoint.FromPoint(new Point(bounds.Left, bounds.Top), 1);
        w.Width = bounds.Width;
        w.Height = bounds.Height;
    }

    private static bool IsSavingSupported(Window w) => w is { WindowStartupLocation: WindowStartupLocation.Manual, WindowState: WindowState.Normal };

    private static string GetFileName(Window w)
    {
        var windowName = !string.IsNullOrEmpty(w.Name) ? w.Name : w.GetType().FullName;
        var fileName = $"{windowName}.json";
        return fileName;
    }
}