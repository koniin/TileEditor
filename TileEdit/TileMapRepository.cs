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
            List<Sprite> sprites = new List<Sprite>();
            foreach (string line in lines.Skip(1))
            {
                sprites.Add(GetSprite(line));
            }

            tileMap.AddTiles(sprites);
            return tileMap;
        }

        private static TileMap ReadHeader(string header)
        {
            string[] parts = header.Split(',');
            TileMap t = new TileMap(int.Parse(parts[0]), int.Parse(parts[1]));
            return t;
        }

        private static Sprite GetSprite(string line)
        {
            Sprite sprite = new Sprite();

            string[] values = line.Split(COLUMNDELIMITER.ToCharArray());

            sprite.X = int.Parse(values[0].Split(',')[0]);
            sprite.Y = int.Parse(values[0].Split(',')[1]);
            sprite.Name = values[1];
            sprite.SourceRect = StringToRectangle(values[2]);
            return sprite;
        }

        private static Rectangle StringToRectangle(string rectangle)
        {
            string[] values = rectangle.Split(',');
            return new Rectangle(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]));
        }

        public static void WriteMapFile(int width, int height, string fileName, IList<Layer> layers, IList<Sprite> sprites, bool compress = false)
        {
            throw new NotImplementedException();
            //StringBuilder sb = new StringBuilder();

            //AppendHeader(sb, width, height);

            //foreach (Sprite sprite in layers[0].Tiles)
            //{
            //    WriteSpriteLine(sb, sprite, sprites.First(s => s.Name == sprite.Name).SourceRect);
            //    sb.AppendLine();
            //}

            //if(!compress)
            //    File.WriteAllText(fileName, sb.ToString());
            //else
            //    Compression.Compress(fileName, sb.ToString());

        }

        private static void AppendHeader(StringBuilder sb, int width, int height)
        {
            sb.AppendLine(string.Format("{0},{1}", width, height));
        }

        private static void WriteSpriteLine(StringBuilder sb, Sprite sprite, Rectangle sourceRect)
        {
            sb.Append(string.Format("{1},{2}{0}{3}{0}{4}", COLUMNDELIMITER, sprite.X, sprite.Y, sprite.Name, RectangleToString(sourceRect)));
        }

        private static string RectangleToString(Rectangle rect)
        {
            if (rect != null)
                return string.Format("{0},{1},{2},{3}", rect.X, rect.Y, rect.Width, rect.Height);
            return string.Empty;
        }
    }
}
