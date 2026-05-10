// we don't need this class on the lib, move this to the GUI
namespace NovellusLib.Configuration
{
    public class NovellusConfig
    {
        public bool CheckForUpdatesOnStartup { get; set; } = true;
        public bool DarkMode { get; set; } = true;
    }
}
