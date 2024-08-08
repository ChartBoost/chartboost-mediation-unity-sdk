using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Chartboost.Mediation.Ad.Banner
{
    /// <summary>
    /// Serializable information of layout parameters for UI elements.
    /// </summary>
    [Serializable]
    public class LayoutParams
    {
        public float x;
        public float y;
        public float width;
        public float height;
        
        [JsonIgnore]public Vector2 topLeft;
        [JsonIgnore]public Vector2 bottomLeft;
        [JsonIgnore]public Vector2 topRight;
        [JsonIgnore]public Vector2 bottomRight;
    }
}
