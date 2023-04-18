using ReactiveUI;

namespace ThemeViewer.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        public ViewModelBase(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public string Image { get; set; }
    }
}