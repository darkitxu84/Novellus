using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;
using NovellusLib;
using NovellusGUI.Models;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.Linq;

namespace NovellusGUI.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        public ObservableCollection<GameItem> AvaibleGames { get; }
        private GameItem? _selectedGame;
        public GameItem? SelectedGame
        {
            get => _selectedGame;
            set => this.RaiseAndSetIfChanged(ref _selectedGame, value);
        }

        public MainWindowViewModel()
        {
            AvaibleGames = [];

            foreach (var game in Enum.GetValues<Game>().Skip(1))
            {
                var uri = new Uri($"avares://NovellusGUI/Assets/Icons/{game.Folder()}.png");
                if (!AssetLoader.Exists(uri))
                    uri = new Uri("avares://NovellusGUI/Assets/Icons/unknown.png");

                AvaibleGames.Add(new GameItem
                {
                    Game = game,
                    DisplayName = game.Name(),
                    Icon = new Bitmap(AssetLoader.Open(uri))
                });
            }
        }
    }
}
