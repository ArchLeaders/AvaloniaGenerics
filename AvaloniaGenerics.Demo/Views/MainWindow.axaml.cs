using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaGenerics.Demo.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
