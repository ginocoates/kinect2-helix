using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinect2helix
{
    public class MapperUtils
    {
        public static void MapDepthFrameToCameraSpace(CoordinateMapper mapper, KinectData data)
        {
            if (data.DepthPixels == null) return;

            var pinnedDepthArray = System.Runtime.InteropServices.GCHandle.Alloc(data.DepthPixels, System.Runtime.InteropServices.GCHandleType.Pinned);

            data.MappedDepthToCameraSpacePixels = new CameraSpacePoint[Kinect2Metrics.DepthFrameWidth * Kinect2Metrics.DepthFrameHeight];

            var pinnedMappedArry = System.Runtime.InteropServices.GCHandle.Alloc(data.MappedDepthToCameraSpacePixels, System.Runtime.InteropServices.GCHandleType.Pinned);

            try
            {
                IntPtr depthPointer = pinnedDepthArray.AddrOfPinnedObject();
                IntPtr mappedCameraSpacePointer = pinnedMappedArry.AddrOfPinnedObject();
                mapper.MapDepthFrameToCameraSpaceUsingIntPtr(depthPointer, (uint)Kinect2Metrics.DepthBufferLength, mappedCameraSpacePointer, (uint)Kinect2Metrics.MappedDepthToCameraBufferLength);
            }
            finally
            {
                pinnedMappedArry.Free();
                pinnedDepthArray.Free();
            }
        }
        
        public static void MapDepthFrameToColorSpace(CoordinateMapper mapper, KinectData data)
        {
            if (data.DepthPixels == null) return;

            var pinnedArray = System.Runtime.InteropServices.GCHandle.Alloc(data.DepthPixels, System.Runtime.InteropServices.GCHandleType.Pinned);

            data.MappedDepthToColorPixels = new ColorSpacePoint[Kinect2Metrics.DepthFrameWidth * Kinect2Metrics.DepthFrameHeight];

            var pinnedMappedArry = System.Runtime.InteropServices.GCHandle.Alloc(data.MappedDepthToColorPixels, System.Runtime.InteropServices.GCHandleType.Pinned);

            try
            {
                IntPtr depthPointer = pinnedArray.AddrOfPinnedObject();
                IntPtr mappedIRPointer = pinnedMappedArry.AddrOfPinnedObject();
                mapper.MapDepthFrameToColorSpaceUsingIntPtr(depthPointer, (uint)Kinect2Metrics.DepthBufferLength, mappedIRPointer, (uint)Kinect2Metrics.MappedDepthToColorBufferLength);
            }
            finally
            {
                pinnedArray.Free();
                pinnedMappedArry.Free();
            }
        }
    }
}
