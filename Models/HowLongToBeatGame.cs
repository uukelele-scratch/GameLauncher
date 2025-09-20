using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace GameLauncher.Models
{
    public class HowLongToBeatGame
    {
        [JsonProperty("game_image")]
        public string? GameImage { get; set; }
    }
}
