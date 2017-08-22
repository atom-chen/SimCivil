﻿using Newtonsoft.Json;
using static SimCivil.Config;

namespace SimCivil.Map
{
    /// <summary>
    /// Class used to store part of map.
    /// </summary>
    public class Atlas
    {
        /// <summary>
        /// Atlas' all tiles.
        /// </summary>
        public Tile[,] Tiles { get; private set; }
        /// <summary>
        /// Atlas' X position.
        /// </summary>
        public int X { get; private set; }
        /// <summary>
        /// Atlas' Y position.
        /// </summary>
        public int Y { get; private set; }
        /// <summary>
        /// Atlas' Width.
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// Atlas' Height.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The minimum X position of it's tiles.
        /// </summary>
        [JsonIgnore]
        public int Left { get { return X; } }
        /// <summary>
        /// The maximum X position of it's tiles.
        /// </summary>
        [JsonIgnore]
        public int Right { get { return X + Width; } }
        /// <summary>
        /// The minimum Y position of it's tiles.
        /// </summary>
        [JsonIgnore]
        public int Top { get { return Y; } }
        /// <summary>
        /// The maximum Y position of it's tiles.
        /// </summary>
        [JsonIgnore]
        public int Bottom { get { return Y + Height; } }

        /// <summary>
        /// Whether it's tiles have been created.
        /// </summary>
        [JsonIgnore]
        public bool IsExsist { get; private set; } = false;
        /// <summary>
        /// Whether it's tiles have been loaded.
        /// </summary>
        [JsonIgnore]
        public bool HasLoaded { get; private set; } = false;

        /// <summary>
        /// Method to craete a new empty Atlas.
        /// </summary>
        /// <param name="x">Atlas' X position.</param>
        /// <param name="y">Atlas' Y position.</param>
        /// <param name="width">Atlas' Width.</param>
        /// <param name="height">Atlas' Height.</param>
        /// <returns></returns>
        public static Atlas Create(int x, int y, int width = DefaultAtlasWidth, int height = DefaultAtlasHeight)
        {
            return new Atlas()
            {
                Tiles = new Tile[width, height],
                X = x,
                Y = y,
                Width = width,
                Height = height,
                IsExsist = true,
                HasLoaded = true,
            };
        }
    }
}