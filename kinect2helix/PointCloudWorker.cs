using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace kinect2helix
{
    public class PointCloudWorker : BackgroundWorker
    {
        Point[] texturePoints;
        Point3D[] pointCloud;

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            // if we have no depth data, just bail here
            var args = e.Argument as CloudWorkerArgs;
            var data = args.Data;

            // select a set of indexes based on the sample size
            var pointIndexes = Enumerable.Range(0, Kinect2Metrics.DepthFrameWidth * Kinect2Metrics.DepthFrameHeight)
                .Where(i => i % args.SampleSize == 0).ToArray();

            if (this.texturePoints == null)
            {
                // initialize the arrays of points with empty data
                this.texturePoints = pointIndexes.SelectMany(i => Enumerable.Repeat(new Point(), 4)).ToArray();
                this.pointCloud = pointIndexes.Select(i => new Point3D()).ToArray();
            }

            // update the texture coordinates and points in parallel
            Parallel.For(0, pointIndexes.Count(), (i) =>
            {
                var j = pointIndexes[i];                

                // update texture coordinates, we need four vertices per point in the cloud
                // however, all texture coordiates can point to the same RGB pixel so the 
                // point assumes that colour

                var colorPoint = data.MappedDepthToColorPixels[j];

                var k = i * 4; //there are 4 texture vertices per cloud point

                this.texturePoints[k].X = float.IsNegativeInfinity(colorPoint.X) ? 0 : colorPoint.X;
                this.texturePoints[k].Y = float.IsNegativeInfinity(colorPoint.Y) ? 0 : colorPoint.Y;
                this.texturePoints[k + 1].X = float.IsNegativeInfinity(colorPoint.X) ? 0 : colorPoint.X;
                this.texturePoints[k + 1].Y = float.IsNegativeInfinity(colorPoint.Y) ? 0 : colorPoint.Y;
                this.texturePoints[k + 2].X = float.IsNegativeInfinity(colorPoint.X) ? 0 : colorPoint.X;
                this.texturePoints[k + 2].Y = float.IsNegativeInfinity(colorPoint.Y) ? 0 : colorPoint.Y;
                this.texturePoints[k + 3].X = float.IsNegativeInfinity(colorPoint.X) ? 0 : colorPoint.X;
                this.texturePoints[k + 3].Y = float.IsNegativeInfinity(colorPoint.Y) ? 0 : colorPoint.Y;

                // update the 3D positions in the point cloud
                var cloudPoint = data.MappedDepthToCameraSpacePixels[j];

                this.pointCloud[i].X = float.IsNegativeInfinity(cloudPoint.X) ? 0 : cloudPoint.X;
                this.pointCloud[i].Y = float.IsNegativeInfinity(cloudPoint.Y) ? 0 : cloudPoint.Y;
                this.pointCloud[i].Z = float.IsNegativeInfinity(cloudPoint.Z) ? 0 : cloudPoint.Z;
            });

            // return the result
            var workerResult = new CloudWorkerResult
            {
                Data = data,
                TexturePoints = this.texturePoints,
                PointCloud = this.pointCloud
            };

            e.Result = workerResult;

            base.OnDoWork(e);
        }
    }
}
