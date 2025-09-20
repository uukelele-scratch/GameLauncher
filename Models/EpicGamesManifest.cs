using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace GameLauncher.Models
{
    public class EpicGamesManifest
    {
        [JsonProperty("AppName")]
        public string? AppName { get; set; }
        [JsonProperty("DisplayName")]
        public string? DisplayName { get; set; }
        [JsonProperty("InstallLocation")]
        public string? InstallLocation { get; set; }
        [JsonProperty("LaunchExecutable")]
        public string? LaunchExecutable { get; set; }
        [JsonProperty("LaunchCommand")]
        public string? LaunchCommand { get; set; }
    }
}
