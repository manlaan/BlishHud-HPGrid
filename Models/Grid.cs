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
        [JsonPropertyName("position")]
        public List<float> Position { get; set; }
        [JsonPropertyName("radius")]
        public float Radius { get; set; }
        [JsonPropertyName("phase")]
        public List<GridPhase> Phase { get; set; }

        public bool InRadius(Vector3 point)
        {
            float distance = (new Vector3 (Position[0], Position[1], Position[2]) - point).Length();
            if (distance <= Radius)
                return true;
            else
                return false;
        }
    }

    public class GridPhase
    {
        [JsonPropertyName("percent")]
        public int Percent { get; set; }
        [JsonPropertyName("color")]
        public string Color { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
