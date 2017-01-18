using System.Windows.Media.Media3D;

namespace kinect2helix.Model
{
    public class PointCloudModel
    {
        // for performance reasons we will not use every depth map point
        public static readonly int SampleSize = 10;

        // controls the size of the point cloud points
        public static readonly int PointSize = 4;

        PointCloud pointCloud;

        // expose the point cloud externally
        public ModelVisual3D Model { get; set; }


        public PointCloudModel()
        {
            // initialize the point cloud
            this.pointCloud = new PointCloud(SampleSize);
            this.pointCloud.Size = PointSize;

            // add the point cloud to the model
            this.Model = new ModelVisual3D();
            this.Model.Children.Add(this.pointCloud);
        }

        public void Update(KinectData data)
        {
            // update the point cloud using kinect data
            this.pointCloud.Update(data);
        }
    }
}
