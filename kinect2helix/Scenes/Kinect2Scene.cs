using HelixToolkit.Wpf;
using kinect2helix.Model;
using Microsoft.Kinect;
using System.Linq;

namespace kinect2helix.Scene
{
    public class Kinect2Scene
    {
        private HelixViewport3D viewport;

        PointCloudModel pointCloudModel;

        public Kinect2Scene()
        {
            this.pointCloudModel = new PointCloudModel();
        }

        public void Attach(HelixViewport3D viewport)
        {
            this.viewport = viewport;

            // add the point cloud model to the viewport to display it
            this.viewport.Children.Add(this.pointCloudModel.Model);
        }

        public void Update(KinectData data)
        {
            // update the point cloud with Kinect data
            this.pointCloudModel.Update(data);
        }
    }
}
