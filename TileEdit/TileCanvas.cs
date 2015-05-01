using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TileEdit.Models;
using System.Collections.Generic;

namespace TileEdit
{
    public class TileCanvas : Canvas
    {
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

        private ObservableCollection<Sprite> _Tiles;
        public ObservableCollection<Sprite> Tiles
        {
            get
            {
                return _Tiles;
            }
        }

        public TileCanvas()
        {
            _Tiles = new ObservableCollection<Sprite>();
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
            int x = (int)position.X;
            int y = (int)position.Y;

            Debug.WriteLine("x: " + x + ", y: " + y);
            
            int startX = x - (x % _TileSize);
            int startY = y - (y % _TileSize);

            Debug.WriteLine("start x : " + startX);
            Debug.WriteLine("start y : " + startY);

            Sprite sprite = new Sprite();

            sprite.Name = CurrentTile.Name;
            sprite.X = startX;
            sprite.Y = startY;

            if(startY < this.Height && startX < this.Width)
                AddTile(sprite);
        }

        public void AddTile(Sprite sprite)
        {
            RemoveTile(sprite.X, sprite.Y);

            Tiles.Add(sprite);
        }

        public void RemoveTile(int x, int y)
        {
            Sprite sp = Tiles.FirstOrDefault(t => t.X == x && t.Y == y);
            if (sp != null)
                Tiles.Remove(sp);
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
            foreach (Sprite sprite in Tiles)
            {
                if (ImageRepository.Contains(sprite.Name)) {
                    ImageSource image = ImageRepository.GetImage(sprite.Name);
                    rect.X = sprite.X;
                    rect.Y = sprite.Y;
                    rect.Height = image.Height;
                    rect.Width = image.Width;
                    dc.DrawImage(image, rect);
                }
            }
        }

        internal void ClearTiles()
        {
            Tiles.Clear();
            this.InvalidateVisual();
        }
    }
}
