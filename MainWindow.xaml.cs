using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

using GameLauncher.Views;
using GameLauncher.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GameLauncher
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public GameService gameService = new GameService();

        public MainWindow()
        {
            InitializeComponent();
            contentFrame.Navigate(typeof(GameView), gameService);
            nvSample.SelectedItem = nvSample.MenuItems[0];

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            string iconPath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets/gamelauncher-logo.ico");
            appWindow.SetIcon(iconPath);
        }

        private void nvSample_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer != null)
            {
                // Get the page type from the Tag property
                var container = args.InvokedItemContainer;
                if (container.Tag == null) return;
                var navItemTag = container.Tag.ToString();
                if (string.IsNullOrEmpty(navItemTag)) return;

                Type pageType = Type.GetType(navItemTag);
                if (pageType != null && contentFrame.CurrentSourcePageType != pageType)
                {
                    if (pageType == typeof(GameView))
                    {
                        contentFrame.Navigate(pageType, gameService);
                    }
                    else
                    {
                        contentFrame.Navigate(pageType);
                    }
                }
            }
        }

        private void searchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            Debug.WriteLine($"Search: {args.QueryText}");
            // search doesn't actually do anything (for now)
        }

        private void searchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            string text = sender.Text;
            var filtered = gameService.SearchGames(text).Result;
            sender.ItemsSource = filtered.Select(g => g.Name).ToList();
        }
    }
}
