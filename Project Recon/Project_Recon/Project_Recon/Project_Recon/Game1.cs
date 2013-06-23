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
        System.EventArgs e;

        GestureController gestureController;

        KinectSensor kinect;

        byte[] colorData;
        Texture2D colorTex;

        String lpunch, rpunch, lshin, rshin, lforward, rforward, lastMove;
        String FinalGesture;

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

        SkeletonPoint[] littleGuy;
        SkeletonPoint[] transGuy;

        Texture2D circleTex;
        Texture2D lineTex;
        Texture2D lineTexRed;
        Texture2D rightTex, leftTex;

        Vector3 TransValue;

        Boolean keepcover;

        DetectGesture rwrist, lwrist, rankle, lankle, rknee, lknee, relbow, lelbow, rshoulder, lshoulder, spine;

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

            //Client.connect("kinect", e, "8080","8080");


            kinect = KinectSensor.KinectSensors[0];

            kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

            kinect.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);

            kinect.SkeletonStream.Enable();

            kinect.Start();

            kinect.ElevationAngle = 0;

            colorData = new byte[640 * 480 * 4];
            colorTex = new Texture2D(GraphicsDevice, 640, 480);

            rawSkeletons = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength];

            littleGuy = new SkeletonPoint[20];
            transGuy = new SkeletonPoint[20];

            spine = new DetectGesture(30);
            lshoulder = new DetectGesture(30);
            rshoulder = new DetectGesture(30);
            rwrist = new DetectGesture(30);
            lwrist = new DetectGesture(30);
            relbow = new DetectGesture(30);
            lelbow = new DetectGesture(30);
            rknee = new DetectGesture(30);
            lknee = new DetectGesture(30);
            lankle = new DetectGesture(30);
            rankle = new DetectGesture(30);

            TransValue = new Vector3(0, 0, 0);

            lpunch = "";
            rpunch = "";
            lastMove = "";
            FinalGesture = "";

            gestureController = new GestureController();
            gestureController.GestureRecognized += OnGestureRecognized;

            keepcover = true;

            //--------------Gestures
            //--WaveLeft
            //this is the array of segments which defines the gesture
            IRelativeGestureSegment[] waveLeftSegments = new IRelativeGestureSegment[6];
            //this is the first segment of the gesture you already implemented that implement the Irelativegestuesegment interface
            WaveLeftSegment1 waveLeftSegment1 = new WaveLeftSegment1();
            WaveLeftSegment2 waveLeftSegment2 = new WaveLeftSegment2();
            //we define the wave gesture as moving hand from right to left in order 3 times consecutively
            waveLeftSegments[0] = waveLeftSegment1;
            waveLeftSegments[1] = waveLeftSegment2;
            waveLeftSegments[2] = waveLeftSegment1;
            waveLeftSegments[3] = waveLeftSegment2;
            waveLeftSegments[4] = waveLeftSegment1;
            waveLeftSegments[5] = waveLeftSegment2;
            //add the gesture to the gesture controller to check for recognition
            this.gestureController.AddGesture("WaveLeft", waveLeftSegments);

            //SwipeLeft
            //this is the array of segments which defines the gesture
            IRelativeGestureSegment[] swipeLeftSegments = new IRelativeGestureSegment[2];
            //this is the first segment of the gesture you already implemented that implement the Irelativegestuesegment interface
            SwipeLeftSegment1 swipeLeftSegment1 = new SwipeLeftSegment1();
            SwipeLeftSegment2 swipeLeftSegment2 = new SwipeLeftSegment2();
            //we define the wave gesture as moving hand from right to left in order 3 times consecutively
            swipeLeftSegments[0] = swipeLeftSegment1;
            swipeLeftSegments[1] = swipeLeftSegment2;
            //add the gesture to the gesture controller to check for recognition
            this.gestureController.AddGesture("SwipeLeft", swipeLeftSegments);

            //SwipeRight
            //this is the array of segments which defines the gesture
            IRelativeGestureSegment[] swipeRightSegments = new IRelativeGestureSegment[3];
            //this is the first segment of the gesture you already implemented that implement the Irelativegestuesegment interface
            SwipeLeftSegment1 swipeRightSegment1 = new SwipeLeftSegment1();
            SwipeLeftSegment2 swipeRightSegment2 = new SwipeLeftSegment2();
            SwipeLeftSegment3 swipeRightSegment3 = new SwipeLeftSegment3();
            //we define the wave gesture as moving hand from right to left in order 3 times consecutively
            swipeRightSegments[0] = swipeRightSegment3;
            swipeRightSegments[1] = swipeRightSegment2;
            swipeRightSegments[2] = swipeRightSegment1;
            //add the gesture to the gesture controller to check for recognition
            this.gestureController.AddGesture("SwipeRight", swipeRightSegments);

            //SwipeUp
            //this is the array of segments which defines the gesture
            IRelativeGestureSegment[] swipeUpSegments = new IRelativeGestureSegment[3];
            //this is the first segment of the gesture you already implemented that implement the Irelativegestuesegment interface
            SwipeUpSegment1 swipeUpSegment1 = new SwipeUpSegment1();
            SwipeUpSegment2 swipeUpSegment2 = new SwipeUpSegment2();
            SwipeUpSegment3 swipeUpSegment3 = new SwipeUpSegment3();
            //we define the wave gesture as moving hand from right to left in order 3 times consecutively
            swipeUpSegments[0] = swipeUpSegment1;
            swipeUpSegments[1] = swipeUpSegment2;
            swipeUpSegments[2] = swipeUpSegment3;
            //add the gesture to the gesture controller to check for recognition
            this.gestureController.AddGesture("SwipeUp", swipeUpSegments);

            //SwipeDown
            //this is the array of segments which defines the gesture
            IRelativeGestureSegment[] swipeDownSegments = new IRelativeGestureSegment[3];
            //this is the first segment of the gesture you already implemented that implement the Irelativegestuesegment interface
            SwipeUpSegment1 swipeDownSegment1 = new SwipeUpSegment1();
            SwipeUpSegment2 swipeDownSegment2 = new SwipeUpSegment2();
            SwipeUpSegment3 swipeDownSegment3 = new SwipeUpSegment3();
            //we define the wave gesture as moving hand from right to left in order 3 times consecutively
            swipeDownSegments[2] = swipeDownSegment1;
            swipeDownSegments[1] = swipeDownSegment2;
            swipeDownSegments[0] = swipeDownSegment3;
            //add the gesture to the gesture controller to check for recognition
            this.gestureController.AddGesture("SwipeDown", swipeDownSegments);

            //Choose Move Forward
            //this is the array of segments which defines the gesture
            IRelativeGestureSegment[] chooseSegments = new IRelativeGestureSegment[3];
            //this is the first segment of the gesture you already implemented that implement the Irelativegestuesegment interface
            ZoomSegment1 chooseSegment1 = new ZoomSegment1();
            ZoomSegment2 chooseSegment2 = new ZoomSegment2();
            ZoomSegment3 chooseSegment3 = new ZoomSegment3();
            //we define the wave gesture as moving hand from right to left in order 3 times consecutively
            chooseSegments[0] = chooseSegment1;
            chooseSegments[1] = chooseSegment2;
            chooseSegments[2] = chooseSegment3;
            //add the gesture to the gesture controller to check for recognition
            this.gestureController.AddGesture("Choose", chooseSegments);

            //Choose Move Forward
            //this is the array of segments which defines the gesture
            IRelativeGestureSegment[] backSegments = new IRelativeGestureSegment[3];
            //this is the first segment of the gesture you already implemented that implement the Irelativegestuesegment interface
            ZoomSegment1 backSegment1 = new ZoomSegment1();
            ZoomSegment2 backSegment2 = new ZoomSegment2();
            ZoomSegment3 backSegment3 = new ZoomSegment3();
            //we define the wave gesture as moving hand from right to left in order 3 times consecutively
            backSegments[2] = backSegment1;
            backSegments[1] = backSegment2;
            backSegments[0] = backSegment3;
            //add the gesture to the gesture controller to check for recognition
            this.gestureController.AddGesture("Back", backSegments);

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

                gestureController.UpdateAllGestures(skeleton);
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

                        lankle.addPosition(littleguy_lankle, kinect);

                    }
                    if (joint.JointType == JointType.AnkleRight)
                    {
                        littleguy_rankle = skeleton.Joints[JointType.AnkleRight].Position;
                        littleGuy[18] = littleguy_rankle;

                        transGuy[18] = littleGuy[18];
                        transGuy[18].X = (float)((decimal)(littleGuy[18].X) + (decimal)(TransValue.X));
                        transGuy[18].Y = (float)((decimal)(littleGuy[18].Y) + (decimal)(TransValue.Y));
                        transGuy[18].Z = (float)((decimal)(littleGuy[18].Z));

                        rankle.addPosition(littleguy_rankle, kinect);
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

                        lelbow.addPosition(littleguy_lelbow, kinect);
                    }
                    if (joint.JointType == JointType.ElbowRight)
                    {
                        littleguy_relbow = skeleton.Joints[JointType.ElbowRight].Position;
                        littleGuy[9] = littleguy_relbow;

                        transGuy[9] = littleGuy[9];
                        transGuy[9].X = (float)((decimal)(littleGuy[9].X) + (decimal)(TransValue.X));
                        transGuy[9].Y = (float)((decimal)(littleGuy[9].Y) + (decimal)(TransValue.Y));
                        transGuy[9].Z = (float)((decimal)(littleGuy[9].Z));

                        relbow.addPosition(littleguy_relbow, kinect);
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
                    }
                    if (joint.JointType == JointType.FootRight)
                    {
                        littleguy_rfoot = skeleton.Joints[JointType.FootRight].Position;
                        littleGuy[19] = littleguy_rfoot;

                        transGuy[19] = littleGuy[19];
                        transGuy[19].X = (float)((decimal)(littleGuy[19].X) + (decimal)(TransValue.X));
                        transGuy[19].Y = (float)((decimal)(littleGuy[19].Y) + (decimal)(TransValue.Y));
                        transGuy[19].Z = (float)((decimal)(littleGuy[19].Z));
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
                    }
                    if (joint.JointType == JointType.HandRight)
                    {
                        littleguy_rhand = skeleton.Joints[JointType.HandRight].Position;
                        littleGuy[11] = littleguy_rhand;

                        transGuy[11] = littleGuy[11];
                        transGuy[11].X = (float)((decimal)(littleGuy[11].X) + (decimal)(TransValue.X));
                        transGuy[11].Y = (float)((decimal)(littleGuy[11].Y) + (decimal)(TransValue.Y));
                        transGuy[11].Z = (float)((decimal)(littleGuy[11].Z));
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
                    }

                    if (joint.JointType == JointType.HipLeft)
                    {
                        littleguy_lhip = skeleton.Joints[JointType.HipLeft].Position;
                        littleGuy[12] = littleguy_lhip;

                        transGuy[12] = littleGuy[12];
                        transGuy[12].X = (float)((decimal)(littleGuy[12].X) + (decimal)(TransValue.X));
                        transGuy[12].Y = (float)((decimal)(littleGuy[12].Y) + (decimal)(TransValue.Y));
                        transGuy[12].Z = (float)((decimal)(littleGuy[12].Z));
                    }
                    if (joint.JointType == JointType.HipRight)
                    {
                        littleguy_rhip = skeleton.Joints[JointType.HipRight].Position;
                        littleGuy[16] = littleguy_rhip;

                        transGuy[16] = littleGuy[16];
                        transGuy[16].X = (float)((decimal)(littleGuy[16].X) + (decimal)(TransValue.X));
                        transGuy[16].Y = (float)((decimal)(littleGuy[16].Y) + (decimal)(TransValue.Y));
                        transGuy[16].Z = (float)((decimal)(littleGuy[16].Z));
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

                        lknee.addPosition(littleguy_lknee, kinect);
                    }
                    if (joint.JointType == JointType.KneeRight)
                    {
                        littleguy_rknee = skeleton.Joints[JointType.KneeRight].Position;
                        littleGuy[17] = littleguy_rknee;

                        transGuy[17] = littleGuy[17];
                        transGuy[17].X = (float)((decimal)(littleGuy[17].X) + (decimal)(TransValue.X));
                        transGuy[17].Y = (float)((decimal)(littleGuy[17].Y) + (decimal)(TransValue.Y));
                        transGuy[17].Z = (float)((decimal)(littleGuy[17].Z));

                        rknee.addPosition(littleguy_rknee, kinect);
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
                    }
                    if (joint.JointType == JointType.ShoulderLeft)
                    {
                        littleguy_lshoulder = skeleton.Joints[JointType.ShoulderLeft].Position;
                        littleGuy[4] = littleguy_lshoulder;

                        transGuy[4] = littleGuy[4];
                        transGuy[4].X = (float)((decimal)(littleGuy[4].X) + (decimal)(TransValue.X));
                        transGuy[4].Y = (float)((decimal)(littleGuy[4].Y) + (decimal)(TransValue.Y));
                        transGuy[4].Z = (float)((decimal)(littleGuy[4].Z));

                        lshoulder.addPosition(littleguy_lshoulder, kinect);
                    }
                    if (joint.JointType == JointType.ShoulderRight)
                    {
                        littleguy_rshoulder = skeleton.Joints[JointType.ShoulderRight].Position;
                        littleGuy[8] = littleguy_rshoulder;

                        transGuy[8] = littleGuy[8];
                        transGuy[8].X = (float)((decimal)(littleGuy[8].X) + (decimal)(TransValue.X));
                        transGuy[8].Y = (float)((decimal)(littleGuy[8].Y) + (decimal)(TransValue.Y));
                        transGuy[8].Z = (float)((decimal)(littleGuy[8].Z));

                        rshoulder.addPosition(littleguy_rshoulder, kinect);
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

                        spine.addPosition(littleguy_spine, kinect);
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

                        lwrist.addPosition(littleguy_lwrist, kinect);
                    }
                    if (joint.JointType == JointType.WristRight)
                    {
                        littleguy_rwrist = skeleton.Joints[JointType.WristRight].Position;
                        littleGuy[10] = littleguy_rwrist;

                        transGuy[10] = littleGuy[10];
                        transGuy[10].X = (float)((decimal)(littleGuy[10].X) + (decimal)(TransValue.X));
                        transGuy[10].Y = (float)((decimal)(littleGuy[10].Y) + (decimal)(TransValue.Y));
                        transGuy[10].Z = (float)((decimal)(littleGuy[10].Z));

                        rwrist.addPosition(littleguy_rwrist, kinect);
                    }
                }


                /*
                //Gesture Recognition
                */

                //Punch
                lpunch = lelbow.Detect(spine.PosList, lshoulder.PosList);
                rpunch = relbow.Detect(spine.PosList, rshoulder.PosList);

                if (lpunch == "punch" || rpunch == "punch")
                {
                    lastMove = "punch";
                }

                //Keep Cover!

                if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.Head].Position.Y
                    && skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ShoulderCenter].Position.Y
                    || skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.Head].Position.Y
                    && skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ShoulderCenter].Position.Y)
                    keepcover = false;
                else
                    keepcover = true;

                //Forward Thrust!

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

            spriteBatch.Draw(colorTex, new Vector2(80,0), Color.White);

            spriteBatch.DrawString(font, "Project Recon v3.0", new Vector2(80, 0), Color.Red);
            spriteBatch.DrawString(font, FinalGesture, new Vector2(80, 20), Color.Red);

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
                            spriteBatch.DrawString(font,"I am here" ,new Vector2(200, 300), Color.Red);

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

                    }

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

        private void OnGestureRecognized(object sender, GestureEventArgs e)
        {
            switch (e.GestureName)
            {
                //case "Menu":
                //    FinalGesture = "Menu";
                //    //Client.send("kinect", e, FinalGesture);
                //    break;
                case "WaveRight":
                    FinalGesture = "Wave Right";
                    break;
                case "WaveLeft":
                    FinalGesture = "Wave Left";
                    break;
                //case "JoinedHands":
                //    FinalGesture = "Joined Hands";
                //    break;
                case "Choose":
                    FinalGesture = "Choice Made";
                    break;
                case "Back":
                    FinalGesture = "Back";
                    break;
                case "SwipeLeft":
                    FinalGesture = "Swipe Left";
                    break;
                case "SwipeRight":
                    FinalGesture = "Swipe Right";
                    break;
                case "SwipeUp":
                    FinalGesture = "Swipe Up";
                    break;
                case "SwipeDown":
                    FinalGesture = "Swipe Down";
                    break;
                default:
                    break;
            }
        }
    }
}
