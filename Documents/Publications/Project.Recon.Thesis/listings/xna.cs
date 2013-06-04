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

            keepcover = true;

            base.Initialize();
        }
		
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

var dir = Vector3.Cross(rhip - chip, lhip - chip);
var norm = Vector3.Normalize(dir);
		
		