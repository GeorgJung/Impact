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

        KinectSensor kinect;

        byte[] colorData;
        Texture2D colorTex;

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
        SkeletonPoint[] prevPositions;
        SkeletonPoint[] transGuy;

        Texture2D circleTex;
        Texture2D lineTex;
        Texture2D lineTexRed;
        Texture2D rightTex, leftTex;

        Vector2 TransValue;

        double thetaz; // angle between normal vector and Z axis
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

            TransValue = new Vector2(0, 0);

            keepcover = true;
            flip = false;
            
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
                    if ((joint.Equals(skeleton.Joints[JointType.AnkleLeft])))
                    {
                        littleguy_lankle = skeleton.Joints[JointType.AnkleLeft].Position;
                        littleGuy[14] = littleguy_lankle;
                        transGuy[14] = littleGuy[14];
                        prevPositions[14] = joint.Position;
                    }
                    if ((joint.Equals(skeleton.Joints[JointType.AnkleRight])))
                    {
                        littleguy_rankle = skeleton.Joints[JointType.AnkleRight].Position;
                        littleGuy[18] = littleguy_rankle;
                        transGuy[18] = littleGuy[18];
                        prevPositions[18] = joint.Position;
                    }

                    //Elbow
                    if ((joint.Equals(skeleton.Joints[JointType.ElbowLeft])))
                    {
                        littleguy_lelbow = skeleton.Joints[JointType.ElbowLeft].Position;
                        littleGuy[5] = littleguy_lelbow;
                        transGuy[5] = littleGuy[5];
                        prevPositions[5] = joint.Position;
                    }
                    if ((joint.Equals(skeleton.Joints[JointType.ElbowRight])))
                    {
                        littleguy_relbow = skeleton.Joints[JointType.ElbowRight].Position;
                        littleGuy[9] = littleguy_relbow;
                        transGuy[9] = littleGuy[9];
                        prevPositions[9] = joint.Position;
                    }

                    //Foot
                    if ((joint.Equals(skeleton.Joints[JointType.FootLeft])))
                    {
                        littleguy_lfoot = skeleton.Joints[JointType.FootLeft].Position;
                        littleGuy[15] = littleguy_lfoot;
                        transGuy[15] = littleGuy[15];
                        prevPositions[15] = joint.Position;
                    }
                    if ((joint.Equals(skeleton.Joints[JointType.FootRight])))
                    {
                        littleguy_rfoot = skeleton.Joints[JointType.FootRight].Position;
                        littleGuy[19] = littleguy_rfoot;
                        transGuy[19] = littleGuy[19];
                        prevPositions[19] = joint.Position;
                    }

                    //Hand
                    if ((joint.Equals(skeleton.Joints[JointType.HandLeft])))
                    {
                        littleguy_lhand = skeleton.Joints[JointType.HandLeft].Position;
                        littleGuy[7] = littleguy_lhand;
                        transGuy[7] = littleGuy[7];
                        prevPositions[7] = joint.Position;
                    }
                    if ((joint.Equals(skeleton.Joints[JointType.HandRight])))
                    {
                        littleguy_rhand = skeleton.Joints[JointType.HandRight].Position;
                        littleGuy[11] = littleguy_rhand;
                        transGuy[11] = littleGuy[11];
                        prevPositions[11] = joint.Position;
                    }

                    //Head
                    if ((joint.Equals(skeleton.Joints[JointType.Head])))
                    {
                        littleguy_head = skeleton.Joints[JointType.Head].Position;
                        littleGuy[3] = littleguy_head;
                        transGuy[3] = littleGuy[3];
                        prevPositions[3] = joint.Position;
                    }

                    //Hip
                    if ((joint.Equals(skeleton.Joints[JointType.HipCenter])))
                    {
                        littleguy_chip = skeleton.Joints[JointType.HipCenter].Position;
                        littleGuy[0] = littleguy_chip;
                        transGuy[0] = littleGuy[0];
                        prevPositions[0] = joint.Position;
                    }

                    if ((joint.Equals(skeleton.Joints[JointType.HipLeft])))
                    {
                        littleguy_lhip = skeleton.Joints[JointType.HipLeft].Position;
                        littleGuy[12] = littleguy_lhip;
                        transGuy[12] = littleGuy[12];
                        prevPositions[12] = joint.Position;
                    }
                    if ((joint.Equals(skeleton.Joints[JointType.HipRight])))
                    {
                        littleguy_rhip = skeleton.Joints[JointType.HipRight].Position;
                        littleGuy[16] = littleguy_rhip;
                        transGuy[16] = littleGuy[16];
                        prevPositions[16] = joint.Position;
                    }

                    //Knee
                    if ((joint.Equals(skeleton.Joints[JointType.KneeLeft])))
                    {
                        littleguy_lknee = skeleton.Joints[JointType.KneeLeft].Position;
                        littleGuy[13] = littleguy_lknee;
                        transGuy[13] = littleGuy[13];
                        prevPositions[13] = joint.Position;
                    }
                    if ((joint.Equals(skeleton.Joints[JointType.KneeRight])))
                    {
                        littleguy_rknee = skeleton.Joints[JointType.KneeRight].Position;
                        littleGuy[17] = littleguy_rknee;
                        transGuy[17] = littleGuy[17];
                        prevPositions[17] = joint.Position;
                    }

                    //Shoulder
                    if ((joint.Equals(skeleton.Joints[JointType.ShoulderCenter])))
                    {
                        littleguy_cshoulder = skeleton.Joints[JointType.ShoulderCenter].Position;
                        littleGuy[2] = littleguy_cshoulder;
                        transGuy[2] = littleGuy[2];
                        prevPositions[2] = joint.Position;   
                    }                    
                    if ((joint.Equals(skeleton.Joints[JointType.ShoulderLeft])))
                    {
                        littleguy_lshoulder = skeleton.Joints[JointType.ShoulderLeft].Position;
                        littleGuy[4] = littleguy_lshoulder;
                        transGuy[4] = littleGuy[4];
                        prevPositions[4] = joint.Position;
                    }
                    if ((joint.Equals(skeleton.Joints[JointType.ShoulderRight])))
                    {
                        littleguy_rshoulder = skeleton.Joints[JointType.ShoulderRight].Position;
                        littleGuy[8] = littleguy_rshoulder;
                        transGuy[8] = littleGuy[8];
                        prevPositions[8] = joint.Position;
                    }

                    //Spine
                    if ((joint.Equals(skeleton.Joints[JointType.Spine])))
                    {
                        littleguy_spine = skeleton.Joints[JointType.Spine].Position;
                        littleGuy[1] = littleguy_spine;
                        transGuy[1] = littleGuy[1];
                        prevPositions[1] = joint.Position;
                    }

                    //Wrist
                    if ((joint.Equals(skeleton.Joints[JointType.WristLeft])))
                    {
                        littleguy_lwrist = skeleton.Joints[JointType.WristLeft].Position;
                        littleGuy[6] = littleguy_lwrist;
                        transGuy[6] = littleGuy[6];
                        prevPositions[6] = joint.Position;
                    }
                    if ((joint.Equals(skeleton.Joints[JointType.WristRight])))
                    {
                        littleguy_rwrist = skeleton.Joints[JointType.WristRight].Position;
                        littleGuy[10] = littleguy_rwrist;
                        transGuy[10] = littleGuy[10];
                        prevPositions[10] = joint.Position;
                    }
                }
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

            if (keepcover) 
                //spriteBatch.DrawString(font, "Keep Cover!", new Vector2(150, 80), Color.Red);

            if (coachSkeleton != null && coachSkeleton != skeleton)
            {
                spriteBatch.DrawString(font, "Coach Recognized!", new Vector2(450, 80), Color.Green);
            }

            //Drawing transGuy
            if (skeleton != null)
            {
                //Translating transGuy
                for (int i = 0; i < littleGuy.Length; i++)
                {

                    var p = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                        transGuy[i], ColorImageFormat.RgbResolution640x480Fps30);

                    if (i == 0)
                    {
                        TransValue.X = 400 - p.X;
                        TransValue.Y = 200 - p.Y;

                        p.X = 400;
                        p.Y = 200;
                    }
                    else
                    {
                        p.X = p.X + (int)TransValue.X;
                        p.Y = p.Y + (int)TransValue.Y;
                    }

                    //rotate


                    for (int j = 0; j < littleGuy.Length; j++)
                    {
                        if ((transGuy[i] == littleguy_head && transGuy[j] == littleguy_cshoulder)
                            || (transGuy[i] == littleguy_cshoulder && transGuy[j] == littleguy_spine)
                            || (transGuy[i] == littleguy_spine && transGuy[j] == littleguy_chip))
                        {
                            var p2 = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                                transGuy[j], ColorImageFormat.RgbResolution640x480Fps30);

                            if (j == 0)
                            {
                                TransValue.X = 400 - p2.X;
                                TransValue.Y = 200 - p2.Y;

                                p2.X = 400;
                                p2.Y = 200;
                            }
                            else
                            {
                                p2.X = p2.X + (int)TransValue.X;
                                p2.Y = p2.Y + (int)TransValue.Y;
                            }

                            spriteBatch.Draw(lineTex, new Vector2(p.X, p.Y),
                                null, Color.White, (float)Math.Atan2(p2.Y - p.Y, (p2.X) - (p.X)), new Vector2(0f, (float)lineTex.Height / 2),
                                new Vector2(Vector2.Distance(new Vector2(p.X, p.Y), new Vector2(p2.X, p2.Y)), 1f), SpriteEffects.None, 0f);

                        }

                        if ((transGuy[i] == littleguy_chip && transGuy[j] == littleguy_lhip)
                            || (transGuy[i] == littleguy_lhip && transGuy[j] == littleguy_lknee)
                            || (transGuy[i] == littleguy_lknee && transGuy[j] == littleguy_lankle)
                            || (transGuy[i] == littleguy_lankle && transGuy[j] == littleguy_lfoot)
                            || (transGuy[i] == littleguy_cshoulder && transGuy[j] == littleguy_lshoulder)
                            || (transGuy[i] == littleguy_lshoulder && transGuy[j] == littleguy_lelbow)
                            || (transGuy[i] == littleguy_lelbow && transGuy[j] == littleguy_lwrist)
                            || (transGuy[i] == littleguy_lwrist && transGuy[j] == littleguy_lhand))
                        {
                            var p2 = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                            transGuy[j], ColorImageFormat.RgbResolution640x480Fps30);

                            if (j == 0)
                            {
                                TransValue.X = 400 - p2.X;
                                TransValue.Y = 200 - p2.Y;

                                p2.X = 400;
                                p2.Y = 200;
                            }
                            else
                            {
                                p2.X = p2.X + (int)TransValue.X;
                                p2.Y = p2.Y + (int)TransValue.Y;
                            }

                            spriteBatch.Draw(leftTex, new Vector2(p.X, p.Y),
                                null, Color.White, (float)Math.Atan2(p2.Y - p.Y, (p2.X) - (p.X)), new Vector2(0f, (float)lineTex.Height / 2),
                                new Vector2(Vector2.Distance(new Vector2(p.X, p.Y), new Vector2(p2.X, p2.Y)), 1f), SpriteEffects.None, 0f);
                        }

                        if ((transGuy[i] == littleguy_chip && transGuy[j] == littleguy_rhip)
                            || (transGuy[i] == littleguy_rhip && transGuy[j] == littleguy_rknee)
                            || (transGuy[i] == littleguy_rknee && transGuy[j] == littleguy_rankle)
                            || (transGuy[i] == littleguy_rankle && transGuy[j] == littleguy_rfoot)
                            || (transGuy[i] == littleguy_cshoulder && transGuy[j] == littleguy_rshoulder)
                            || (transGuy[i] == littleguy_rshoulder && transGuy[j] == littleguy_relbow)
                            || (transGuy[i] == littleguy_relbow && transGuy[j] == littleguy_rwrist)
                            || (transGuy[i] == littleguy_rwrist && transGuy[j] == littleguy_rhand))
                        {
                            var p2 = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                            transGuy[j], ColorImageFormat.RgbResolution640x480Fps30);

                            if (j == 0)
                            {
                                TransValue.X = 400 - p2.X;
                                TransValue.Y = 200 - p2.Y;

                                p2.X = 400;
                                p2.Y = 200;
                            }
                            else
                            {
                                p2.X = p2.X + (int)TransValue.X;
                                p2.Y = p2.Y + (int)TransValue.Y;
                            }

                            spriteBatch.Draw(rightTex, new Vector2(p.X, p.Y),
                                null, Color.White, (float)Math.Atan2(p2.Y - p.Y, (p2.X) - (p.X)), new Vector2(0f, (float)lineTex.Height / 2),
                                new Vector2(Vector2.Distance(new Vector2(p.X, p.Y), new Vector2(p2.X, p2.Y)), 1f), SpriteEffects.None, 0f);
                        }
                    }
                    //Creating The Normal Vector

                    SkeletonPoint Normal_joint = skeleton.Joints[JointType.HipCenter].Position;
                    SkeletonPoint Normal2_joint = Normal_joint;

                    Vector3 a = new Vector3((transGuy[12].X) - (transGuy[16].X)
                        , (transGuy[12].Y) - (transGuy[16].Y),
                        (transGuy[12].Z) - (transGuy[16].Z));

                    Vector3 b = new Vector3((transGuy[16].X) - (transGuy[0].X)
                        , (transGuy[16].Y) - (transGuy[0].Y),
                        (transGuy[16].Z) - (transGuy[0].Z));

                    if (!flip)
                    {
                        Vector3 Dir = Vector3.Cross(a, b);
                        Vector3 Norm = Vector3.Normalize(Dir);
                        Vector3 rotZAxis = new Vector3(0f, 0f, 0.5f);
                        Vector3 rotYAxis = new Vector3(0f, 0.5f, 0f);
                        Vector3 Dir_2 = new Vector3(-10f * Dir.X, -10f * Dir.Y, 10f * Dir.Z);

                        double dotProdZ = Vector3.Dot(Norm, rotZAxis);
                        double dotProdY = Vector3.Dot(Norm, rotYAxis);

                        double magZAxis = Math.Sqrt(Math.Pow(rotZAxis.X, 2) + Math.Pow(rotZAxis.Y, 2) + Math.Pow(rotZAxis.Z, 2));
                        double magYAxis = Math.Sqrt(Math.Pow(rotYAxis.X, 2) + Math.Pow(rotYAxis.Y, 2) + Math.Pow(rotYAxis.Z, 2)); ;
                        double magNorm = Math.Sqrt(Math.Pow(Norm.X, 2) + Math.Pow(Norm.Y, 2) + Math.Pow(Norm.Z, 2));

                        double MagnitudeZ = magZAxis * magNorm;
                        double MagnitudeY = magYAxis * magNorm;

                        thetaz = Math.Acos(dotProdZ / MagnitudeZ);
                        thetay = Math.Acos(dotProdY / MagnitudeY);

                        Normal_joint.X = Norm.X;
                        Normal_joint.Y = Norm.Y;
                        Normal_joint.Z = Norm.Z;

                        Normal2_joint.X = Norm.X;
                        Normal2_joint.Y = Norm.Y;
                        Normal2_joint.Z = Norm.Z;

                        //flipping was here
                    }
                    else
                    {
                        Vector3 Dir = Vector3.Cross(b, a);
                        Vector3 Norm = Vector3.Normalize(Dir);
                        Vector3 rotZAxis = new Vector3(0f, 0f, 0.5f);
                        Vector3 rotYAxis = new Vector3(0f, 0.5f, 0f);
                        Vector3 Dir_2 = new Vector3(-10f * Dir.X, -10f * Dir.Y, 10f * Dir.Z);

                        double dotProdZ = Vector3.Dot(Norm, rotZAxis);
                        double dotProdY = Vector3.Dot(Norm, rotYAxis);

                        double magZAxis = Math.Sqrt(Math.Pow(rotZAxis.X, 2) + Math.Pow(rotZAxis.Y, 2) + Math.Pow(rotZAxis.Z, 2));
                        double magYAxis = Math.Sqrt(Math.Pow(rotYAxis.X, 2) + Math.Pow(rotYAxis.Y, 2) + Math.Pow(rotYAxis.Z, 2)); ;
                        double magNorm = Math.Sqrt(Math.Pow(Norm.X, 2) + Math.Pow(Norm.Y, 2) + Math.Pow(Norm.Z, 2));
                        
                        double MagnitudeZ = magZAxis * magNorm;
                        double MagnitudeY = magYAxis * magNorm;

                        thetaz = Math.Acos(dotProdZ / MagnitudeZ);
                        thetay = Math.Acos(dotProdY / MagnitudeY);

                        Normal_joint.X = Norm.X;
                        Normal_joint.Y = Norm.Y;
                        Normal_joint.Z = Norm.Z;

                        Normal2_joint.X = Norm.X;
                        Normal2_joint.Y = Norm.Y;
                        Normal2_joint.Z = Norm.Z;
                    }
                    if (i == 0)
                    {
                        var n = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                           Normal_joint, ColorImageFormat.RgbResolution640x480Fps30);

                        spriteBatch.Draw(lineTexRed, new Vector2(p.X, p.Y),
                            null, Color.White, (float)Math.Atan2((n.Y) - p.Y, ((n.X)) - (p.X)), new Vector2(0f, (float)lineTex.Height / 2),
                            new Vector2(Vector2.Distance(new Vector2(p.X, p.Y), new Vector2((n.X), (n.Y))), 1f), SpriteEffects.None, 0f);
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
