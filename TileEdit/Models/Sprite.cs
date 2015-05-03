using Gengine.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace TileEdit.Models {
    public class Sprite : Tile {
        public ImageSource ImageSource { get; set; }
        public Rect Rect {
            get {
                return new System.Windows.Rect(Position.X, Position.Y, SourceRectangle.Width, SourceRectangle.Height);
            }
        }

        public Sprite(string textureName, Vector2 position, Rectangle sourceRectangle) : base(textureName, position, sourceRectangle) { }
    }
}
