using Gengine.Map;
using Gengine.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEdit.Models;

namespace TileEdit {
    public class TileMapRepository {
        private IMapRepository mapRepository;
        public TileMapRepository() {
            mapRepository = new MapRepository();
        }

        public void WriteMap(int width, int height, string fileName, IList<Layer> layers, bool compress = false) {
            mapRepository.WriteMap(width, height, fileName, layers, compress);
        }

        public TileMapWrapper LoadMap(string fileName, bool compressed = false) {
            TileMap tileMap = mapRepository.LoadMap(fileName, compressed);
            TileMapWrapper result = new TileMapWrapper(tileMap.Width, tileMap.Height);
            ConvertLayers(result, tileMap.Layers);
            return result;
        }

        private static void ConvertLayers(TileMapWrapper result, IList<Layer> layersToConvert) {
            List<Layer> layers = new List<Layer>();
            foreach (Layer l in layersToConvert) {
                l.Tiles = ConvertTiles(l.Tiles);
                result.Layers.Add(l);
            }
        }

        private static List<Tile> ConvertTiles(List<Tile> list) {
            List<Tile> tiles = new List<Tile>();
            foreach (Tile t in list) {
                tiles.Add(new Sprite(t.TextureName, t.Position, t.SourceRectangle));
            }
            return tiles;
        }
    }
}
