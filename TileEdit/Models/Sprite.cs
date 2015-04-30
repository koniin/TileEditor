using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace TileEdit.Models
{
    public class Sprite
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public ImageSource ImageSource { get; set; }
    }
}
