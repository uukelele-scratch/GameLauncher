using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;

namespace GameLauncher.ViewModels
{
    public class GameViewModel
    {
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("imagePath")]
        public string? ImagePath { get; set; } = "https://placehold.co/200x300.png?text=No+Image&font=Oswald";
        [JsonProperty("launchPath")]
        public string? LaunchPath { get; set; }
        [JsonProperty("launchArgs")]
        public string? LaunchArgs { get; set; }
    }
}
