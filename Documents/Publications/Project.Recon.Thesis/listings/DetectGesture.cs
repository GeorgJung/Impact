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
        }
		
		public String Detect(List<StoreGesture> reference1, List<StoreGesture> reference2)
        {
            if (PosList.Count == MaxSize)
            {
                //Punch
                //r1 spine and r2 shoulder
                for (int i = 0; i < MaxSize; i++)
                {
                    float a = PosList[i].Position.Y;
                    float r1 = reference1[i].Position.Y;
                    float r2 = reference2[i].Position.Y;

                    float diff1 = Math.Abs(a - r1);
                    float diff2 = Math.Abs(a - r2);

                    float magnitude1 = (float)Math.Sqrt(Math.Pow((reference2[i].Position - PosList[i].Position).X, 2) +
                        Math.Pow((reference2[i].Position - PosList[i].Position).Y, 2) +
                        Math.Pow((reference2[i].Position - PosList[i].Position).Z, 2));

                    float magnitude2 = (float)Math.Sqrt(Math.Pow((reference2[i].Position - reference1[i].Position).X, 2) +
                        Math.Pow((reference2[i].Position - reference1[i].Position).Y, 2) +
                        Math.Pow((reference2[i].Position - reference1[i].Position).Z, 2));

                    float angle = (float)Math.Acos((Vector3.Dot((reference2[i].Position - PosList[i].Position), (reference2[i].Position - reference1[i].Position))) /
                        magnitude1 * magnitude2);

                    if (i < 11 && diff1 < diff2)
                    {
                        punchprob++;
                    }

                    if (i < 21 && i > 11 && diff1 > diff2)
                    {
                        punchprob++;
                    }

                    if (i > 21 && i < 30 && diff1 < diff2)
                    {
                        punchprob++;
                    }

                    //if (angle > 0.25f * Math.PI)
                    //{
                    //    punchprob = 30;
                    //}
                }

            }

            if (punchprob > 25)
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