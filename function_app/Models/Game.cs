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

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    //<player, password>
    [JsonProperty(PropertyName = "players")]
    public Dictionary<string, string> Players { get; set; }

    //<player, List<id>>
    [JsonProperty(PropertyName = "ships")]
    public Dictionary<string, List<string>> Ships { get; set; }

    [JsonProperty(PropertyName = "dateCreated")]
    public DateTime DateCreated { get; set; }
}
