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
using Microsoft.Xna.Framework;

namespace TileEdit {
    public class TileCanvas : Canvas, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifiyPropertyChanged(string property) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private int _TileSize = 32;
        public int TileSize {
            get {
                return _TileSize;
            }
            set {
                _TileSize = value;
            }
        }

        private int _SelectedLayer;
        public int SelectedLayer {
            get {
                return _SelectedLayer;
            }
            set {
                if (value != _SelectedLayer) {
                    _SelectedLayer = value;
                    NotifiyPropertyChanged("SelectedLayer");
                }
            }
        }

        public ObservableCollection<Layer> Layers { get; private set; }

        public TileCanvas() {
            Layers = new ObservableCollection<Layer>();
            Layers.Add(new Layer { Index = 0, Name = "Main" });
            CacheMode = new BitmapCache();
        }

        public Sprite CurrentTile { get; set; }

        protected override void OnMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
            base.OnMouseRightButtonDown(e);

            System.Windows.Point position = e.GetPosition(this);
            RemoveCurrentTile(position);
            this.InvalidateVisual();
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);

            if (CurrentTile == null)
                MessageBox.Show("You must select a sprite first.");
            else {
                System.Windows.Point position = e.GetPosition(this);
                AddCurrentTile(position);
            }
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e) {
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e) {
            base.OnMouseMove(e);

            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
                if (AddCurrentTile(e.GetPosition(this))) { 

                }
            }
        }

        public void AddNewLayer(string name) {
            Layers.Add(new Layer { Name = name, Index = Layers.Count() });
            _SelectedLayer = 1;
        }

        private void RemoveCurrentTile(System.Windows.Point position) {
            int x = (int)position.X;
            int y = (int)position.Y;
            int startX = x - (x % _TileSize);
            int startY = y - (y % _TileSize);
            RemoveTile(startX, startY);
        }

        private Vector2 lastMousePosition = Vector2.Zero;

        private bool AddCurrentTile(System.Windows.Point position) {
            if (CurrentTile == null) {
                MessageBox.Show("No tile selected", "Missing tile");
                return false;
            }

            int x = (int)position.X;
            int y = (int)position.Y;
            int startX = x - (x % _TileSize);
            int startY = y - (y % _TileSize);
            Vector2 currentPosition = new Vector2(startX, startY);
            if (currentPosition.X == lastMousePosition.X && currentPosition.Y == lastMousePosition.Y)
                return false;

            lastMousePosition = currentPosition;

            Sprite sprite = new Sprite(CurrentTile.TextureName, currentPosition, CurrentTile.SourceRectangle);
            sprite.ImageSource = CurrentTile.ImageSource;
            
            if (startY < this.Height && startX < this.Width)
                AddTile(sprite);
            return true;
        }

        public void AddTile(Sprite sprite) {
            RemoveTile((int)sprite.Position.X, (int)sprite.Position.Y);

            Layers[_SelectedLayer].Tiles.Add(sprite);

            Image finalImage = new Image();
            finalImage.Source = sprite.ImageSource;
            Canvas.SetLeft(finalImage, sprite.Position.X);
            Canvas.SetTop(finalImage, sprite.Position.Y);
            Children.Add(finalImage);
        }

        public void RemoveTile(int x, int y) {
            Tile tile = Layers[_SelectedLayer].Tiles.FirstOrDefault(t => t.Position.X == x && t.Position.Y == y);
            if (tile != null) {
                Layers[_SelectedLayer].Tiles.Remove(tile);

                UIElement toRemove = null;
                foreach (UIElement child in Children) {
                    UIElement container = VisualTreeHelper.GetParent(child) as UIElement; 
                    System.Windows.Point relativeLocation = child.TranslatePoint(new System.Windows.Point(0, 0), container);
                    if (relativeLocation.X == x && relativeLocation.Y == y)
                        toRemove = child;
                }
                if (toRemove != null)
                    Children.Remove(toRemove);
            }
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc) {
            base.OnRender(dc);

            Pen pen = new Pen(Brushes.DarkGray, 1);
            Rect rect = new Rect();
            for (int y = 0; y < this.Height + _TileSize; y += _TileSize) {
                for (int x = 0; x < this.Width + _TileSize; x += _TileSize) {
                    rect.X = x;
                    rect.Y = y; 
                    rect.Width = _TileSize;
                    rect.Height = _TileSize;
                    dc.DrawRectangle(null, pen, rect);

                }
            }

            RenderTiles(dc);
        }

        protected void RenderTiles(System.Windows.Media.DrawingContext dc) {
            double opacityIncrement = 1.0f / Layers.Count;
            double opacity = opacityIncrement;
            foreach (Layer layer in Layers) {
                dc.PushOpacity(opacity);
                foreach (Sprite sprite in layer.Tiles) {
                    dc.DrawImage(sprite.ImageSource, sprite.Rect);
                }
                dc.Pop();
                opacity += opacityIncrement;
            }
        }

        internal void ClearTiles() {
            Layers.Clear();
            this.InvalidateVisual();
        }

        public void Update(IList<Sprite> sprites) {
            foreach (Layer layer in Layers) {
                foreach (Sprite sprite in layer.Tiles) {
                    var target = sprites.FirstOrDefault(s => s.TextureName == sprite.TextureName && s.SourceRectangle.X == sprite.SourceRectangle.X && s.SourceRectangle.Y == sprite.SourceRectangle.Y);
                    if (target != null)
                        sprite.ImageSource = target.ImageSource;
                }
            }
        }

        public void AddLayers(IList<Layer> layers) {
            if (layers.Any()) {
                foreach (var layer in layers) {
                    Layers.Add(layer);
                }
                _SelectedLayer = 1;
            } else {
                AddNewLayer("Main");
            }
        }
    }
}
