using Microsoft.Xna.Framework;
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

        private int _CanvasViewHeight;
        public int CanvasViewHeight
        {
            get
            {
                return _CanvasViewHeight;
            }
            set
            {
                if (value != _CanvasViewHeight)
                {
                    _CanvasViewHeight = value;
                    NotifiyPropertyChanged("CanvasViewHeight");
                }
            }
        }

        private int _CanvasViewWidth;
        public int CanvasViewWidth
        {
            get
            {
                return _CanvasViewWidth;
            }
            set
            {
                if (value != _CanvasViewWidth)
                {
                    _CanvasViewWidth = value;
                    NotifiyPropertyChanged("CanvasViewWidth");
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
            CanvasViewHeight = 358;
            CanvasViewWidth = 900;
            TileSize = 32;

            Sprites = new ObservableCollection<Sprite>();
            
            FilePath = Environment.CurrentDirectory + "\\tilemap.txt";

            PropertyChanged += TileFormViewModel_PropertyChanged;

            string settingsDir = Settings.GetSetting(Settings.SpriteDirectory);
            if (settingsDir != null)
            {
                LoadFiles(settingsDir);
            }
        }

        void TileFormViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CanvasHeight")
            {
                CanvasViewHeight = Math.Min(570, CanvasHeight + TileSize + 10);
                    // 358
            } 
            else if(e.PropertyName == "CanvasWidth")
            {
                CanvasViewWidth = Math.Min(900, CanvasWidth + 60);

                // 900
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
            Bitmap image;
            try
            {
                image = new Bitmap(fileName);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "Error loading image");
                return;
            }

            string name = System.IO.Path.GetFileName(fileName);

            int x = image.Width / 32;
            int y = image.Height / 32;

            int counter = 0;
            for (int j = 0; j < y; j++)
            {
                for (int i = 0; i < x; i++)
                {
                    string spriteName = name;
                    var cloneRect = new System.Drawing.Rectangle(i * 32, j * 32, 32, 32);
                    Bitmap cloneBitmap = image.Clone(cloneRect, image.PixelFormat);
                    var imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(cloneBitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    Sprites.Add(new Sprite(spriteName, new Vector2(0, 0), new Microsoft.Xna.Framework.Rectangle(cloneRect.X, cloneRect.Y, cloneRect.Width, cloneRect.Height))
                    {
                        ImageSource = imageSource
                    });
                    counter++;
                }
            }
        }

    }
}
