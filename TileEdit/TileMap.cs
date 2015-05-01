using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEdit.Models;

namespace TileEdit
{
    public class TileMap
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public IList<Sprite> Tiles { get; private set; }

        public TileMap(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void AddTiles(IList<Sprite> tiles)
        {
            Tiles = tiles;
        }
    }
}
