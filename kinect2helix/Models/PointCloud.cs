using HelixToolkit.Wpf;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace kinect2helix.Model
{
    public class PointCloud : PointsVisual3D
    {
        // The RGB texture taken from the colour stream
        public WriteableBitmap texture;

        // a background worker to update the point cloud as data arrives
        PointCloudWorker worker;

        // for performance reasons, control the sample size
        private int sampleSize;

        public PointCloud(int sampleSize)
        {
            this.sampleSize = sampleSize;

            // initialize the texture to the size of the RGB frame
            this.texture = new WriteableBitmap(Kinect2Metrics.RGBFrameWidth, Kinect2Metrics.RGBFrameHeight, Kinect2Metrics.DPI, Kinect2Metrics.DPI, PixelFormats.Bgr32, null);

            // setup the texture brush to colour the point cloud using the RGB image
            var materialBrush = new ImageBrush(this.texture)
            {
                ViewportUnits = BrushMappingMode.Absolute,
                Viewport = new System.Windows.Rect(0, 0, this.texture.Width, this.texture.Height),
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top,
                Stretch = Stretch.None,
                TileMode = TileMode.Tile
            };

            this.Model.Material = new DiffuseMaterial(materialBrush);

            // reverse the model on the x axis so we see what Kinect sees
            this.Model.Transform = new ScaleTransform3D(-1, 1, 1);

            worker = new PointCloudWorker(this.sampleSize);
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // get the result from the worker and update our model
            // We will just replace the texturecoordinates and points with the results from the worker
            var workerResult = e.Result as CloudWorkerResult;

            this.Mesh.TextureCoordinates = new PointCollection(workerResult.TexturePoints);
            this.Points = new Point3DCollection(workerResult.PointCloud);
            this.texture.WritePixels(
                                  new Int32Rect(0, 0, Kinect2Metrics.RGBFrameWidth, Kinect2Metrics.RGBFrameHeight),
                                  workerResult.Data.ColorPixels,
                                  Kinect2Metrics.ColorStride,
                                  0);
        }

        public void Update(KinectData data)
        {
            // if the worker is still updating, just bail
            // note: this may add some update delay depending on system performance
            if (this.worker.IsBusy) return;

            // send the data to the worker
            var workerArgs = new CloudWorkerArgs
            {
                Data = data
            };

            this.worker.RunWorkerAsync(workerArgs);
        }
    }
}
