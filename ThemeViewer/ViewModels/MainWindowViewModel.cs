using Avalonia;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ThemeViewer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            this.WhenAnyValue(x => x.SelectedTheme)
                .Subscribe(UpdateTheme);

            this.WhenAnyValue(x => x.IsCompact)
                .Subscribe(UpdateDensity);

            _selectedTheme = Themes[0];
        }

        public ObservableCollection<string> Themes => new() { "Fluent Dark", "Fluent Light" };

        private string _selectedTheme;
        public string SelectedTheme
        {
            get => _selectedTheme;
            set => this.RaiseAndSetIfChanged(ref _selectedTheme, value);
        }

        private bool _isCompact = true;
        public bool IsCompact
        {
            get => _isCompact;
            set => this.RaiseAndSetIfChanged(ref _isCompact, value);
        }

        private void UpdateTheme(string theme)
        {
            if (Application.Current == null) return;
            Application.Current.RequestedThemeVariant = theme == Themes[0] ? ThemeVariant.Dark : ThemeVariant.Light;
        }

        private void UpdateDensity(bool isCompact)
        {
            if (Application.Current == null) return;
            var fluentTheme = Application.Current.Styles.FirstOrDefault(s => s is FluentTheme);
            if (fluentTheme is FluentTheme ft)
                ft.DensityStyle = IsCompact ? DensityStyle.Compact : DensityStyle.Normal;
        }
    }
}