using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinect2helix
{
    public class CloudWorkerArgs
    {
        public KinectData Data { get; set; }

        public int SampleSize { get; set; }
    }
}
