using System.ComponentModel;
using System;
using UnityEngine;
using XRRemote;
using System.Runtime.InteropServices;
using UnityEngine.XR.ARFoundation;

namespace KlakNDI_Test.Assets.Scripts.ObjectSerializationExtension 
{
    [Serializable]
    public partial class RemotePacket
    {
        public CameraFrameEvent cameraFrame;
        // todo make frameInfo and timestamp their own object classes
        public int frameInfo;
        public long? timestamp;
        public int bytesSent;
        // public FaceInfo face;
        // public Pose trackedPose;
        public PlanesInfo planesInfo;
        // // public HumanBodyInfo humanBody;
    }

    [Serializable]
    public class PlanesInfo
    {
        public XRPlane[] added;
        public XRPlane[] updated;
        public XRPlane[] removed;

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("PlanesInfo");
            foreach (var f in added)
            {
                sb.AppendLine($"added: {f}");
            }
            foreach (var f in updated)
            {
                sb.AppendLine($"updated: {f}");
            }
            foreach (var f in removed)
            {
                sb.AppendLine($"removed: {f}");
            }
            return sb.ToString();
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct XRPlane : IEquatable<XRPlane>
    {
        public XRRemote.TrackableId trackableId;
        public XRRemote.Pose pose;
        public float3 center;
        public float3 centerInPlaneSpace;
        public float3 normal;
        public int trackingState;
        public float vertexChangedThreshold;
        public float2[] boundary;
        public float2 size;
        public bool isSubsumed;

        public XRPlane(ARPlane arPlane)
        {
            trackableId = arPlane.trackableId;
            pose = XRRemote.Pose.FromTransform(arPlane.transform);
            center = new float3(arPlane.center);
            centerInPlaneSpace = new float3(arPlane.centerInPlaneSpace);
            normal = new float3(arPlane.normal);
            trackingState = (int)arPlane.trackingState;
            vertexChangedThreshold = arPlane.vertexChangedThreshold;
            size = new float2(arPlane.size);
            isSubsumed = (arPlane.subsumedBy != null);

            //Save the boundary array as a float2 array
            Vector2[] boundaryPoints = arPlane.boundary.ToArray();
            boundary = new float2[boundaryPoints.Length];
            for (int j = 0; j < boundaryPoints.Length; j++) {
                boundary[j] = new float2(arPlane.boundary[j]);
            }
        }

        public bool Equals(XRPlane o)
        {
            return trackableId.Equals(o.trackableId);
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append($"[XRPlane] id: {trackableId} ");
            sb.Append($"pose: {pose} ");
            sb.Append($"center: {center} ");
            sb.Append($"state: {trackingState} ");
            sb.Append($"vertexChangedThreshold: {vertexChangedThreshold} ");
            sb.Append($"boundary: {boundary} ");

            return sb.ToString();
        }
    }
}

