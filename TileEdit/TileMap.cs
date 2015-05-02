using Gengine.Map;
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
        public IList<Layer> Layers { get; private set; }

        public TileMap(int width, int height)
        {
            Width = width;
            Height = height;
            Layers = new List<Layer>();
        }

        public void AddLayer(Layer layer)
        {
            Layers.Add(layer);
        }

        public void AddSprite(string layerName, Sprite sprite)
        {
            Layer layer = Layers.First(l => l.Name == layerName);
            layer.Tiles.Add(sprite);
        }
    }
}
