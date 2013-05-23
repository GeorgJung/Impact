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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        client Client;

        KinectSensor kinect;

        byte[] colorData;
        Texture2D colorTex;


        String thetaYString;

        SpriteFont font;

        Skeleton[] rawSkeletons;
        Skeleton skeleton;
        Skeleton coachSkeleton;

        SkeletonPoint littleguy_lankle, littleguy_rankle,
            littleguy_lelbow, littleguy_relbow,
            littleguy_lfoot, littleguy_rfoot,
            littleguy_lhand, littleguy_rhand,
            littleguy_head, littleguy_chip,
            littleguy_lhip, littleguy_rhip,
            littleguy_lknee, littleguy_rknee,
            littleguy_cshoulder, littleguy_lshoulder,
            littleguy_rshoulder, littleguy_spine,
            littleguy_lwrist, littleguy_rwrist;
        SkeletonPoint Normal_joint;
        SkeletonPoint Normal2_joint;


        SkeletonPoint[] littleGuy;
        SkeletonPoint[] prevPositions;
        SkeletonPoint[] transGuy;

        Texture2D circleTex;
        Texture2D lineTex;
        Texture2D lineTexRed;
        Texture2D rightTex, leftTex;

        Vector3 TransValue;

        Matrix rotY;
        Matrix rotX;
        Matrix rotPoint;
        Matrix totalRot;

        double thetax; // angle between normal vector and Z axis
        double thetay; // angle between normal vector and Y axis

        Boolean keepcover, flip;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            kinect = KinectSensor.KinectSensors[0];

            kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

            kinect.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);

            kinect.SkeletonStream.Enable();

            kinect.Start();

            kinect.ElevationAngle = 0;

            colorData = new byte[640 * 480 * 4];
            colorTex = new Texture2D(GraphicsDevice, 640, 480);

            rawSkeletons = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength];

            prevPositions = new SkeletonPoint[20];
            littleGuy = new SkeletonPoint[20];
            transGuy = new SkeletonPoint[20];

            TransValue = new Vector3(0, 0, 0);

            keepcover = true;
            flip = false;

            thetaYString = "";

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            circleTex = Content.Load<Texture2D>("circle");
            lineTex = Content.Load<Texture2D>("4KWjQ");
            lineTexRed = Content.Load<Texture2D>("4KWjQR");
            rightTex = Content.Load<Texture2D>("richt");
            leftTex = Content.Load<Texture2D>("links");

            font = Content.Load<SpriteFont>("MainFont");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            var colorFrame = kinect.ColorStream.OpenNextFrame(0);
            if (colorFrame != null)
            {
                colorFrame.CopyPixelDataTo(colorData);
                colorFrame.Dispose();

                for (int i = 0; i < colorData.Length; i += 4)
                {
                    byte temp = colorData[i];
                    colorData[i] = colorData[i + 2];
                    colorData[i + 2] = temp;
                    colorData[i + 3] = 255;
                }

                GraphicsDevice.Textures[0] = null;
                colorTex.SetData(colorData);
            }

            var skelFrame = kinect.SkeletonStream.OpenNextFrame(0);
            if (skelFrame != null)
            {
                skelFrame.CopySkeletonDataTo(rawSkeletons);
                skelFrame.Dispose();

                skeleton = rawSkeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
                coachSkeleton = rawSkeletons.LastOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked && coachSkeleton != skeleton);
            }

            if (skeleton != null)
            {
                //Little Guy
                foreach (Joint joint in skeleton.Joints)
                {
                    //Ankle
                    if (joint.JointType == JointType.AnkleLeft)
                    {
                        littleguy_lankle = skeleton.Joints[JointType.AnkleLeft].Position;
                        littleGuy[14] = littleguy_lankle;

                        transGuy[14] = littleGuy[14];
                        transGuy[14].X = (float)((decimal)(littleGuy[14].X) + (decimal)(TransValue.X));
                        transGuy[14].Y = (float)((decimal)(littleGuy[14].Y) + (decimal)(TransValue.Y));
                        transGuy[14].Z = (float)((decimal)(littleGuy[14].Z));

                        prevPositions[14] = joint.Position;
                    }
                    if (joint.JointType == JointType.AnkleRight)
                    {
                        littleguy_rankle = skeleton.Joints[JointType.AnkleRight].Position;
                        littleGuy[18] = littleguy_rankle;

                        transGuy[18] = littleGuy[18];
                        transGuy[18].X = (float)((decimal)(littleGuy[18].X) + (decimal)(TransValue.X));
                        transGuy[18].Y = (float)((decimal)(littleGuy[18].Y) + (decimal)(TransValue.Y));
                        transGuy[18].Z = (float)((decimal)(littleGuy[18].Z));

                        prevPositions[18] = joint.Position;
                    }

                    //Elbow
                    if (joint.JointType == JointType.ElbowLeft)
                    {
                        littleguy_lelbow = skeleton.Joints[JointType.ElbowLeft].Position;
                        littleGuy[5] = littleguy_lelbow;

                        transGuy[5] = littleGuy[5];
                        transGuy[5].X = (float)((decimal)(littleGuy[5].X) + (decimal)(TransValue.X));
                        transGuy[5].Y = (float)((decimal)(littleGuy[5].Y) + (decimal)(TransValue.Y));
                        transGuy[5].Z = (float)((decimal)(littleGuy[5].Z));

                        prevPositions[5] = joint.Position;
                    }
                    if (joint.JointType == JointType.ElbowRight)
                    {
                        littleguy_relbow = skeleton.Joints[JointType.ElbowRight].Position;
                        littleGuy[9] = littleguy_relbow;

                        transGuy[9] = littleGuy[9];
                        transGuy[9].X = (float)((decimal)(littleGuy[9].X) + (decimal)(TransValue.X));
                        transGuy[9].Y = (float)((decimal)(littleGuy[9].Y) + (decimal)(TransValue.Y));
                        transGuy[9].Z = (float)((decimal)(littleGuy[9].Z));

                        prevPositions[9] = joint.Position;
                    }

                    //Foot
                    if (joint.JointType == JointType.FootLeft)
                    {
                        littleguy_lfoot = skeleton.Joints[JointType.FootLeft].Position;
                        littleGuy[15] = littleguy_lfoot;

                        transGuy[15] = littleGuy[15];
                        transGuy[15].X = (float)((decimal)(littleGuy[15].X) + (decimal)(TransValue.X));
                        transGuy[15].Y = (float)((decimal)(littleGuy[15].Y) + (decimal)(TransValue.Y));
                        transGuy[15].Z = (float)((decimal)(littleGuy[15].Z));

                        prevPositions[15] = joint.Position;
                    }
                    if (joint.JointType == JointType.FootRight)
                    {
                        littleguy_rfoot = skeleton.Joints[JointType.FootRight].Position;
                        littleGuy[19] = littleguy_rfoot;

                        transGuy[19] = littleGuy[19];
                        transGuy[19].X = (float)((decimal)(littleGuy[19].X) + (decimal)(TransValue.X));
                        transGuy[19].Y = (float)((decimal)(littleGuy[19].Y) + (decimal)(TransValue.Y));
                        transGuy[19].Z = (float)((decimal)(littleGuy[19].Z));

                        prevPositions[19] = joint.Position;
                    }

                    //Hand
                    if (joint.JointType == JointType.HandLeft)
                    {
                        littleguy_lhand = skeleton.Joints[JointType.HandLeft].Position;
                        littleGuy[7] = littleguy_lhand;

                        transGuy[7] = littleGuy[17];
                        transGuy[7].X = (float)((decimal)(littleGuy[7].X) + (decimal)(TransValue.X));
                        transGuy[7].Y = (float)((decimal)(littleGuy[7].Y) + (decimal)(TransValue.Y));
                        transGuy[7].Z = (float)((decimal)(littleGuy[7].Z));

                        prevPositions[7] = joint.Position;
                    }
                    if (joint.JointType == JointType.HandRight)
                    {
                        littleguy_rhand = skeleton.Joints[JointType.HandRight].Position;
                        littleGuy[11] = littleguy_rhand;

                        transGuy[11] = littleGuy[11];
                        transGuy[11].X = (float)((decimal)(littleGuy[11].X) + (decimal)(TransValue.X));
                        transGuy[11].Y = (float)((decimal)(littleGuy[11].Y) + (decimal)(TransValue.Y));
                        transGuy[11].Z = (float)((decimal)(littleGuy[11].Z));

                        prevPositions[11] = joint.Position;
                    }

                    //Head
                    if (joint.JointType == JointType.Head)
                    {
                        littleguy_head = skeleton.Joints[JointType.Head].Position;
                        littleGuy[3] = littleguy_head;

                        transGuy[3] = littleGuy[3];
                        transGuy[3].X = (float)((decimal)(littleGuy[3].X) + (decimal)(TransValue.X));
                        transGuy[3].Y = (float)((decimal)(littleGuy[3].Y) + (decimal)(TransValue.Y));
                        transGuy[3].Z = (float)((decimal)(littleGuy[3].Z));

                        prevPositions[3] = joint.Position;
                    }

                    //Hip
                    if (joint.JointType == JointType.HipCenter)
                    {
                        littleguy_chip = skeleton.Joints[JointType.HipCenter].Position;
                        littleGuy[0] = littleguy_chip;

                        TransValue.X = littleguy_chip.X * (-1.0f);
                        TransValue.Y = littleguy_chip.Y * (-1.0f);
                        TransValue.Z = littleguy_chip.Z * (-1.0f);

                        transGuy[0] = littleGuy[0];
                        transGuy[0].X = (float)((decimal)(littleGuy[0].X) + (decimal)(TransValue.X));
                        transGuy[0].Y = (float)((decimal)(littleGuy[0].Y) + (decimal)(TransValue.Y));
                        transGuy[0].Z = (float)((decimal)(littleGuy[0].Z));

                        prevPositions[0] = joint.Position;
                    }

                    if (joint.JointType == JointType.HipLeft)
                    {
                        littleguy_lhip = skeleton.Joints[JointType.HipLeft].Position;
                        littleGuy[12] = littleguy_lhip;

                        transGuy[12] = littleGuy[12];
                        transGuy[12].X = (float)((decimal)(littleGuy[12].X) + (decimal)(TransValue.X));
                        transGuy[12].Y = (float)((decimal)(littleGuy[12].Y) + (decimal)(TransValue.Y));
                        transGuy[12].Z = (float)((decimal)(littleGuy[12].Z));

                        prevPositions[12] = joint.Position;
                    }
                    if (joint.JointType == JointType.HipRight)
                    {
                        littleguy_rhip = skeleton.Joints[JointType.HipRight].Position;
                        littleGuy[16] = littleguy_rhip;

                        transGuy[16] = littleGuy[16];
                        transGuy[16].X = (float)((decimal)(littleGuy[16].X) + (decimal)(TransValue.X));
                        transGuy[16].Y = (float)((decimal)(littleGuy[16].Y) + (decimal)(TransValue.Y));
                        transGuy[16].Z = (float)((decimal)(littleGuy[16].Z));

                        prevPositions[16] = joint.Position;
                    }

                    //Knee
                    if (joint.JointType == JointType.KneeLeft)
                    {
                        littleguy_lknee = skeleton.Joints[JointType.KneeLeft].Position;
                        littleGuy[13] = littleguy_lknee;

                        transGuy[13] = littleGuy[13];
                        transGuy[13].X = (float)((decimal)(littleGuy[13].X) + (decimal)(TransValue.X));
                        transGuy[13].Y = (float)((decimal)(littleGuy[13].Y) + (decimal)(TransValue.Y));
                        transGuy[13].Z = (float)((decimal)(littleGuy[13].Z));

                        prevPositions[13] = joint.Position;
                    }
                    if (joint.JointType == JointType.KneeRight)
                    {
                        littleguy_rknee = skeleton.Joints[JointType.KneeRight].Position;
                        littleGuy[17] = littleguy_rknee;

                        transGuy[17] = littleGuy[17];
                        transGuy[17].X = (float)((decimal)(littleGuy[17].X) + (decimal)(TransValue.X));
                        transGuy[17].Y = (float)((decimal)(littleGuy[17].Y) + (decimal)(TransValue.Y));
                        transGuy[17].Z = (float)((decimal)(littleGuy[17].Z));

                        prevPositions[17] = joint.Position;
                    }

                    //Shoulder
                    if (joint.JointType == JointType.ShoulderCenter)
                    {
                        littleguy_cshoulder = skeleton.Joints[JointType.ShoulderCenter].Position;
                        littleGuy[2] = littleguy_cshoulder;

                        transGuy[2] = littleGuy[2];
                        transGuy[2].X = (float)((decimal)(littleGuy[2].X) + (decimal)(TransValue.X));
                        transGuy[2].Y = (float)((decimal)(littleGuy[2].Y) + (decimal)(TransValue.Y));
                        transGuy[2].Z = (float)((decimal)(littleGuy[2].Z));

                        prevPositions[2] = joint.Position;
                    }
                    if (joint.JointType == JointType.ShoulderLeft)
                    {
                        littleguy_lshoulder = skeleton.Joints[JointType.ShoulderLeft].Position;
                        littleGuy[4] = littleguy_lshoulder;

                        transGuy[4] = littleGuy[4];
                        transGuy[4].X = (float)((decimal)(littleGuy[4].X) + (decimal)(TransValue.X));
                        transGuy[4].Y = (float)((decimal)(littleGuy[4].Y) + (decimal)(TransValue.Y));
                        transGuy[4].Z = (float)((decimal)(littleGuy[4].Z));

                        prevPositions[4] = joint.Position;
                    }
                    if (joint.JointType == JointType.ShoulderRight)
                    {
                        littleguy_rshoulder = skeleton.Joints[JointType.ShoulderRight].Position;
                        littleGuy[8] = littleguy_rshoulder;

                        transGuy[8] = littleGuy[8];
                        transGuy[8].X = (float)((decimal)(littleGuy[8].X) + (decimal)(TransValue.X));
                        transGuy[8].Y = (float)((decimal)(littleGuy[8].Y) + (decimal)(TransValue.Y));
                        transGuy[8].Z = (float)((decimal)(littleGuy[8].Z));

                        prevPositions[8] = joint.Position;
                    }

                    //Spine
                    if (joint.JointType == JointType.Spine)
                    {
                        littleguy_spine = skeleton.Joints[JointType.Spine].Position;
                        littleGuy[1] = littleguy_spine;

                        transGuy[1] = littleGuy[1];
                        transGuy[1].X = (float)((decimal)(littleGuy[1].X) + (decimal)(TransValue.X));
                        transGuy[1].Y = (float)((decimal)(littleGuy[1].Y) + (decimal)(TransValue.Y));
                        transGuy[1].Z = (float)((decimal)(littleGuy[1].Z));

                        prevPositions[1] = joint.Position;
                    }

                    //Wrist
                    if (joint.JointType == JointType.WristLeft)
                    {
                        littleguy_lwrist = skeleton.Joints[JointType.WristLeft].Position;
                        littleGuy[6] = littleguy_lwrist;

                        transGuy[6] = littleGuy[6];
                        transGuy[6].X = (float)((decimal)(littleGuy[6].X) + (decimal)(TransValue.X));
                        transGuy[6].Y = (float)((decimal)(littleGuy[6].Y) + (decimal)(TransValue.Y));
                        transGuy[6].Z = (float)((decimal)(littleGuy[6].Z));

                        prevPositions[6] = joint.Position;
                    }
                    if (joint.JointType == JointType.WristRight)
                    {
                        littleguy_rwrist = skeleton.Joints[JointType.WristRight].Position;
                        littleGuy[10] = littleguy_rwrist;

                        transGuy[10] = littleGuy[10];
                        transGuy[10].X = (float)((decimal)(littleGuy[10].X) + (decimal)(TransValue.X));
                        transGuy[10].Y = (float)((decimal)(littleGuy[10].Y) + (decimal)(TransValue.Y));
                        transGuy[10].Z = (float)((decimal)(littleGuy[10].Z));

                        prevPositions[10] = joint.Position;
                    }
                }

                if (!flip)
                {
                    ////OLD CODE
                    //Vector3 rotXAxis = new Vector3(0.5f, 0f, 0f);
                    //Vector3 rotYAxis = new Vector3(0f, 0.5f, 0f);
                    ////Calculate Dot Product between two Vectors
                    //double dotProdZ = Vector3.Dot(Norm, rotXAxis);
                    //double dotProdY = Vector3.Dot(Norm, rotYAxis);
                    ////Claculate Magnitude of all Vectors
                    //double magZAxis = Math.Sqrt(Math.Pow(rotXAxis.X, 2) + Math.Pow(rotXAxis.Y, 2) + Math.Pow(rotXAxis.Z, 2));
                    //double magYAxis = Math.Sqrt(Math.Pow(rotYAxis.X, 2) + Math.Pow(rotYAxis.Y, 2) + Math.Pow(rotYAxis.Z, 2)); ;
                    //double magNorm = Math.Sqrt(Math.Pow(Norm.X, 2) + Math.Pow(Norm.Y, 2) + Math.Pow(Norm.Z, 2));

                    //double MagnitudeZ = magZAxis * magNorm;
                    //double MagnitudeY = magYAxis * magNorm;

                    //thetax = Math.Acos(dotProdZ / MagnitudeZ);
                    //thetay = Math.Acos(dotProdY / MagnitudeY);
                    ////OLD CODE END

                    //ROTATION
                    //NEW CODE
                    /*
                    thetay = Math.Atan((transGuy[0].Y - Norm.Y) / (transGuy[0].Z - Norm.Z));
                    thetax = Math.Atan((transGuy[0].X - Norm.X) / (transGuy[0].Z - Norm.Z));
                    */
                    //NEW CODE END

                    /*
                    u \ Math.Sqrt(Math.Pow(u, 2) + Math.Pow(v, 2)), v/ Math.Sqrt(Math.Pow(u, 2) + Math.Pow(v, 2)), 0 ,0

                    -v\ Math.Sqrt(Math.Pow(u, 2) + Math.Pow(v, 2)), u/ Math.Sqrt(Math.Pow(u, 2) + Math.Pow(v, 2)), 0 ,0

                       0, 0, 1, 0
                        0, 0, 0, 1
                    */

                    //totalRot = Matrix.Multiply(rotY, rotX);


                    for (int i = 0; i < transGuy.Length; i++)
                    {
                        //Translate to Origin
                        Vector3 temp = new Vector3(transGuy[i].X - TransValue.X
                            , transGuy[i].Y - TransValue.Y
                            , transGuy[i].Z - TransValue.Z);
                        
                        Vector3 c, l, r;

                        Vector3 unitY = Vector3.UnitY;

                        //Creating The Normal Vector

                        l = new Vector3(transGuy[12].X - TransValue.X
                            , transGuy[12].Y - TransValue.Y
                            , transGuy[12].Z - TransValue.Z);

                        r = new Vector3(transGuy[16].X - TransValue.X
                            , transGuy[16].Y - TransValue.Y
                            , transGuy[16].Z - TransValue.Z);

                        c = new Vector3(transGuy[0].X - TransValue.X
                            , transGuy[0].Y - TransValue.Y
                            , transGuy[0].Z - TransValue.Z);

                        Normal_joint = skeleton.Joints[JointType.HipCenter].Position;
                        Normal2_joint = Normal_joint;

                        Vector3 a = new Vector3(l.X - r.X, l.Y - r.Y, l.Z - r.Z);

                        Vector3 b = new Vector3(r.X - c.X, r.Y - c.Y, r.Z - c.Z);

                        //Calculating Normal Vector
                        Vector3 Dir = Vector3.Cross(a, b);
                        Vector3 Norm = Vector3.Normalize(Dir);
                        Norm = new Vector3(-1f * Norm.X, -1f * Norm.Y, Norm.Z);
                        
                        //Calculate Angel
                        thetay = 360 - (90 - Math.Atan((Norm.X - c.X) / (Norm.Z - c.Z)));
                        thetax = Math.Atan((Norm.Y - c.Y) / (Norm.Z - c.Z));

                        rotY = new Matrix((float)Math.Cos(thetay), 0f, (float)(-Math.Sin(thetay)), 0f
                            , 0f, 1f, 0f, 0f
                            , ((float)(Math.Sin(thetay))), 0f, (float)Math.Cos(thetay), 0f
                            , 0f, 0f, 0f, 1f);

                        rotX = new Matrix(1f, 0f, 0f, 0f
                            , 0f, (float)Math.Cos(thetax), (float)Math.Sin(thetax), 0f
                            , 0f, (float)(- Math.Sin(thetax)), (float)Math.Cos(thetax), 0f
                            , 0f, 0f, 0f, 1f);

                        Matrix scale = new Matrix(0.5f, 0f, 0f, 0f
                            , 0f, 0.5f, 0f, 0f
                            , 0f, 0f, 0.5f, 0f
                            , 0f, 0f, 0f, 1f);

                        thetaYString = thetay.ToString();

                        Normal_joint.X = Norm.X;
                        Normal_joint.Y = Norm.Y;
                        Normal_joint.Z = Norm.Z;
                        //Ending Calculation of Normal Vector

                        rotPoint = new Matrix(temp.X, 0, 0, 0,
                            temp.Y, 0, 0, 0,
                            temp.Z, 0, 0, 0,
                            0, 0, 0, 1);

                        rotPoint = Matrix.Multiply(rotPoint, rotY);
                        //rotPoint = Matrix.Multiply(rotPoint, rotX);
                        //rotPoint = Matrix.Multiply(rotPoint, scale);

                        temp = new Vector3(rotPoint.M11 + TransValue.X, rotPoint.M21 + TransValue.Y, rotPoint.M31 + TransValue.Z);
                        
                        float zOffset = temp.Z - 3f;
                        temp.Z = temp.Z + zOffset;
                        //Vector3 temp2 = new Vector3(0f, 0f, 0f);

                        //temp2.X = (float)(temp.X * ((Math.Tan(thetay) * unitY.X * unitY.X) + Math.Cos(thetay))
                        //    + temp.Y * ((Math.Tan(thetay) * unitY.X * unitY.Y) - (Math.Sin(thetay) * unitY.Z)) 
                        //    + temp.Z * ((Math.Tan(thetay) * unitY.X * unitY.Z) + (Math.Sin(thetay) * unitY.Y)));

                        //temp2.Y = (float)(temp.X * ((Math.Tan(thetay) * unitY.X * unitY.Y) + (Math.Sin(thetay) * unitY.Z)) 
                        //    + temp.Y * ((Math.Tan(thetay) * unitY.Y * unitY.Y) + (Math.Cos(thetay))) 
                        //    + temp.Z * ((Math.Tan(thetay) * unitY.Y * unitY.Z) - (Math.Sin(thetay) * unitY.X)));

                        //temp2.Z = (float)(temp.X * ((Math.Tan(thetay) * unitY.X *unitY.Z) - (Math.Sin(thetay) * unitY.Y)) 
                        //    + temp.Y * ((Math.Tan(thetay) * unitY.Y * unitY.Z) + (Math.Sin(thetay) * unitY.X))  
                        //    + temp.Z * ((Math.Tan(thetay) * unitY.Z * unitY.Z) + Math.Cos(thetay)));

                        transGuy[i].X = temp.X;
                        transGuy[i].Y = temp.Y;
                        transGuy[i].Z = temp.Z;
                    }
                    //Normal2_joint.X = Norm.X;
                    //Normal2_joint.Y = Norm.Y;
                    //Normal2_joint.Z = Norm.Z;
                }

                //Rotation

            }

            /*
            //Gesture Recognition
            */

            //Keep Cover!
            if (skeleton != null)
            {
                if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.Head].Position.Y
                    && skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ShoulderCenter].Position.Y
                    || skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.Head].Position.Y
                    && skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ShoulderCenter].Position.Y)
                    keepcover = false;
                else
                    keepcover = true;
            }

            //Forward Thrust!
            if (skeleton != null)
            {
                if (skeleton.Joints[JointType.FootRight].Position.Y < skeleton.Joints[JointType.KneeLeft].Position.Y
                    && skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ShoulderCenter].Position.Y
                    || skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.Head].Position.Y
                    && skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ShoulderCenter].Position.Y)
                    keepcover = false;
                else
                    keepcover = true;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            //spriteBatch.Draw(colorTex, new Vector2(80,0), Color.White);

            spriteBatch.DrawString(font, "Project Recon v2.0", new Vector2(80, 0), Color.Red);
            spriteBatch.DrawString(font, thetaYString, new Vector2(80, 20), Color.Red);


            if (keepcover)
                //spriteBatch.DrawString(font, "Keep Cover!", new Vector2(150, 80), Color.Red);

                if (coachSkeleton != null && coachSkeleton != skeleton)
                {
                    spriteBatch.DrawString(font, "Coach Recognized!", new Vector2(450, 80), Color.Green);
                }

            //Drawing transGuy
            if (skeleton != null)
            {
                for (int i = 0; i < transGuy.Length; i++)
                {
                    var p = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                               transGuy[i], ColorImageFormat.RgbResolution640x480Fps30);

                    spriteBatch.Draw(circleTex, new Vector2(p.X + 75, p.Y - 5), Color.Green);

                    for (int j = 0; j < transGuy.Length; j++)
                    {
                        if ((i == 3 && j == 2)
                            || (i == 2 && j == 1)
                            || (i == 1 && j == 0))
                        {
                            //spriteBatch.DrawString(font,"I am here" ,new Vector2(200, 300), Color.Red);

                            var p2 = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                                transGuy[j], ColorImageFormat.RgbResolution640x480Fps30);
                          
                            spriteBatch.Draw(lineTex, new Vector2(p.X + 80, p.Y),
                                null, Color.White, (float)Math.Atan2(p2.Y - p.Y, (p2.X + 80) - (p.X + 80)), new Vector2(0f, (float)lineTex.Height / 2),
                                new Vector2(Vector2.Distance(new Vector2(p.X + 80, p.Y), new Vector2(p2.X + 80, p2.Y)), 1f), SpriteEffects.None, 0f);
                        }

                        if ((i == 0 && j == 12)
                            || (i == 12 && j == 13)
                            || (i == 13 && j == 14)
                            || (i == 14 && j == 15)
                            || (i == 2 && j == 4)
                            || (i == 4 && j == 5)
                            || (i == 5 && j == 6)
                            || (i == 6 && j == 7))
                        {
                            var p2 = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                            transGuy[j], ColorImageFormat.RgbResolution640x480Fps30);

                            spriteBatch.Draw(leftTex, new Vector2(p.X + 80, p.Y),
                                null, Color.White, (float)Math.Atan2(p2.Y - p.Y, (p2.X + 80) - (p.X + 80)), new Vector2(0f, (float)lineTex.Height / 2),
                                new Vector2(Vector2.Distance(new Vector2(p.X + 80, p.Y), new Vector2(p2.X + 80, p2.Y)), 1f), SpriteEffects.None, 0f);
                        }

                        if ((i == 0 && j == 16)
                            || (i == 16 && j == 17)
                            || (i == 17 && j == 18)
                            || (i == 18 && j == 19)
                            || (i == 2 && j == 8)
                            || (i == 8 && j == 9)
                            || (i == 9 && j == 10)
                            || (i == 10 && j == 11))
                        {
                            var p2 = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                            transGuy[j], ColorImageFormat.RgbResolution640x480Fps30);

                            spriteBatch.Draw(rightTex, new Vector2(p.X + 80, p.Y),
                                null, Color.White, (float)Math.Atan2(p2.Y - p.Y, (p2.X + 80) - (p.X + 80)), new Vector2(0f, (float)lineTex.Height / 2),
                                new Vector2(Vector2.Distance(new Vector2(p.X + 80, p.Y), new Vector2(p2.X + 80, p2.Y)), 1f), SpriteEffects.None, 0f);

                        }
                        //Drawing Normal Vector
                        if (i == 0)
                        {
                            var n = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                               Normal_joint, ColorImageFormat.RgbResolution640x480Fps30);

                            spriteBatch.Draw(lineTexRed, new Vector2(p.X + 80, p.Y),
                                null, Color.White, (float)Math.Atan2(n.Y - p.Y, (n.X + 80) - (p.X + 80)), new Vector2(0f, (float)lineTex.Height / 2),
                                new Vector2(Vector2.Distance(new Vector2(p.X + 80, p.Y), new Vector2(n.X + 80, n.Y)), 1f), SpriteEffects.None, 0f);
                        }

                    }

                    /*
                        double lengthofNormal = Math.Sqrt(Math.Pow(Math.Abs(Normal_2.X - skeleton.Joints[JointType.HipCenter].Position.X), 2)
                            + Math.Pow(Math.Abs(Normal_2.Y - skeleton.Joints[JointType.HipCenter].Position.Y), 2));

                        String length_Normal= lengthofNormal.ToString();
                        String Normal_x = Normal_2.X.ToString();
                        String Normal_y = Normal_2.Y.ToString();
                        String Normal_z = Normal_2.Z.ToString();

                        spriteBatch.DrawString(font, "Length: " + length_Normal, new Vector2(80, 40), Color.Blue);
                        spriteBatch.DrawString(font, "X: " + Normal_x, new Vector2(80, 60), Color.Blue);
                        spriteBatch.DrawString(font, "Y: " + Normal_y, new Vector2(80, 85), Color.Blue);
                        spriteBatch.DrawString(font, "Z: " + Normal_z, new Vector2(80, 105), Color.Blue);
                        */

                    String flipstr = flip.ToString();
                    spriteBatch.DrawString(font, flipstr, new Vector2(80, 40), Color.Blue);

                    //Drawing Joints
                    foreach (Joint joint in skeleton.Joints)
                    {
                        var p3 = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                            joint.Position, ColorImageFormat.RgbResolution640x480Fps30);

                        var shade = (float)joint.JointType / 20f;
                        spriteBatch.Draw(circleTex, new Vector2(p3.X + 75, p3.Y - 5), new Color(shade, shade, shade, 1));
                    }
                }

            }


            //NOTHING TO DO HERE!

            /*
            foreach (Joint joint in skeleton.Joints)
            {
                var p = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                    joint.Position, ColorImageFormat.RgbResolution640x480Fps30);

                foreach (Joint joint2 in skeleton.Joints)
                {
                    if ((joint.Equals(skeleton.Joints[JointType.Head]) && joint2.Equals(skeleton.Joints[JointType.ShoulderCenter]))
                        || (joint.Equals(skeleton.Joints[JointType.ShoulderCenter]) && joint2.Equals(skeleton.Joints[JointType.Spine]))
                        || (joint.Equals(skeleton.Joints[JointType.Spine]) && joint2.Equals(skeleton.Joints[JointType.HipCenter])))
                    {
                        var p2 = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                            joint2.Position, ColorImageFormat.RgbResolution640x480Fps30);

                        spriteBatch.Draw(lineTex, new Vector2(p.X + 80, p.Y),
                            null, Color.White, (float)Math.Atan2(p2.Y - p.Y, (p2.X + 80) - (p.X + 80)), new Vector2(0f, (float)lineTex.Height / 2),
                            new Vector2(Vector2.Distance(new Vector2(p.X + 80, p.Y), new Vector2(p2.X + 80, p2.Y)), 1f), SpriteEffects.None, 0f);

                    }

                    if ((joint.Equals(skeleton.Joints[JointType.HipCenter]) && joint2.Equals(skeleton.Joints[JointType.HipLeft]))
                        || (joint.Equals(skeleton.Joints[JointType.HipLeft]) && joint2.Equals(skeleton.Joints[JointType.KneeLeft]))
                        || (joint.Equals(skeleton.Joints[JointType.KneeLeft]) && joint2.Equals(skeleton.Joints[JointType.AnkleLeft]))
                        || (joint.Equals(skeleton.Joints[JointType.AnkleLeft]) && joint2.Equals(skeleton.Joints[JointType.FootLeft]))
                        || (joint.Equals(skeleton.Joints[JointType.ShoulderCenter]) && joint2.Equals(skeleton.Joints[JointType.ShoulderLeft]))
                        || (joint.Equals(skeleton.Joints[JointType.ShoulderLeft]) && joint2.Equals(skeleton.Joints[JointType.ElbowLeft]))
                        || (joint.Equals(skeleton.Joints[JointType.ElbowLeft]) && joint2.Equals(skeleton.Joints[JointType.WristLeft]))
                        || (joint.Equals(skeleton.Joints[JointType.WristLeft]) && joint2.Equals(skeleton.Joints[JointType.HandLeft])))
                    {
                        var p2 = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                        joint2.Position, ColorImageFormat.RgbResolution640x480Fps30);

                        spriteBatch.Draw(leftTex, new Vector2(p.X + 80, p.Y),
                            null, Color.White, (float)Math.Atan2(p2.Y - p.Y, (p2.X + 80) - (p.X + 80)), new Vector2(0f, (float)lineTex.Height / 2),
                            new Vector2(Vector2.Distance(new Vector2(p.X + 80, p.Y), new Vector2(p2.X + 80, p2.Y)), 1f), SpriteEffects.None, 0f);
                    }

                    if ((joint.Equals(skeleton.Joints[JointType.HipCenter]) && joint2.Equals(skeleton.Joints[JointType.HipRight]))
                        || (joint.Equals(skeleton.Joints[JointType.HipRight]) && joint2.Equals(skeleton.Joints[JointType.KneeRight]))
                        || (joint.Equals(skeleton.Joints[JointType.KneeRight]) && joint2.Equals(skeleton.Joints[JointType.AnkleRight]))
                        || (joint.Equals(skeleton.Joints[JointType.AnkleRight]) && joint2.Equals(skeleton.Joints[JointType.FootRight]))
                        || (joint.Equals(skeleton.Joints[JointType.ShoulderCenter]) && joint2.Equals(skeleton.Joints[JointType.ShoulderRight])) //here
                        || (joint.Equals(skeleton.Joints[JointType.ShoulderRight]) && joint2.Equals(skeleton.Joints[JointType.ElbowRight]))
                        || (joint.Equals(skeleton.Joints[JointType.ElbowRight]) && joint2.Equals(skeleton.Joints[JointType.WristRight]))
                        || (joint.Equals(skeleton.Joints[JointType.WristRight]) && joint2.Equals(skeleton.Joints[JointType.HandRight])))
                    {
                        var p2 = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                        joint2.Position, ColorImageFormat.RgbResolution640x480Fps30);

                        spriteBatch.Draw(rightTex, new Vector2(p.X + 80, p.Y),
                            null, Color.White, (float)Math.Atan2(p2.Y - p.Y, (p2.X + 80) - (p.X + 80)), new Vector2(0f, (float)lineTex.Height / 2),
                            new Vector2(Vector2.Distance(new Vector2(p.X + 80, p.Y), new Vector2(p2.X + 80, p2.Y)), 1f), SpriteEffects.None, 0f);
                    }
                }
            }*/

            spriteBatch.DrawString(font, "O", new Vector2(400, 200), Color.Green);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
