using Gengine.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace TileEdit.Models
{
    public class Sprite : Tile
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public ImageSource ImageSource { get; set; }
        public System.Drawing.Rectangle SourceRect { get; set; }

        public Sprite(int x, int y, System.Drawing.Rectangle rect) : base(null, new Vector2(x, y), new Rectangle(rect.X, rect.Y, rect.Width, rect.Height)) {
            X = x;
            Y = y;
        }
        public Sprite(Texture2D texture, Vector2 position, Rectangle sourceRectangle) : base(texture, position, sourceRectangle) { }
    }
}
