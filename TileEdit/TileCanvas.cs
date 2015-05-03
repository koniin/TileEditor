using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TileEdit.Models;
using System.Collections.Generic;
using System.ComponentModel;
using Gengine.Map;

namespace TileEdit
{
    public class TileCanvas : Canvas, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifiyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private int _TileSize = 32;
        public int TileSize
        {
            get
            {
                return _TileSize;
            }
            set
            {
                _TileSize = value;
            }
        }

        private int _SelectedLayer;
        public int SelectedLayer
        {
            get
            {
                return _SelectedLayer;
            }
            set
            {
                if (value != _SelectedLayer)
                {
                    _SelectedLayer = value;
                    NotifiyPropertyChanged("SelectedLayer");
                }
            }
        }

        public ObservableCollection<Layer> Layers { get; set; }

        public TileCanvas()
        {
            Layers = new ObservableCollection<Layer>();
            Layers.Add(new Layer { Index = 0, Name = "Main" });
        }

        public Sprite CurrentTile { get; set; }

        protected override void OnMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            Point position = e.GetPosition(this);
            RemoveCurrentTile(position);
            this.InvalidateVisual();
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            
            if (CurrentTile == null)
                MessageBox.Show("You must select a sprite first.");
            else
            {
                Point position = e.GetPosition(this);
                AddCurrentTile(position);
                this.InvalidateVisual();
            }
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                AddCurrentTile(e.GetPosition(this));
                this.InvalidateVisual();
            }
        }

        public void AddNewLayer(string name)
        {
            Layers.Add(new Layer { Name = name, Index = Layers.Count() });
        }

        private void RemoveCurrentTile(Point position)
        {
            int x = (int)position.X;
            int y = (int)position.Y;
            int startX = x - (x % _TileSize);
            int startY = y - (y % _TileSize);
            RemoveTile(startX, startY);
        }

        private void AddCurrentTile(Point position)
        {
            if (CurrentTile == null)
            {
                MessageBox.Show("No tile selected", "Missing tile");
                return;
            }

            int x = (int)position.X;
            int y = (int)position.Y;

            Debug.WriteLine("x: " + x + ", y: " + y);
            
            int startX = x - (x % _TileSize);
            int startY = y - (y % _TileSize);

            Debug.WriteLine("start x : " + startX);
            Debug.WriteLine("start y : " + startY);

            Sprite sprite = new Sprite(startX, startY, new System.Drawing.Rectangle(startX, startY, _TileSize, _TileSize));
            sprite.Name = CurrentTile.Name;
            sprite.EditorId = CurrentTile.EditorId;

            if(startY < this.Height && startX < this.Width)
                AddTile(sprite);
        }

        public void AddTile(Sprite sprite)
        {
            RemoveTile(sprite.X, sprite.Y);

            Layers[_SelectedLayer].Tiles.Add(sprite);
        }

        public void RemoveTile(int x, int y)
        {
            Tile tile = Layers[_SelectedLayer].Tiles.FirstOrDefault(t => t.Position.X == x && t.Position.Y == y);
            if (tile != null)
                Layers[_SelectedLayer].Tiles.Remove(tile);
            this.InvalidateVisual();
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);

            Pen pen = new Pen(Brushes.Black, 1);
            Rect rect = new Rect();
            for (int y = 0; y < this.Height; y += _TileSize)
            {
                for (int x = 0; x < this.Width; x += _TileSize)
                {
                    rect.X = x;
                    rect.Y = y;
                    dc.DrawRectangle(null, pen, rect);

                }
            }

            RenderTiles(dc);
        }

        protected void RenderTiles(System.Windows.Media.DrawingContext dc)
        {
            Rect rect = new Rect();
            double opacityIncrement = 1.0f / Layers.Count;
            double opacity = opacityIncrement;
            foreach (Layer layer in Layers)
            {
                dc.PushOpacity(opacity);
                foreach (Sprite sprite in layer.Tiles)
                {
                    if (ImageRepository.Contains(sprite.EditorId))
                    {
                        ImageSource image = ImageRepository.GetImage(sprite.EditorId);
                        rect.X = sprite.X;
                        rect.Y = sprite.Y;
                        rect.Height = image.Height;
                        rect.Width = image.Width;
                        dc.DrawImage(image, rect);
                    }
                }
                dc.Pop();
                opacity += opacityIncrement;
            }
        }

        internal void ClearTiles()
        {
            Layers.Clear();
            this.InvalidateVisual();
        }
    }
}
