using Microsoft.Kinect;

namespace Project_Recon
{
    /// <summary>
    /// The first part of the swipe right gesture
    /// </summary>
    public class SwipeUpSegment1: IRelativeGestureSegment
    {
        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            // //left hand in front of left Shoulder
            if (skeleton != null)
            {
                // right hand in front of right elbow
                if (skeleton.Joints[JointType.HandRight].Position.Z < skeleton.Joints[JointType.ElbowRight].Position.Z)
                {
                    // right hand below shoulder height but above hip height
                    if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.Head].Position.Y && skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.HipCenter].Position.Y)
                    {
                        // right hand right of right shoulder
                        if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ShoulderRight].Position.X)
                        {
                            return GesturePartResult.Succeed;
                        }
                        return GesturePartResult.Pausing;
                    }
                    return GesturePartResult.Fail;
                }
            }

            // Debug.WriteLine("GesturePart 0 - left hand in front of left Shoulder - FAIL");
            return GesturePartResult.Fail;
        }
    }

    /// <summary>
    /// The second part of the swipe right gesture
    /// </summary>
    public class SwipeUpSegment2 : IRelativeGestureSegment
    {
        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                // right hand in front of right shoulder
                if (skeleton.Joints[JointType.HandRight].Position.Z < skeleton.Joints[JointType.ShoulderRight].Position.Z)
                {
                    // right hand above right shoulder
                    if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ShoulderRight].Position.Y)
                    {
                        // right hand right of right shoulder
                        if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ShoulderRight].Position.X)
                        {
                            return GesturePartResult.Succeed;
                        }
                        return GesturePartResult.Pausing;
                    }
                    return GesturePartResult.Fail;
                }
            }

            // Debug.WriteLine("GesturePart 1 - left hand in front of left Shoulder - FAIL");
            return GesturePartResult.Fail;
        }
    }

    /// <summary>
    /// The third part of the swipe right gesture
    /// </summary>
    public class SwipeUpSegment3 : IRelativeGestureSegment
    {
        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                // //Right hand in front of right shoulder
                if (skeleton.Joints[JointType.HandRight].Position.Z < skeleton.Joints[JointType.ShoulderRight].Position.Z)
                {
                    // right hand above head
                    if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.Head].Position.Y)
                    {
                        // right hand right of right shoulder
                        if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ShoulderRight].Position.X)
                        {
                            return GesturePartResult.Succeed;
                        }
                        return GesturePartResult.Pausing;
                    }

                    // Debug.WriteLine("GesturePart 2 - right hand below shoulder height but above hip height - FAIL");
                    return GesturePartResult.Fail;
                }
            }
            return GesturePartResult.Fail;
        }
    }
}
