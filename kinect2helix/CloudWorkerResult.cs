using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace kinect2helix
{
    class CloudWorkerResult
    {
        public KinectData Data { get; set; }

        public Point3D[] PointCloud { get; set; }

        public Point[] TexturePoints { get; set; }
    }
}
