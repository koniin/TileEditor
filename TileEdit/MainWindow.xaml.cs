using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TileEdit.Models;

namespace TileEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TileFormViewModel model;
        private string lastFolder = null;
        private string currentMap;
        public MainWindow()
        {
            InitializeComponent();

            model = new TileFormViewModel();

            model.PropertyChanged += model_PropertyChanged;

            this.DataContext = model;

            DrawRulers();
        }

        void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CanvasWidth" || e.PropertyName == "CanvasHeight" || e.PropertyName == "TileSize")
            {
                DrawRulers();
            }
        }

        private void DrawRulers()
        {
            TopRuler.Children.Clear();
            LeftRuler.Children.Clear();

            for(int i = 0; i < model.CanvasWidth; i += model.TileSize)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = i.ToString();
                textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                textBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Canvas.SetLeft(textBlock, i+5);
                //Canvas.SetTop(textBlock, 0);
                TopRuler.Children.Add(textBlock);
            }

            for (int i = 0; i < model.CanvasHeight; i += model.TileSize)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = i.ToString();
                textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                Canvas.SetTop(textBlock, i+15);

                //Dirty for fixing margin (alignment)
                Canvas.SetLeft(textBlock, i > 999 ? 1 : (i > 99 ? 5 : (i > 9 ? 10 : 15)));
                LeftRuler.Children.Add(textBlock);
            }
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            if (lastFolder != null)
                dialog.InitialDirectory = lastFolder;
            dialog.Filter = "Map Files|*.tmap|Text Files|*.txt";
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                TileGrid.ClearTiles();
                File.Create(dialog.FileName);
                FilePath.Text = dialog.FileName;
                currentMap = dialog.FileName;
                Status.Text = "Created new tilemap in: " + dialog.FileName;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            TileMapRepository.WriteMapFile(model.CanvasWidth, model.CanvasHeight, currentMap, TileGrid.Tiles, model.Sprites);
            Status.Text = "Saved tilemap to: " + FilePath.Text;
        }

        private void SaveCompressed_Click(object sender, RoutedEventArgs e)
        {
            TileMapRepository.WriteMapFile(model.CanvasWidth, model.CanvasHeight, currentMap, TileGrid.Tiles, model.Sprites, true);
            Status.Text = "Saved tilemap compressed to: " + FilePath.Text + ".gz";
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveAs(false);
        }

        private void SaveAsCompressed_Click(object sender, RoutedEventArgs e)
        {
            SaveAs(true);
        }

        private void SaveAs(bool compress)
        {
            var dialog = new SaveFileDialog();
            if (lastFolder != null)
                dialog.InitialDirectory = lastFolder;
            dialog.Filter = "Map Files|*.tmap|Text Files|*.txt";
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SaveFile(dialog.FileName, compress);
            }
        }

        private void SaveFile(string fileName, bool compress = false)
        {
            FilePath.Text = fileName;
            currentMap = fileName;
            TileMapRepository.WriteMapFile(model.CanvasWidth, model.CanvasHeight, fileName, TileGrid.Tiles, model.Sprites, compress);
            Status.Text = "Saved tilemap to: " + fileName;
            Settings.AddUpdateAppSettings(Settings.SpriteDirectory, System.IO.Path.GetFileName(fileName));
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            LoadTileMap(FilePath.Text);
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(false);
        }

        private void BrowseCompressed_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(true);
        }

        private void OpenFile(bool compressed)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (compressed)
                dialog.Filter = "GZip files|*.gz";
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FilePath.Text = dialog.FileName;
                LoadTileMap(dialog.FileName, compressed);
            }
        }

        private void LoadTileMap(string fileName, bool compressed = false)
        {
            currentMap = fileName;
            TileGrid.ClearTiles();

            TileMap tileMap = TileMapRepository.ReadMapFile(fileName, compressed);
            if (tileMap != null)
            {
                foreach (Sprite sprite in tileMap.Tiles)
                {
                    TileGrid.AddTile(sprite);
                }

                model.CanvasWidth = tileMap.Width;
                model.CanvasHeight = tileMap.Height;
            }
            Status.Text = "Loaded tilemap from: " + currentMap;
        }

        private void AddSprites_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (lastFolder != null)
                dialog.SelectedPath = lastFolder;
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) 
            {
                model.LoadFiles(dialog.SelectedPath);
                Settings.AddUpdateAppSettings(Settings.SpriteDirectory, dialog.SelectedPath);
                lastFolder = dialog.SelectedPath;
            }
        }

        private void AddSpriteSheet_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (lastFolder != null)
                dialog.InitialDirectory = lastFolder;

            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) 
            {
                model.LoadSheet(dialog.FileName);
            }
            TileGrid.InvalidateVisual();
        }

        private void lstImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TileGrid.CurrentTile = lstImages.SelectedItem as Sprite;
        }

        private void IncreaseWidth_Click(object sender, RoutedEventArgs e)
        {
            model.CanvasWidth += int.Parse(TileSize.Text);
        }

        private void IncreaseHeight_Click(object sender, RoutedEventArgs e)
        {
            model.CanvasHeight += int.Parse(TileSize.Text);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Made by Henrik Aronsson 2015", "About", MessageBoxButton.OK);
        }
    }
}
