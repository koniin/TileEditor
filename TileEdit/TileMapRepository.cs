﻿using System;
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
        public static IList<Sprite> ReadMapFile(string fileName)
        {
            string[] spriteLines = File.ReadAllLines(fileName);

            List<Sprite> sprites = new List<Sprite>();
            foreach (string line in spriteLines)
            {
                sprites.Add(GetSprite(line));
            }
            return sprites;
        }

        private static readonly string COLUMNDELIMITER = ";";

        private static Sprite GetSprite(string line)
        {
            Sprite sprite = new Sprite();

            string[] values = line.Split(COLUMNDELIMITER.ToCharArray());

            sprite.Name = values[0];
            sprite.X = int.Parse(values[1]);
            sprite.Y = int.Parse(values[2]);
            return sprite;
        }

        public static void WriteMapFile(string fileName, IList<Sprite> spriteLocations, IList<Sprite> sprites)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Sprite sprite in spriteLocations)
            {
                WriteSpriteLine(sb, sprite, sprites.First(s => s.Name == sprite.Name).SourceRect);
                sb.AppendLine();
            }

            File.WriteAllText(fileName, sb.ToString());
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
