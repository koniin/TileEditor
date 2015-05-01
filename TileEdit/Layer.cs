using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEdit.Models;

namespace TileEdit
{
    public class Layer
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public IEnumerable<Sprite> Tiles { get; set; }
    }
}
