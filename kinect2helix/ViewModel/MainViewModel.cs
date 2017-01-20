using GalaSoft.MvvmLight;
using kinect2helix.Scene;
using Microsoft.Kinect;
using System;

namespace kinect2helix.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        // the one and only kinect sensor
        KinectSensor sensor;

        // Frame reader returning multiple frames
        MultiSourceFrameReader frameReader;

        /// <summary>
        /// Gets or sets the KinectScene to display in the Helix Viewport
        /// </summary>
        public Kinect2Scene KinectScene
        {
            get; private set;
        }

        public MainViewModel()
        {
            // initialize the scene with the kinect coordinate mapper
            this.KinectScene = new Kinect2Scene();

            // create the Kinect sensor
            this.sensor = KinectSensor.GetDefault();

            if (!this.sensor.IsAvailable)
            {
                // nothing works without a Kinect!
                throw new InvalidOperationException("Plug in the Kinect Sensor");
            }

            // We'll be creating a colour 3D depth map, so listen for color and depth frames
            this.frameReader = this.sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Infrared | FrameSourceTypes.Depth | FrameSourceTypes.Color);

            this.frameReader.MultiSourceFrameArrived += FrameReader_MultiSourceFrameArrived;
            
            this.sensor.Open();
        }


        private void FrameReader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {           
            var frame = e.FrameReference.AcquireFrame();

            if (frame == null) return;

            var colorFrame = frame.ColorFrameReference.AcquireFrame();
            var depthFrame = frame.DepthFrameReference.AcquireFrame();

            try
            {
                // we need both frames to update the point cloud, if we don't both, bail here
                if (depthFrame == null || colorFrame == null)
                {
                    return;
                };

                KinectData kinectData = new KinectData
                {
                    ColorPixels = new byte[Kinect2Metrics.ColorBufferLength],
                    DepthPixels = new ushort[Kinect2Metrics.DepthFrameWidth * Kinect2Metrics.DepthFrameHeight]
                };

                // get the color pixels            
                var pinnedArray = System.Runtime.InteropServices.GCHandle.Alloc(kinectData.ColorPixels, System.Runtime.InteropServices.GCHandleType.Pinned);

                try
                {
                    IntPtr colorPointer = pinnedArray.AddrOfPinnedObject();

                    if (colorFrame.RawColorImageFormat == ColorImageFormat.Bgra)
                    {
                        colorFrame.CopyRawFrameDataToIntPtr(colorPointer, (uint)Kinect2Metrics.ColorBufferLength);
                    }
                    else
                    {
                        colorFrame.CopyConvertedFrameDataToIntPtr(colorPointer, (uint)Kinect2Metrics.ColorBufferLength, ColorImageFormat.Bgra);
                    }
                }
                finally
                {
                    pinnedArray.Free();
                }

                var pinnedDepthArray = System.Runtime.InteropServices.GCHandle.Alloc(kinectData.DepthPixels, System.Runtime.InteropServices.GCHandleType.Pinned);

                try
                {
                    IntPtr depthPointer = pinnedDepthArray.AddrOfPinnedObject();
                    depthFrame.CopyFrameDataToIntPtr(depthPointer, (uint)Kinect2Metrics.DepthBufferLength);
                }
                finally
                {
                    pinnedDepthArray.Free();
                }

                // map the depth pixels to CameraSpace for 3D rendering
                MapperUtils.MapDepthFrameToCameraSpace(this.sensor.CoordinateMapper, kinectData);

                // map color pixels to depth space so we can color the depth map
                MapperUtils.MapDepthFrameToColorSpace(this.sensor.CoordinateMapper, kinectData);
                
                // update the scene using the kinect data
                this.KinectScene.Update(kinectData);
            }
            finally {
                // if we don't dispose these, kinect won't send another one
                if (depthFrame != null) depthFrame.Dispose();
                if (colorFrame != null) colorFrame.Dispose();
            }
        }
    }
}