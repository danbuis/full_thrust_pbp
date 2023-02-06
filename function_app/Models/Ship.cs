using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Functions.Models;

internal class Ship
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "player")]
    public string Player { get; set; }

    [JsonProperty(PropertyName = "currentSpeed")]
    public int CurrentSpeed { get; set; }

    [JsonProperty(PropertyName = "currentBearing")]
    public int CurrentBearing { get; set; }

    [JsonProperty(PropertyName = "x")]
    public float X { get; set; } 

    [JsonProperty(PropertyName = "y")]
    public float Y { get; set; }

    [JsonProperty(PropertyName = "speedChange")]
    public int SpeedChange { get; set; }

    [JsonProperty(PropertyName = "bearingChange")]
    public int BearingChange { get; set; }

    [JsonProperty(PropertyName = "NavigationOrders")]
    public int[] NavigationOrders { get; set; }

    [JsonProperty(PropertyName = "dateCreated")]
    public DateTime DateCreated { get; set; }
}
