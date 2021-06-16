using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HPGrid.Models
{
    public class Grid
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("map")]
        public int Map { get; set; }
        [JsonPropertyName("fights")]
        public List<GridFight> Fights { get; set; }
    }

    public class GridFight
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("coord")]
        public List<float> Coord { get; set; }
        [JsonPropertyName("radius")]
        public float Radius { get; set; }
        [JsonPropertyName("lines")]
        public List<GridLine> Lines { get; set; }

        public bool InRadius(Vector3 point)
        {
            float distance = (new Vector3 (Coord[0], Coord[1], Coord[2]) - point).Length();
            if (distance <= Radius)
                return true;
            else
                return false;
        }
    }

    public class GridLine
    {
        [JsonPropertyName("pct")]
        public int pct { get; set; }
        [JsonPropertyName("color")]
        public string color { get; set; }
        [JsonPropertyName("description")]
        public string description { get; set; }
    }
}
