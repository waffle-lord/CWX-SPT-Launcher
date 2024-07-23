namespace CWX_SPT_Backend.CWX;

public class AppSettings
{
    public StartLocation StartLocation { get; set; }
    public StartSize StartSize { get; set; }
    
    // left panel options
    public bool CloseToTray { get; set; }
    public bool MinimizeOnLaunch { get; set; }
    public bool AlwaysTop { get; set; }
    public bool UseProfileColors { get; set; }
    
    // Advanced panel options
    public bool AdvancedUser { get; set; }
    public string SptPath { get; set; }
}