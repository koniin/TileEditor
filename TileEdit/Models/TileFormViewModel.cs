using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TileEdit.Models
{
    public class TileFormViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifiyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public ObservableCollection<Sprite> Sprites { get; set; }

        private string _FilePath;
        public string FilePath
        {
            get
            {
                return _FilePath;
            }
            set    
            {
                if (value != _FilePath)
                {
                    _FilePath = value;
                    NotifiyPropertyChanged("FilePath");
                }
            }
        }

        private int _CanvasWidth;
        public int CanvasWidth
        {
            get
            {
                return _CanvasWidth;
            }
            set
            {
                if (value != _CanvasWidth)
                {
                    _CanvasWidth = value;
                    NotifiyPropertyChanged("CanvasWidth");
                }
            }
        }

        private int _CanvasHeight;
        public int CanvasHeight
        {
            get
            {
                return _CanvasHeight;
            }
            set
            {
                if (value != _CanvasHeight)
                {
                    _CanvasHeight = value;
                    NotifiyPropertyChanged("CanvasHeight");
                }
            }
        }

        private int _TileSize;
        public int TileSize
        {
            get
            {
                return _TileSize;
            }
            set
            {
                if (value != _TileSize)
                {
                    _TileSize = value;
                    NotifiyPropertyChanged("TileSize");
                }
            }
        }

        public TileFormViewModel()
        {
            CanvasWidth = 640;
            CanvasHeight = 320;
            TileSize = 32;

            Sprites = new ObservableCollection<Sprite>();
            
            FilePath = Environment.CurrentDirectory + "\\tilemap.txt";

            string settingsDir = Settings.GetSetting(Settings.SpriteDirectory);
            if (settingsDir != null)
            {
                LoadFiles(settingsDir);
            }
        }

        public void LoadFiles(string path)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string filePath in files)
            {
                try
                {
                    LoadSheet(filePath);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Problem loading file");
                }
            }
        }

        public void LoadSheet(string fileName)
        {
            Bitmap image = new Bitmap(fileName);
            string name = System.IO.Path.GetFileName(fileName);


            int x = image.Width / 32;
            int y = image.Height / 32;

            int counter = 0;
            for (int j = 0; j < y; j++)
            {
                for (int i = 0; i < x; i++)
                {
                    string spriteName = name + counter;
                    var cloneRect = new System.Drawing.Rectangle(i * 32, j * 32, 32, 32);
                    Bitmap cloneBitmap = image.Clone(cloneRect, image.PixelFormat);
                    var imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(cloneBitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    ImageRepository.AddImage(spriteName, imageSource);
                    Sprites.Add(new Sprite
                    {
                        Name = spriteName,
                        FilePath = fileName,
                        ImageSource = imageSource,
                        SourceRect = cloneRect
                    });
                    counter++;
                }
            }
        }

    }
}
