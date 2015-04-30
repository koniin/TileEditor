using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TileEdit
{
    public static class SpriteRepository
    {
        private static Dictionary<string, ImageSource> Images = new Dictionary<string,ImageSource>();

        public static ImageSource GetImage(string name)
        {
            return Images[name];
        }

        public static void AddImage(string name, ImageSource image)
        {
            Images.Add(name, image);
        }

        public static bool Contains(string name)
        {
            return Images.ContainsKey(name);
        }
    }
}
