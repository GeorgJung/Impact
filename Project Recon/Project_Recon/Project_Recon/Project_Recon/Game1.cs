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

        Texture2D circleTex;
        Texture2D lineTex;
        Texture2D lineTexRed;
        Texture2D rightTex, leftTex;

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
                coachSkeleton = rawSkeletons.LastOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);

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
                    }
                    if ((joint.Equals(skeleton.Joints[JointType.AnkleRight])))
                    {
                        littleguy_rankle = skeleton.Joints[JointType.AnkleRight].Position;
                    }

                    //Elbow
                    if ((joint.Equals(skeleton.Joints[JointType.ElbowLeft])))
                    {
                        littleguy_lelbow = skeleton.Joints[JointType.ElbowLeft].Position;
                    }
                    if ((joint.Equals(skeleton.Joints[JointType.ElbowRight])))
                    {
                        littleguy_relbow = skeleton.Joints[JointType.ElbowRight].Position;
                    }

                    //Hand
                    if ((joint.Equals(skeleton.Joints[JointType.HandLeft])))
                    {
                        littleguy_lhand = skeleton.Joints[JointType.HandLeft].Position;
                    }
                    if ((joint.Equals(skeleton.Joints[JointType.HandRight])))
                    {
                        littleguy_rhand = skeleton.Joints[JointType.HandRight].Position;
                    }

                    //Head
                    if ((joint.Equals(skeleton.Joints[JointType.Head])))
                    {
                        littleguy_head = skeleton.Joints[JointType.Head].Position;
                    }

                    //Hip
                    if ((joint.Equals(skeleton.Joints[JointType.HipCenter])))
                    {
                        littleguy_chip = skeleton.Joints[JointType.HipCenter].Position;
                    }

                    if ((joint.Equals(skeleton.Joints[JointType.HipLeft])))
                    {
                        littleguy_lhip = skeleton.Joints[JointType.HipLeft].Position;
                    }
                    if ((joint.Equals(skeleton.Joints[JointType.HipRight])))
                    {
                        littleguy_rhip = skeleton.Joints[JointType.HipRight].Position;
                    }

                    //Knee
                    if ((joint.Equals(skeleton.Joints[JointType.KneeLeft])))
                    {
                        littleguy_lknee = skeleton.Joints[JointType.KneeLeft].Position;
                    }
                    if ((joint.Equals(skeleton.Joints[JointType.KneeRight])))
                    {
                        littleguy_rhip = skeleton.Joints[JointType.KneeRight].Position;
                    }

                    //Shoulder
                    if ((joint.Equals(skeleton.Joints[JointType.ShoulderCenter])))
                    {
                        littleguy_cshoulder = skeleton.Joints[JointType.ShoulderCenter].Position;
                    }
                    if ((joint.Equals(skeleton.Joints[JointType.ShoulderLeft])))
                    {
                        littleguy_lshoulder = skeleton.Joints[JointType.ShoulderLeft].Position;
                    }
                    if ((joint.Equals(skeleton.Joints[JointType.ShoulderRight])))
                    {
                        littleguy_rshoulder = skeleton.Joints[JointType.ShoulderRight].Position;
                    }

                    //Spine
                    if ((joint.Equals(skeleton.Joints[JointType.Spine])))
                    {
                        littleguy_spine = skeleton.Joints[JointType.Spine].Position;
                    }

                    //Wrist
                    if ((joint.Equals(skeleton.Joints[JointType.WristLeft])))
                    {
                        littleguy_lwrist = skeleton.Joints[JointType.WristLeft].Position;
                    }
                    if ((joint.Equals(skeleton.Joints[JointType.WristRight])))
                    {
                        littleguy_rwrist = skeleton.Joints[JointType.WristRight].Position;
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
            
            spriteBatch.DrawString(font, "Project Recon v1.0", new Vector2(80, 0), Color.Red);

            if (keepcover) 
                //spriteBatch.DrawString(font, "Keep Cover!", new Vector2(150, 80), Color.Red);

            if (skeleton != null)
            {
                //Creating The Normal Vector
              
                SkeletonPoint Normal_joint = skeleton.Joints[JointType.HipCenter].Position;
                SkeletonPoint Normal2_joint = Normal_joint;

                Vector3 a = new Vector3((skeleton.Joints[JointType.HipCenter].Position.X) - (skeleton.Joints[JointType.HipLeft].Position.X)
                    , (skeleton.Joints[JointType.HipCenter].Position.Y) - (skeleton.Joints[JointType.HipLeft].Position.Y),
                    (skeleton.Joints[JointType.HipCenter].Position.Z) - (skeleton.Joints[JointType.HipLeft].Position.Z));

                Vector3 b = new Vector3((skeleton.Joints[JointType.HipCenter].Position.X) - (skeleton.Joints[JointType.HipRight].Position.X)
                    , (skeleton.Joints[JointType.HipCenter].Position.Y) - (skeleton.Joints[JointType.HipRight].Position.Y),
                    (skeleton.Joints[JointType.HipCenter].Position.Z) - (skeleton.Joints[JointType.HipRight].Position.Z));

                Vector3 Normal = new Vector3(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);

                Vector3 Normal_2 = new Vector3(-10f*(a.Y * b.Z - a.Z * b.Y), -10f*(a.Z * b.X - a.X * b.Z), 10f*(a.X * b.Y - a.Y * b.X));

                Normal_joint.X = Normal_2.X;
                Normal_joint.Y = Normal_2.Y;
                Normal_joint.Z = Normal_2.Z;

                Normal2_joint.X = Normal_2.X;
                Normal2_joint.Y = Normal_2.Y;
                Normal2_joint.Z = Normal_2.Z;

                var n = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                        Normal_joint, ColorImageFormat.RgbResolution640x480Fps30);

                var n2 = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                        skeleton.Joints[JointType.HipCenter].Position, ColorImageFormat.RgbResolution640x480Fps30);

                spriteBatch.Draw(lineTexRed, new Vector2(n2.X + 80, n2.Y),
                    null, Color.White, (float)Math.Atan2((n.Y) - n2.Y, ((n.X) + 80) - (n2.X + 80)), new Vector2(0f, (float)lineTex.Height / 2),
                    new Vector2(Vector2.Distance(new Vector2(n2.X + 80, n2.Y), new Vector2((n.X) + 80, (n.Y))), 1f), SpriteEffects.None, 0f);

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

                //flipping the skeleton
                
                /*
                if (Normal_2.Z < 0)
                {
                    flip = true;
                    foreach (Joint joint in skeleton.Joints)
                    {
                        foreach (Joint joint2 in skeleton.Joints)
                        {
                            if((joint.Equals(skeleton.Joints[JointType.HipLeft]) && joint2.Equals(skeleton.Joints[JointType.HipRight]))
                                || (joint.Equals(skeleton.Joints[JointType.ShoulderLeft]) && joint2.Equals(skeleton.Joints[JointType.ShoulderRight]))
                                || (joint.Equals(skeleton.Joints[JointType.ElbowLeft]) && joint2.Equals(skeleton.Joints[JointType.ElbowRight]))
                                || (joint.Equals(skeleton.Joints[JointType.WristLeft]) && joint2.Equals(skeleton.Joints[JointType.WristRight]))
                                || (joint.Equals(skeleton.Joints[JointType.HandLeft]) && joint2.Equals(skeleton.Joints[JointType.HandRight]))
                                || (joint.Equals(skeleton.Joints[JointType.KneeLeft]) && joint2.Equals(skeleton.Joints[JointType.KneeRight]))
                                || (joint.Equals(skeleton.Joints[JointType.AnkleLeft]) && joint2.Equals(skeleton.Joints[JointType.AnkleRight]))
                                || (joint.Equals(skeleton.Joints[JointType.FootLeft]) && joint2.Equals(skeleton.Joints[JointType.FootRight])))
                            {
                                Vector3 position = new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
                                joint.Position = joint2.Position;
                            
                            }
                        }
                    }
                }
                */

                //Drawing Joints
                foreach (Joint joint in skeleton.Joints)
                {
                    var p = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                        joint.Position, ColorImageFormat.RgbResolution640x480Fps30);

                    var shade = (float)joint.JointType / 20f;
                    spriteBatch.Draw(circleTex, new Vector2(p.X + 75, p.Y - 5), new Color(shade, shade, shade, 1));
                }
            }
            

            if (coachSkeleton != null && coachSkeleton != skeleton)
            {
                spriteBatch.DrawString(font, "Coach Recognized!", new Vector2(450, 80), Color.Green);
            }
            
            //NEW
            if (skeleton != null)
            {
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
                }                
            }
           /* 
            if (coachSkeleton != null)
            {
                foreach (Joint joint in coachSkeleton.Joints)
                {
                    var p = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                        joint.Position, ColorImageFormat.RgbResolution640x480Fps30);

                    foreach (Joint joint2 in coachSkeleton.Joints)
                    {
                        if ((joint.Equals(coachSkeleton.Joints[JointType.Head]) && joint2.Equals(coachSkeleton.Joints[JointType.ShoulderCenter]))
                            || (joint.Equals(coachSkeleton.Joints[JointType.ShoulderCenter]) && joint2.Equals(coachSkeleton.Joints[JointType.Spine]))
                            || (joint.Equals(coachSkeleton.Joints[JointType.Spine]) && joint2.Equals(coachSkeleton.Joints[JointType.HipCenter]))
                            || (joint.Equals(coachSkeleton.Joints[JointType.HipCenter]) && joint2.Equals(coachSkeleton.Joints[JointType.HipLeft]))
                            || (joint.Equals(coachSkeleton.Joints[JointType.HipLeft]) && joint2.Equals(coachSkeleton.Joints[JointType.KneeLeft]))
                            || (joint.Equals(coachSkeleton.Joints[JointType.KneeLeft]) && joint2.Equals(coachSkeleton.Joints[JointType.AnkleLeft]))
                            || (joint.Equals(coachSkeleton.Joints[JointType.AnkleLeft]) && joint2.Equals(coachSkeleton.Joints[JointType.FootLeft]))
                            || (joint.Equals(coachSkeleton.Joints[JointType.HipCenter]) && joint2.Equals(coachSkeleton.Joints[JointType.HipRight]))
                            || (joint.Equals(coachSkeleton.Joints[JointType.HipRight]) && joint2.Equals(coachSkeleton.Joints[JointType.KneeRight]))
                            || (joint.Equals(coachSkeleton.Joints[JointType.KneeRight]) && joint2.Equals(coachSkeleton.Joints[JointType.AnkleRight]))
                            || (joint.Equals(coachSkeleton.Joints[JointType.AnkleRight]) && joint2.Equals(coachSkeleton.Joints[JointType.FootRight]))
                            || (joint.Equals(coachSkeleton.Joints[JointType.ShoulderCenter]) && joint2.Equals(coachSkeleton.Joints[JointType.ShoulderRight])) //here
                            || (joint.Equals(coachSkeleton.Joints[JointType.ShoulderRight]) && joint2.Equals(coachSkeleton.Joints[JointType.ElbowRight]))
                            || (joint.Equals(coachSkeleton.Joints[JointType.ElbowRight]) && joint2.Equals(coachSkeleton.Joints[JointType.WristRight]))
                            || (joint.Equals(coachSkeleton.Joints[JointType.WristRight]) && joint2.Equals(coachSkeleton.Joints[JointType.HandRight]))
                            || (joint.Equals(coachSkeleton.Joints[JointType.ShoulderCenter]) && joint2.Equals(coachSkeleton.Joints[JointType.ShoulderLeft]))
                            || (joint.Equals(coachSkeleton.Joints[JointType.ShoulderLeft]) && joint2.Equals(coachSkeleton.Joints[JointType.ElbowLeft]))
                            || (joint.Equals(coachSkeleton.Joints[JointType.ElbowLeft]) && joint2.Equals(coachSkeleton.Joints[JointType.WristLeft]))
                            || (joint.Equals(coachSkeleton.Joints[JointType.WristLeft]) && joint2.Equals(coachSkeleton.Joints[JointType.HandLeft])))
                        {
                            var p2 = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                                joint2.Position, ColorImageFormat.RgbResolution640x480Fps30);

                            spriteBatch.Draw(lineTex, new Vector2(p.X + 80, p.Y),
                                null, Color.White, (float)Math.Atan2(p2.Y - p.Y, (p2.X + 80) - (p.X + 80)), new Vector2(0f, (float)lineTex.Height / 2),
                                new Vector2(Vector2.Distance(new Vector2(p.X + 80, p.Y), new Vector2(p2.X + 80, p2.Y)), 1f), SpriteEffects.None, 0f);

                        }
                    }
                } 
            }
            */
            

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
