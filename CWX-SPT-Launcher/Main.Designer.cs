using CWX_SPT_Launcher.Models;

namespace CWX_SPT_Launcher;

partial class Main
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.NotifyIcon notifyIcon = null;
    private int widthOffset = 16;
    private int heightOffset = 39;
    
    protected override void Dispose(bool disposing)
    {
        _settingsHelper.SetClientLocationSettings(Location.X, Location.Y);
        _settingsHelper.SetClientSizeSettings(Height, Width);
        _settingsHelper.SetFirstRun(false);
        _settingsHelper.SaveSettings();
        
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }
    
    private void InitializeComponent(SettingsClass settings)
    {
        SuspendLayout();

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        MinimumSize = new Size(640, 450);
			
        if (settings.FirstRun)
        {
            ClientSize = new Size() { Width = 792 - widthOffset, Height = 500 - heightOffset };
            StartPosition = FormStartPosition.CenterScreen;
        }
        else
        {
            ClientSize = new Size()
            {
                Width = settings.AppSettings.StartSize.Width - widthOffset,
                Height = settings.AppSettings.StartSize.Height - heightOffset
            };

            StartPosition = FormStartPosition.Manual;
				
            Location = new Point()
            {
                X = settings.AppSettings.StartLocation.X,
                Y = settings.AppSettings.StartLocation.Y
            };
        }
           
        ResumeLayout(false);
    }
}