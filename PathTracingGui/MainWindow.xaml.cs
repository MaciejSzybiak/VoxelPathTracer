using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoxelPathTracing;

namespace PathTracingGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _renderSamples = 100;
        private const float Gamma = 2.2f;
        private const float Fov = 0.45f;
        private (int x, int y) _resolution = (x: 1000, y: 1000);
        private Vector3 _cameraOrigin = new(-6, 15, -10);
        private (int x, int y, int z) _gridOrigin = (x: 0, y: 1, z: 0);
        private (int x, int y, int z) _gridSize = (x: 15, y: 11, z: 15);
        private CancellationTokenSource? _cancellationTokenSource;
        private Vector3 _ambientColor = new(0.5f, 0.5f, 0.5f);

        public MainWindow()
        {
            InitializeComponent();

            Samples.Text = _renderSamples.ToString();
            ResolutionX.Text = _resolution.x.ToString();
            ResolutionY.Text = _resolution.y.ToString();
            GridSizeX.Text = _gridSize.x.ToString();
            GridSizeY.Text = _gridSize.y.ToString();
            GridSizeZ.Text = _gridSize.z.ToString();
            GridOriginX.Text = _gridOrigin.x.ToString();
            GridOriginY.Text = _gridOrigin.y.ToString();
            GridOriginZ.Text = _gridOrigin.z.ToString();
            CameraOriginX.Text = _cameraOrigin.X.ToString(CultureInfo.InvariantCulture);
            CameraOriginY.Text = _cameraOrigin.Y.ToString(CultureInfo.InvariantCulture);
            CameraOriginZ.Text = _cameraOrigin.Z.ToString(CultureInfo.InvariantCulture);
            UpdateLabelColor(BackgroundLabel, _ambientColor);
            BackgroundR.Value = _ambientColor.X;
            BackgroundG.Value = _ambientColor.Y;
            BackgroundB.Value = _ambientColor.Z;

        }

        private async Task Render()
        {
            try
            {
                _resolution = (x: int.Parse(ResolutionX.Text), y: int.Parse(ResolutionY.Text));
                _renderSamples = int.Parse(Samples.Text);
                _cameraOrigin = new Vector3(float.Parse(CameraOriginX.Text), float.Parse(CameraOriginY.Text),
                    float.Parse(CameraOriginZ.Text));
                _gridOrigin = (int.Parse(GridOriginX.Text), int.Parse(GridOriginY.Text), int.Parse(GridOriginZ.Text));
                _gridSize = (int.Parse(GridSizeX.Text), int.Parse(GridSizeY.Text), int.Parse(GridSizeZ.Text));
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to parse parameters");
                FinishRender();
                return;
            }

            
            IGridProvider gridProvider = new ColumnGridProvider(_gridOrigin, _gridSize);
            var grid = gridProvider.Get();
            var center = new Vector3(grid.Size.X / 2f + _gridOrigin.x, grid.Size.Y / 2f + _gridOrigin.y, grid.Size.Z / 2f + _gridOrigin.z);
            var camera = new PerspectiveCamera(Fov, (float) _resolution.x / _resolution.y, 
                _cameraOrigin, center - Vector3.One * 1, Vector3.UnitY);
            var colorCorrection = new ColorCorrection(Gamma, 1);
            var world = new World(grid, _ambientColor, new Floor(0, new Material(Vector3.One * 0.7f, 0)));
            var renderer = new Renderer(world, camera, colorCorrection, _renderSamples, _resolution);
            
            var progress = new Progress<RenderProgress>();
            progress.ProgressChanged += SetProgressPreview;

            _cancellationTokenSource = new CancellationTokenSource();
            await renderer.Render(progress, _cancellationTokenSource.Token);
            
            FinishRender();
        }

        private void CancelRender()
        {
            _cancellationTokenSource?.Cancel();
        }

        private void FinishRender()
        {
            ProgressBar.Visibility = Visibility.Collapsed;
            CancelBtn.Visibility = Visibility.Collapsed;
            RenderBtn.IsEnabled = true;
        }

        private void SetProgressPreview(object? sender, RenderProgress progress)
        {
            ProgressBar.Value = progress.Percentage;
            
            var stride = _resolution.x * 3;
            
            var pixels = new byte[_resolution.y * stride];
            
            var i = 0;
            for (var y = 0; y < _resolution.y; y++)
            {
                for (var x = 0; x < _resolution.x; x++)
                {
                    var color = progress.Image[x, _resolution.y - 1 - y];
                    pixels[i++] = (byte) (color.X * 255);
                    pixels[i++] = (byte) (color.Y * 255);
                    pixels[i++] = (byte) (color.Z * 255);
                }
            }
            
            var bitmapSource = BitmapSource.Create(_resolution.x, _resolution.y,
                96, 96, PixelFormats.Rgb24, null, pixels, stride);
            RenderImage.Source = bitmapSource;
        }

        private async void RenderBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;
            CancelBtn.Visibility = Visibility.Visible;
            RenderBtn.IsEnabled = false;
            
            ProgressBar.Value = 0;
            await Render();
        }

        private void CancelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            CancelRender();
            FinishRender();
        }

        private void Background_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _ambientColor = new Vector3((float) BackgroundR.Value, (float) BackgroundG.Value,
                (float) BackgroundB.Value);
            UpdateLabelColor(BackgroundLabel, _ambientColor);
        }

        private void UpdateLabelColor(Control label, Vector3 value)
        {
            label.Background = new SolidColorBrush(Color.FromRgb(
                (byte) (value.X * 255),
                (byte) (value.Y * 255),
                (byte) (value.Z * 255)
            ));
        }
    }
}