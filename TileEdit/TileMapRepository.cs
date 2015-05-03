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

namespace TileEdit
{
    public static class TileMapRepository {
        public static void WriteMapFile(int width, int height, string fileName, IList<Layer> layers, bool compress = false) {
            MapRepository.WriteMapFile(width, height, fileName, layers, compress);
        }

        public static TileMap ReadMapFile(string fileName, bool compressed = false) {
            Gengine.Map.TileMap tileMap = MapRepository.ReadMapFile(fileName, compressed);
            TileMap result = new TileMap(tileMap.Width, tileMap.Height);
            ConvertLayers(result, tileMap.Layers);
            return result;
        }

        private static void ConvertLayers(TileMap result, IList<Layer> layersToConvert) {
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
