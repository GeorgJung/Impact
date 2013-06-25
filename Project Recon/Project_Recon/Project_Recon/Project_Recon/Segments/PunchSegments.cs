using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;

namespace Project_Recon
{
    public class PunchSegment1 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                //Create Vector
                Vector3 Lefthandelbow = new Vector3(skeleton.Joints[JointType.HandLeft].Position.X - skeleton.Joints[JointType.ElbowLeft].Position.X,
                    skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.ElbowLeft].Position.Y,
                    skeleton.Joints[JointType.HandLeft].Position.Z - skeleton.Joints[JointType.ElbowLeft].Position.Z);

                Vector3 Righthandelbow = new Vector3(skeleton.Joints[JointType.HandRight].Position.X - skeleton.Joints[JointType.ElbowRight].Position.X,
                    skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.ElbowRight].Position.Y,
                    skeleton.Joints[JointType.HandRight].Position.Z - skeleton.Joints[JointType.ElbowRight].Position.Z);

                Vector3 Rightshoulderelbow = new Vector3(skeleton.Joints[JointType.ShoulderRight].Position.X - skeleton.Joints[JointType.ElbowRight].Position.X,
                    skeleton.Joints[JointType.ShoulderRight].Position.Y - skeleton.Joints[JointType.ShoulderRight].Position.Y,
                    skeleton.Joints[JointType.ShoulderRight].Position.Z - skeleton.Joints[JointType.ShoulderRight].Position.Z);

                Vector3 Leftshoulderelbow = new Vector3(skeleton.Joints[JointType.ShoulderLeft].Position.X - skeleton.Joints[JointType.ElbowLeft].Position.X,
                    skeleton.Joints[JointType.ShoulderLeft].Position.Y - skeleton.Joints[JointType.ShoulderLeft].Position.Y,
                    skeleton.Joints[JointType.ShoulderLeft].Position.Z - skeleton.Joints[JointType.ShoulderLeft].Position.Z);

                // Calculate the Cross product of all
                var leftDot = Vector3.Dot(Lefthandelbow, Leftshoulderelbow);
                var rightDot = Vector3.Dot(Righthandelbow, Rightshoulderelbow);

                // Calculate the Magnitude
                var LeftMag1 = Math.Sqrt(Math.Pow(Lefthandelbow.X, 2) + Math.Pow(Lefthandelbow.Y, 2) + Math.Pow(Lefthandelbow.Z, 2));
                var LeftMag2 = Math.Sqrt(Math.Pow(Leftshoulderelbow.X, 2) + Math.Pow(Leftshoulderelbow.Y, 2) + Math.Pow(Leftshoulderelbow.Z, 2));

                var RightMag1 = Math.Sqrt(Math.Pow(Righthandelbow.X, 2) + Math.Pow(Righthandelbow.Y, 2) + Math.Pow(Righthandelbow.Z, 2));
                var RightMag2 = Math.Sqrt(Math.Pow(Rightshoulderelbow.X, 2) + Math.Pow(Rightshoulderelbow.Y, 2) + Math.Pow(Rightshoulderelbow.Z, 2)); 

                //Calcualte Angle
                float LeftAngle = MathHelper.ToDegrees((float)Math.Acos(leftDot / (LeftMag1 * LeftMag2)));
                float RightAngle = MathHelper.ToDegrees((float)Math.Acos(rightDot / (RightMag1 * RightMag2)));

                // Right or Left Hand below Elbow
                if ((skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.ElbowLeft].Position.Y) || (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.ElbowRight].Position.Y))
                {
                    //Debug.WriteLine("Zoom 0 - Right hand in front of right shoudler - PASS");

                    // Elbow below shoulders
                    if ((skeleton.Joints[JointType.ElbowRight].Position.Y < skeleton.Joints[JointType.ShoulderCenter].Position.Y) || (skeleton.Joints[JointType.ElbowLeft].Position.Y < skeleton.Joints[JointType.ShoulderCenter].Position.Y))
                    {
                        // Angle less than 45
                        if ((LeftAngle < 90 || RightAngle < 90) || ((LeftAngle < 90 || RightAngle < 90) && (skeleton.Joints[JointType.HandRight].Position.Z < skeleton.Joints[JointType.ShoulderRight].Position.Z
                            || skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ShoulderLeft].Position.Z)))
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

    public class PunchSegment2 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                //Create Vector
                Vector3 Lefthandelbow = new Vector3(skeleton.Joints[JointType.HandLeft].Position.X - skeleton.Joints[JointType.ElbowLeft].Position.X,
                    skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.ElbowLeft].Position.Y,
                    skeleton.Joints[JointType.HandLeft].Position.Z - skeleton.Joints[JointType.ElbowLeft].Position.Z);

                Vector3 Righthandelbow = new Vector3(skeleton.Joints[JointType.HandRight].Position.X - skeleton.Joints[JointType.ElbowRight].Position.X,
                    skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.ElbowRight].Position.Y,
                    skeleton.Joints[JointType.HandRight].Position.Z - skeleton.Joints[JointType.ElbowRight].Position.Z);

                Vector3 Rightshoulderelbow = new Vector3(skeleton.Joints[JointType.ShoulderRight].Position.X - skeleton.Joints[JointType.ElbowRight].Position.X,
                    skeleton.Joints[JointType.ShoulderRight].Position.Y - skeleton.Joints[JointType.ShoulderRight].Position.Y,
                    skeleton.Joints[JointType.ShoulderRight].Position.Z - skeleton.Joints[JointType.ShoulderRight].Position.Z);

                Vector3 Leftshoulderelbow = new Vector3(skeleton.Joints[JointType.ShoulderLeft].Position.X - skeleton.Joints[JointType.ElbowLeft].Position.X,
                    skeleton.Joints[JointType.ShoulderLeft].Position.Y - skeleton.Joints[JointType.ShoulderLeft].Position.Y,
                    skeleton.Joints[JointType.ShoulderLeft].Position.Z - skeleton.Joints[JointType.ShoulderLeft].Position.Z);

                // Calculate the Cross product of all
                var leftDot = Vector3.Dot(Lefthandelbow, Leftshoulderelbow);
                var rightDot = Vector3.Dot(Righthandelbow, Rightshoulderelbow);

                // Calculate the Magnitude
                var LeftMag1 = Math.Sqrt(Math.Pow(Lefthandelbow.X, 2) + Math.Pow(Lefthandelbow.Y, 2) + Math.Pow(Lefthandelbow.Z, 2));
                var LeftMag2 = Math.Sqrt(Math.Pow(Leftshoulderelbow.X, 2) + Math.Pow(Leftshoulderelbow.Y, 2) + Math.Pow(Leftshoulderelbow.Z, 2));

                var RightMag1 = Math.Sqrt(Math.Pow(Righthandelbow.X, 2) + Math.Pow(Righthandelbow.Y, 2) + Math.Pow(Righthandelbow.Z, 2));
                var RightMag2 = Math.Sqrt(Math.Pow(Rightshoulderelbow.X, 2) + Math.Pow(Rightshoulderelbow.Y, 2) + Math.Pow(Rightshoulderelbow.Z, 2));

                //Calcualte Angle
                float LeftAngle = MathHelper.ToDegrees((float)Math.Acos(leftDot / (LeftMag1 * LeftMag2)));
                float RightAngle = MathHelper.ToDegrees((float)Math.Acos(rightDot / (RightMag1 * RightMag2)));

                // Right or Left Hand above Elbow
                if ((skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ElbowLeft].Position.Y) || (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ElbowRight].Position.Y))
                {
                    //Debug.WriteLine("Zoom 0 - Right hand in front of right shoudler - PASS");

                    // Hand below shoulders
                    if ((skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.ShoulderCenter].Position.Y) || (skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.ShoulderCenter].Position.Y))
                    {
                        // Angle less than 45
                        if (((LeftAngle > 90 && LeftAngle < 135) || (RightAngle > 90 && RightAngle < 135)) ||
                            (((LeftAngle > 90 && LeftAngle < 135) || (RightAngle > 90 && RightAngle < 135)) && (skeleton.Joints[JointType.HandRight].Position.Z < skeleton.Joints[JointType.ShoulderRight].Position.Z
                            || skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ShoulderLeft].Position.Z)))
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

    public class PunchSegment3 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                //Create Vector
                Vector3 Lefthandelbow = new Vector3(skeleton.Joints[JointType.HandLeft].Position.X - skeleton.Joints[JointType.ElbowLeft].Position.X,
                    skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.ElbowLeft].Position.Y,
                    skeleton.Joints[JointType.HandLeft].Position.Z - skeleton.Joints[JointType.ElbowLeft].Position.Z);

                Vector3 Righthandelbow = new Vector3(skeleton.Joints[JointType.HandRight].Position.X - skeleton.Joints[JointType.ElbowRight].Position.X,
                    skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.ElbowRight].Position.Y,
                    skeleton.Joints[JointType.HandRight].Position.Z - skeleton.Joints[JointType.ElbowRight].Position.Z);

                Vector3 Rightshoulderelbow = new Vector3(skeleton.Joints[JointType.ShoulderRight].Position.X - skeleton.Joints[JointType.ElbowRight].Position.X,
                    skeleton.Joints[JointType.ShoulderRight].Position.Y - skeleton.Joints[JointType.ShoulderRight].Position.Y,
                    skeleton.Joints[JointType.ShoulderRight].Position.Z - skeleton.Joints[JointType.ShoulderRight].Position.Z);

                Vector3 Leftshoulderelbow = new Vector3(skeleton.Joints[JointType.ShoulderLeft].Position.X - skeleton.Joints[JointType.ElbowLeft].Position.X,
                    skeleton.Joints[JointType.ShoulderLeft].Position.Y - skeleton.Joints[JointType.ShoulderLeft].Position.Y,
                    skeleton.Joints[JointType.ShoulderLeft].Position.Z - skeleton.Joints[JointType.ShoulderLeft].Position.Z);

                // Calculate the Cross product of all
                var leftDot = Vector3.Dot(Lefthandelbow, Leftshoulderelbow);
                var rightDot = Vector3.Dot(Righthandelbow, Rightshoulderelbow);

                // Calculate the Magnitude
                var LeftMag1 = Math.Sqrt(Math.Pow(Lefthandelbow.X, 2) + Math.Pow(Lefthandelbow.Y, 2) + Math.Pow(Lefthandelbow.Z, 2));
                var LeftMag2 = Math.Sqrt(Math.Pow(Leftshoulderelbow.X, 2) + Math.Pow(Leftshoulderelbow.Y, 2) + Math.Pow(Leftshoulderelbow.Z, 2));

                var RightMag1 = Math.Sqrt(Math.Pow(Righthandelbow.X, 2) + Math.Pow(Righthandelbow.Y, 2) + Math.Pow(Righthandelbow.Z, 2));
                var RightMag2 = Math.Sqrt(Math.Pow(Rightshoulderelbow.X, 2) + Math.Pow(Rightshoulderelbow.Y, 2) + Math.Pow(Rightshoulderelbow.Z, 2));

                //Calcualte Angle
                float LeftAngle = MathHelper.ToDegrees((float)Math.Acos(leftDot / (LeftMag1 * LeftMag2)));
                float RightAngle = MathHelper.ToDegrees((float)Math.Acos(rightDot / (RightMag1 * RightMag2)));

                // Right and Left Hand in front of Shoulders
                // Right or Left Hand below Elbow
                if ((skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ElbowLeft].Position.Y) || (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ElbowRight].Position.Y))
                {
                    //Debug.WriteLine("Zoom 0 - Right hand in front of right shoudler - PASS");

                    // Elbow below shoulders
                    if ((skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ShoulderCenter].Position.Y) || (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ShoulderCenter].Position.Y))
                    {
                        // Angle less than 45
                        if ((LeftAngle > 135 || RightAngle > 135) || ((LeftAngle > 135 || RightAngle > 135) && (skeleton.Joints[JointType.HandRight].Position.Z < skeleton.Joints[JointType.ShoulderRight].Position.Z
                            || skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ShoulderLeft].Position.Z)))
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
