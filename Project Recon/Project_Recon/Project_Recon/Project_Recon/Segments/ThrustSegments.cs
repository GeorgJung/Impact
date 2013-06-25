using Microsoft.Kinect;

namespace Project_Recon
{
    public class ThrustSegment1 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                // Right or Left Knee above Center hips
                if (skeleton.Joints[JointType.KneeLeft].Position.Y > skeleton.Joints[JointType.HipCenter].Position.Y 
                    || skeleton.Joints[JointType.KneeRight].Position.Y > skeleton.Joints[JointType.HipCenter].Position.Y)
                {
                    //Debug.WriteLine("Zoom 0 - Right hand in front of right shoudler - PASS");

                    // Ankle below hips
                    if (skeleton.Joints[JointType.AnkleLeft].Position.Y < skeleton.Joints[JointType.HipCenter].Position.Y
                    || skeleton.Joints[JointType.AnkleRight].Position.Y < skeleton.Joints[JointType.HipCenter].Position.Y)
                    {
                        // Hands between shoulders
                        if (skeleton.Joints[JointType.KneeLeft].Position.Y > skeleton.Joints[JointType.Spine].Position.Y 
                        || skeleton.Joints[JointType.KneeRight].Position.Y > skeleton.Joints[JointType.Spine].Position.Y)
                        {
                            return GesturePartResult.Succeed;
                        }

                        return GesturePartResult.Pausing;
                    }

                    return GesturePartResult.Fail;
                }
            }

            return GesturePartResult.Fail;
        }
    }

    public class ThrustSegment2 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                // Right or Left Knee above Center hips
                if (skeleton.Joints[JointType.KneeLeft].Position.Y > skeleton.Joints[JointType.HipCenter].Position.Y
                    || skeleton.Joints[JointType.KneeRight].Position.Y > skeleton.Joints[JointType.HipCenter].Position.Y)
                {
                    //Debug.WriteLine("Zoom 0 - Right hand in front of right shoudler - PASS");

                    // Ankle below hips
                    if (skeleton.Joints[JointType.AnkleLeft].Position.Y < skeleton.Joints[JointType.HipCenter].Position.Y
                    || skeleton.Joints[JointType.AnkleRight].Position.Y < skeleton.Joints[JointType.HipCenter].Position.Y)
                    {
                        // Knee Below Spine
                        if (skeleton.Joints[JointType.KneeLeft].Position.Y < skeleton.Joints[JointType.ElbowLeft].Position.Y
                        || skeleton.Joints[JointType.KneeRight].Position.Y < skeleton.Joints[JointType.ElbowRight].Position.Y)
                        {
                            return GesturePartResult.Succeed;
                        }

                        return GesturePartResult.Pausing;
                    }

                    return GesturePartResult.Fail;
                }
            }
            return GesturePartResult.Fail;
        }
    }

    public class ThrustSegment3 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                // Right and Left Hand in front of Shoulders
                if (skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ElbowLeft].Position.Z && skeleton.Joints[JointType.HandRight].Position.Z < skeleton.Joints[JointType.ElbowRight].Position.Z)
                {
                    // Hands between shoulder and hip
                    if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.ShoulderCenter].Position.Y && skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.HipCenter].Position.Y &&
                        skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.ShoulderCenter].Position.Y && skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipCenter].Position.Y)
                    {
                        // Hands outside elbows
                        if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ElbowRight].Position.X && skeleton.Joints[JointType.HandLeft].Position.X < skeleton.Joints[JointType.ElbowLeft].Position.X)
                        {
                            return GesturePartResult.Succeed;
                        }

                        return GesturePartResult.Pausing;
                    }

                    return GesturePartResult.Fail;
                }
            }
            return GesturePartResult.Fail;
        }
    }
}
