using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;

namespace Project_Recon
{
    public class DetectGesture
    {
        public int MinimalPeriodBetweenGestures { get; set; }
        int punchprob = 0;

        readonly List<StoreGesture> posList = new List<StoreGesture>();

        public event Action<string> OnDetected;

        DateTime lastGestureDate = DateTime.Now;

        // Number of recorded positions
        private int maxSize;

        public DetectGesture(int maxSize)
        {
            // TODO: Complete member initialization
            this.maxSize = maxSize; ;
            MinimalPeriodBetweenGestures = 0;
        }

        public List<StoreGesture> PosList
        {
            get { return posList; }
        }

        public int MaxSize
        {
            get { return maxSize; }
        }

        public virtual void addPosition(SkeletonPoint position, KinectSensor sensor)
        {
            StoreGesture newEntry = new StoreGesture { Position = new Vector3(position.X, position.Y, position.Z), Time = DateTime.Now };

            // Remove too old positions
            if (posList.Count > MaxSize)
            {
                StoreGesture entryToRemove = PosList[0];
                PosList.Remove(entryToRemove);
            }
            // Add new position
            posList.Add(newEntry);

            //LookForGesture();
        }
        //protected void LookForGesture();

        protected void RaiseGestureDetected(string gesture)
        {
            // Too close?
            if (DateTime.Now.Subtract(lastGestureDate).TotalMilliseconds > MinimalPeriodBetweenGestures)
            {
                if (OnDetected != null)
                    OnDetected(gesture);

                lastGestureDate = DateTime.Now;
            }
            PosList.Clear();
        }

        public String Detect(List<StoreGesture> reference1, List<StoreGesture> reference2)
        {
            if (PosList.Count == MaxSize)
            {
                //Punch
                for (int i = 0; i < MaxSize; i++)
                {
                    float a = PosList[i].Position.Y;
                    float r1 = reference1[i].Position.Y;
                    float r2 = reference2[i].Position.Y;

                    float diff1 = Math.Abs(a - r1);
                    float diff2 = Math.Abs(a - r2);

                    double magnitude1 = Math.Sqrt(Math.Pow((reference2[i].Position - PosList[i].Position).X, 2) +
                        Math.Pow((reference2[i].Position - PosList[i].Position).Y, 2) +
                        Math.Pow((reference2[i].Position - PosList[i].Position).Z, 2));

                    double magnitude2 = Math.Sqrt(Math.Pow((reference2[i].Position - reference1[i].Position).X, 2) +
                        Math.Pow((reference2[i].Position - reference1[i].Position).Y, 2) +
                        Math.Pow((reference2[i].Position - reference1[i].Position).Z, 2));

                    double angle = Math.Acos((Vector3.Dot((reference2[i].Position - PosList[i].Position), (reference2[i].Position - reference1[i].Position))) /
                        magnitude1 * magnitude2);

                    //r1 spine and r2 shoulder

                    if (i < 14 && diff1 < diff2)
                    {
                        punchprob++;
                    }

                    if (i <= 19 && diff1 > diff2)
                    {
                        punchprob++;
                    }

                    if (angle > 0.48 * Math.PI)
                    {
                        punchprob = 20;
                    }
                }

            }

            if (punchprob > 14)
            {
                punchprob = 0;
                return "punch";
            }
            else
            {
                punchprob = 0;
                return "No Move Detected";
            }
        }
    }
}