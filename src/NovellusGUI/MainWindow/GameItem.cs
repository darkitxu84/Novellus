using Avalonia.Media.Imaging;
using NovellusLib;

namespace NovellusGUI.Models;
public class GameItem
{
    public Game Game { get; set; }
    public string DisplayName { get; set; }
    public Bitmap Icon { get; set; }
}

