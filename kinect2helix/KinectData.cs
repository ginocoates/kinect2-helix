using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinect2helix
{
    public class KinectData
    {
        public byte[] ColorPixels { get; set; }

        public ushort[] DepthPixels { get; set; }
        
        public CameraSpacePoint[] MappedDepthToCameraSpacePixels { get; set; }
        public ColorSpacePoint[] MappedDepthToColorPixels { get; set; }
    }
}
