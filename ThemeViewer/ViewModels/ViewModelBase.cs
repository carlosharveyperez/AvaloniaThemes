using ReactiveUI;

namespace ThemeViewer.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        public ViewModelBase(string name, string image)
        {
            Name = name;
            Image = image;
        }

        public string Name { get; }

        public string Image { get; }
    }
}