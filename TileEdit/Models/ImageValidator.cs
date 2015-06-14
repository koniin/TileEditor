using System.Drawing;

namespace TileEdit.Models{
    public class ImageValidator{
        public ImageValidator(){}

        public bool IsEmpty(Bitmap cloneBitmap){
            bool result = true;
            for(int i = 0; i < cloneBitmap.Width; i++)
                for(int j = 0; j < cloneBitmap.Height; j++)
                    result = result && EmptyPixel(cloneBitmap);
            return result;
        }

        private static bool EmptyPixel(Bitmap cloneBitmap){
            Color pixel = cloneBitmap.GetPixel(0, 0);
            return pixel.A == 0
                   && pixel.B == 0
                   && pixel.G == 0
                   && pixel.R == 0;
        }
    }
}