using Avalonia;
using Avalonia.Styling;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace ThemeViewer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            this.WhenAnyValue(x => x.SelectedTheme)
                .Subscribe(UpdateTheme);
        }

        public ObservableCollection<string> Themes => new() { "Dark", "Light" };

        private string _selectedTheme = "Dark";
        public string SelectedTheme
        {
            get => _selectedTheme;
            set => this.RaiseAndSetIfChanged(ref _selectedTheme, value);
        }

        private void UpdateTheme(string theme)
        {
            if (Application.Current == null) return;
            Application.Current.RequestedThemeVariant = theme == "Dark" ? ThemeVariant.Dark : ThemeVariant.Light;
        }
    }
}