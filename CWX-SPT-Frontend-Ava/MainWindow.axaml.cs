using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CWX_SPT_Ava;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
#if DEBUG
        this.AttachDevTools();
#endif  
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private void OnCloseButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }
}