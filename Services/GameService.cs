using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using GameLauncher.ViewModels;
using GameLauncher.Models;

using Newtonsoft.Json;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace GameLauncher.Services
{
    public class GameService
    {
        public static string ToAscii(string input)
        {
            var sb = new StringBuilder(input.Length);
            foreach (char c in input)
            {
                if (c <= 127) // ASCII
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public async Task<HowLongToBeatGame?> FetchGameInfo(string gameName)
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Origin", "https://howlongtobeat.com/");
            client.DefaultRequestHeaders.Add("Referer", "https://howlongtobeat.com/");
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                "(KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36 OPR/121.0.0.0");

            var url = "https://howlongtobeat.com/api/seek/28b235595e8e894c";


            var json = @"{
                ""searchType"": ""games"",
                ""searchTerms"": [""" + ToAscii(gameName) + @"""],
                ""searchPage"": 1,
                ""size"": 20,
                ""searchOptions"": {
                    ""games"": {
                        ""userId"": 0,
                        ""platform"": """",
                        ""sortCategory"": ""popular"",
                        ""rangeCategory"": ""main"",
                        ""rangeTime"": { ""min"": null, ""max"": null },
                        ""gameplay"": {
                            ""perspective"": """",
                            ""flow"": """",
                            ""genre"": """",
                            ""difficulty"": """"
                        },
                        ""rangeYear"": { ""min"": """", ""max"": """" },
                        ""modifier"": """"
                    },
                    ""users"": { ""sortCategory"": ""postcount"" },
                    ""lists"": { ""sortCategory"": ""follows"" },
                    ""filter"": """",
                    ""sort"": 0,
                    ""randomizer"": 0
                },
                ""useCache"": true
            }";

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            JArray data = JObject.Parse(result)["data"] as JArray;
            if (data.Count > 0)
            {
                return JsonConvert.DeserializeObject<HowLongToBeatGame>(data.First?.ToString());
            }
            else return null;
        }

        private List<GameViewModel> _gamesCache;
        private bool _isCacheFilled = false;

        public async Task<List<GameViewModel>> SearchGames(string query)
        {
            var games = await LoadGamesAsync();
            return games
                .Where(g => g.Name != null && g.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }

        public async Task<List<GameViewModel>> LoadGamesAsync()
        {
            if (_isCacheFilled)
            {
                return _gamesCache;
            }

            List<GameViewModel> games = new List<GameViewModel>();

            try
            {
                // Epic Games
                var epicGamesPath = @"C:\ProgramData\Epic\EpicGamesLauncher\Data\Manifests";
                if (Directory.Exists(epicGamesPath))
                {
                    // check each .item file in Epic Games directory
                    var itemFiles = Directory.GetFiles(epicGamesPath, "*.item");
                    foreach (var item in itemFiles)
                    {
                        // read the item file and JSON parse
                        var json = await File.ReadAllTextAsync(item);
                        var manifest = JsonConvert.DeserializeObject<EpicGamesManifest>(json);
                        
                        games.Add(new GameViewModel
                        {
                            Name = manifest?.DisplayName ?? "Unknown",
                            LaunchPath = $"com.epicgames.launcher://apps/{manifest.AppName}?action=launch&silent=true",
                            LaunchArgs = manifest?.LaunchCommand ?? ""
                        });
                    }
                }

                // add other providers later

                var tasks = games.Select(async g =>
                {
                    var gameInfo = await FetchGameInfo(g.Name);
                    g.ImagePath = gameInfo?.GameImage != null
                        ? $"https://howlongtobeat.com/games/{gameInfo.GameImage}?width=200&height=300"
                        : "https://placehold.co/200x300.png?text=No+Image&font=Oswald";
                });

                await Task.WhenAll(tasks);


            } catch (Exception ex)
            {
                Console.WriteLine($"Error loading games: {ex}");
            }

            _isCacheFilled = true;
            _gamesCache = games;
            return games;
        }
    }
}
