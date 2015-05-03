using Gengine.Map;
using Gengine.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEdit.Models;

namespace TileEdit
{
    public static class TileMapRepository
    {
        private static readonly string COLUMNDELIMITER = ";";

        private struct LayerInfo
        {
            public string Name;
            public int Index;
        }

        public static TileMap ReadMapFile(string fileName, bool compressed = false)
        {
            IEnumerable<string> lines;
            if (compressed)
                lines = Compression.Decompress(fileName);
            else 
                lines = File.ReadAllLines(fileName);

            if (lines.Count() == 0)
                return null;

            TileMap tileMap = ReadHeader(lines.First());
            foreach (string line in lines.Skip(1))
            {
                LayerInfo layerInfo = GetLayerInfo(line);
                if (!tileMap.Layers.Any(l => l.Name == layerInfo.Name))
                    tileMap.Layers.Add(new Layer(layerInfo.Name, layerInfo.Index));

                var layer = tileMap.Layers.First(l => l.Name == layerInfo.Name);
                layer.Tiles.Add(GetSprite(line));
            }

            return tileMap;
        }

        private static LayerInfo GetLayerInfo(string line)
        {
            string[] values = line.Split(COLUMNDELIMITER.ToCharArray());
            string[] info = values[0].Split(',');

            return new LayerInfo
            {
                Name = info[0],
                Index = int.Parse(info[1])
            };
        }

        private static TileMap ReadHeader(string header)
        {
            string[] parts = header.Split(',');
            TileMap t = new TileMap(int.Parse(parts[0]), int.Parse(parts[1]));
            return t;
        }

        private static Sprite GetSprite(string line)
        {
            string[] values = line.Split(COLUMNDELIMITER.ToCharArray());

            Sprite sprite = new Sprite(int.Parse(values[2].Split(',')[0]),
                int.Parse(values[2].Split(',')[1]),
                StringToRectangle(values[4]));
            sprite.SourceRect = StringToRectangle(values[4]);
            sprite.EditorId = values[3];
            sprite.Name = values[1];
            return sprite;
        }

        private static Rectangle StringToRectangle(string rectangle)
        {
            string[] values = rectangle.Split(',');
            return new Rectangle(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]));
        }

        public static void WriteMapFile(int width, int height, string fileName, IList<Layer> layers, IList<Sprite> sprites, bool compress = false)
        {
            StringBuilder sb = new StringBuilder();

            AppendHeader(sb, width, height);

            foreach (Layer layer in layers)
            {
                foreach (Sprite sprite in layer.Tiles)
                {
                    WriteSpriteLine(sb, layer, sprite, sprites.First(s => s.EditorId == sprite.EditorId).SourceRect);
                }
            }

            if(!compress)
                File.WriteAllText(fileName, sb.ToString());
            else
                Compression.Compress(fileName, sb.ToString());
        }

        private static void AppendHeader(StringBuilder sb, int width, int height)
        {
            sb.AppendLine(string.Format("{0},{1}", width, height));
        }

        private static void WriteSpriteLine(StringBuilder sb, Layer layer, Sprite sprite, Rectangle sourceRect)
        {
            sb.AppendLine(string.Format("{1}{0}{2}{0}{3},{4}{0}{5}{0}{6}", COLUMNDELIMITER, layer.Serialize(), sprite.Name, sprite.X, sprite.Y, sprite.EditorId, RectangleToString(sourceRect)));
        }

        private static string RectangleToString(Rectangle rect)
        {
            if (rect != null)
                return string.Format("{0},{1},{2},{3}", rect.X, rect.Y, rect.Width, rect.Height);
            return string.Empty;
        }
    }
}
