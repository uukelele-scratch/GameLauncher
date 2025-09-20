using GameLauncher.Services;
using GameLauncher.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GameLauncher.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameView : Page
    {
        public ObservableCollection<GameViewModel> Games { get; } = new ObservableCollection<GameViewModel>();

        private GameService _gameService;

        public GameView()
        {
            InitializeComponent();
            this.Loaded += GameView_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _gameService = e.Parameter as GameService;
        }

        private async void GameView_Loaded(object sender, RoutedEventArgs e)
        {
            loadingIndicator.IsActive = true;
            var games = await _gameService.LoadGamesAsync();

            Games.Clear();

            foreach (var game in games)
            {
                Games.Add(game);
            }
            loadingIndicator.IsActive = false;
        }

        private void gamesGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedGame = e.ClickedItem as GameViewModel;
            if (selectedGame != null)
            {
                try
                {
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = selectedGame.LaunchPath,
                        Arguments = selectedGame.LaunchArgs,
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., log the error, show a message to the user, etc.)
                    Console.WriteLine($"Error launching game: {ex.Message}");
                    new ContentDialog
                    {
                        Title = "Error",
                        Content = $"Could not launch {selectedGame.Name}. Please check the launch path and try again.\n\nDetails: {ex.Message}",
                    }.ShowAsync();
                }
            }
        }
    }
}
