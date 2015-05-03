using Gengine.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEdit.Models;

namespace TileEdit {
    public class TileMap : Gengine.Map.TileMap {
        public TileMap(int width, int height) : base(width, height) {}

        public void AddLayer(Layer layer) {
            Layers.Add(layer);
        }

        public void AddSprite(string layerName, Sprite sprite) {
            Layer layer = Layers.First(l => l.Name == layerName);
            layer.Tiles.Add(sprite);
        }
    }
}
