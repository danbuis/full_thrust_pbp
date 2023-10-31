using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Blazor.Models;

public class Ship
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

    [JsonProperty(PropertyName = "NewSpeed")]
    public int NewSpeed { get; set; }

    [JsonProperty(PropertyName = "NewBearing")]
    public int NewBearing { get; set; }

    [JsonProperty(PropertyName = "dateCreated")]
    public DateTime DateCreated { get; set; }

    public (float, float) GetNextCoordinates()
    {
        int NewCurrentSpeed = CurrentSpeed + SpeedChange;
        int NewCurrentBearing = (CurrentBearing + BearingChange) % 12;

        int deltaBearing = NewCurrentBearing - CurrentBearing;
        int mult = deltaBearing > 0 ? 1 : -1;
        int firstBearing = CurrentBearing + mult * (int)Math.Floor(Math.Abs(deltaBearing) / 2.0);
        int secondBearing = firstBearing + mult* (int)Math.Ceiling(Math.Abs(deltaBearing) / 2.0);

        int firstMove = (int)Math.Floor(NewCurrentSpeed / 2.0);
        int secondMove = NewCurrentSpeed - firstMove;

        float intermediateX = X + (float)Math.Sin(firstBearing * Math.PI / 6) * firstMove;
        float intermediateY = Y + (float)Math.Cos(firstBearing * Math.PI / 6) * firstMove;

        float newX = intermediateX + (float)Math.Sin(secondBearing * Math.PI / 6) * secondMove;
        float newY = intermediateY + (float)Math.Cos(secondBearing * Math.PI / 6) * secondMove;
        return (newX, newY);
    }
}
