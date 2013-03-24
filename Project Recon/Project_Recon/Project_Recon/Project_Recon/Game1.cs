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

        Texture2D circleTex;

        Boolean keepcover;

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
            
            if (skeleton != null && skeleton.Joints[JointType.HandRight].Position.Y > 0)
                GraphicsDevice.Clear(Color.Red);
            else
            
                GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(colorTex, new Vector2(80,0), Color.White);
            
            spriteBatch.DrawString(font, "Project Recon v1.0", new Vector2(80, 0), Color.Red);

            if (keepcover) 
                spriteBatch.DrawString(font, "Keep Cover!", new Vector2(150, 80), Color.Red);

            if (skeleton != null)
            {
                foreach (Joint joint in skeleton.Joints)
                {
                    var p = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(
                        joint.Position, ColorImageFormat.RgbResolution640x480Fps30);

                    var shade = (float)joint.JointType / 20f;
                    spriteBatch.Draw(circleTex, new Vector2(p.X +60, p.Y -20), new Color(shade, shade, shade, 1));
                }
            }

            if (coachSkeleton != null && coachSkeleton != skeleton)
            {
                spriteBatch.DrawString(font, "Coach Recognized!", new Vector2(450, 80), Color.Green);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
