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

        short[] rawDepthData;
        float[] depthData;
        Texture2D depthTex;

        SpriteFont font;

        Skeleton[] rawSkeletons;
        Skeleton skeleton;

        Texture2D circleTex;

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

            //kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

            kinect.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);

            kinect.SkeletonStream.Enable();

            kinect.Start();

            kinect.ElevationAngle = 0;

            colorData = new byte[640 * 480 * 4];
            colorTex = new Texture2D(GraphicsDevice, 640, 480);

            rawDepthData = new short[320 * 240];
            depthData = new float[320 * 240];
          //  depthTex = new Texture2D(GraphicsDevice, 320, 240, false, SurfaceFormat.Single);

            rawSkeletons = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength];

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

            var depthFrame = kinect.DepthStream.OpenNextFrame(0);
            if (depthFrame != null)
            {
                depthFrame.CopyPixelDataTo(rawDepthData);
                depthFrame.Dispose();

                for (int i = 0; i < depthData.Length; i++)
                {
                    var val = (float)rawDepthData[i] / (depthFrame.MaxDepth * 8);
                    depthData[i] = val;
                }

                GraphicsDevice.Textures[0] = null;
                //depthTex.SetData(depthData);
            }

            var skelFrame = kinect.SkeletonStream.OpenNextFrame(0);
            if (skelFrame != null)
            {
                skelFrame.CopySkeletonDataTo(rawSkeletons);
                skelFrame.Dispose();

                skeleton = rawSkeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
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

            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
            //    DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Begin();

            //spriteBatch.Draw(depthTex, Vector2.Zero, Color.White);
            spriteBatch.Draw(colorTex, new Vector2(80,0), Color.White);

            spriteBatch.DrawString(font, "Project Recon v1.0", new Vector2(80, 0), Color.Red);

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

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
