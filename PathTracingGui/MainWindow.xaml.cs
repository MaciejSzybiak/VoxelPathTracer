using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
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
        // const int renderSamples = 1;
        // const float gamma = 2.2f;
        // const float fov = 0.45f;
        // (int x, int y) resolution = (x: 1000, y: 1000);
        // Vector3 cameraOrigin = new(-6, 15, -10);
        
        public MainWindow()
        {
            InitializeComponent();
            
            // IGridProvider gridProvider = new ColumnGridProvider();
            // var grid = gridProvider.Get();
            //
            // var camera = new PerspectiveCamera(fov, (float) resolution.x / resolution.y, 
            //     cameraOrigin, Vector3.One * (grid.Size / 2f), Vector3.UnitY);
            // var colorCorrection = new ColorCorrection(gamma, 1);
            // var renderer = new Renderer(grid, camera, colorCorrection, renderSamples, resolution);
            //
            // var render = renderer.Render();
            //
            // var stride = resolution.x * 3;
            //
            // var pixels = new byte[resolution.y * stride];
            //
            // var i = 0;
            // for (var y = 0; y < resolution.y; y++)
            // {
            //     for (var x = 0; x < resolution.x; x++)
            //     {
            //         var color = render[x, resolution.y - 1 - y];
            //         pixels[i++] = (byte) (color.X * 255);
            //         pixels[i++] = (byte) (color.Y * 255);
            //         pixels[i++] = (byte) (color.Z * 255);
            //     }
            // }
            //
            // var bitmapSource = BitmapSource.Create(resolution.x, resolution.y,
            //     96, 96, PixelFormats.Rgb24, null, pixels, stride);
            // RenderImage.Source = bitmapSource;
        }
    }
}