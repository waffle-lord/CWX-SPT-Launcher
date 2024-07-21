namespace CWX_SPT_Frontend.Models;

public class AppSettingsClass
{
    public StartLocationClass StartLocation { get; set; }
    public StartSizeClass StartSize { get; set; }
    
    // left panel options
    public bool CloseToTray { get; set; }
    public bool MinimizeOnLaunch { get; set; }
    public bool AlwaysTop { get; set; }
    
    // Advanced panel options
    public bool AdvancedUser { get; set; }
    public string? SptPath { get; set; }
}