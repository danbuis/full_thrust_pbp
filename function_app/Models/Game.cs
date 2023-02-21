using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Functions.Models;

internal class Game
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "phase")]
    public string Phase { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    //<player, password>
    [JsonProperty(PropertyName = "players")]
    public Dictionary<string, string> Players { get; set; }

    //<player, List<id>>
    [JsonProperty(PropertyName = "ships")]
    public Dictionary<string, List<string>> Ships { get; set; }

    //<player, List<id>>
    [JsonProperty(PropertyName = "launchedItesm")]
    public Dictionary<string, List<string>> LaunchedItems { get; set; }

    [JsonProperty(PropertyName = "dateCreated")]
    public DateTime DateCreated { get; set; }

    [JsonProperty(PropertyName = "playerSignoffs")]
    public List<string> PlayerSignoffs { get; set; }

    [JsonProperty(PropertyName = "playerCount")]
    public int PlayerCount { get; set; }

    [JsonProperty(PropertyName = "GameDimensions")]
    public double[] Dimensions { get; set; }

    [JsonProperty(PropertyName = "PlanetLocation")]
    public double[]? PlanetLocation { get; set; }

    [JsonProperty(PropertyName = "GateLocation")]
    public double[]? GateLocation { get; set; }

    [JsonProperty(PropertyName = "GateBearing")]
    public int? GateBearing { get; set; }

    [JsonProperty(PropertyName = "Log")]
    public string[] Log { get; set; }
}
