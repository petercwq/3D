using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Magnet
{
    class MagnetViewModel : INotifyPropertyChanged
    {
        int MagnetPositionOnFloor = 22;
        int MagnetFloorNo = 2;
        int TotalCubes = 12;
        int CountMovingBlocks = 0;
        bool cubeAnimationCompleted = true;
        bool positionAnimationCompleted = true;
        bool cameraLookdirectionAnimationCompleted = true;
        bool fieldViewAnimationCompleted = true;
        double fieldofView = 70;

        PerspectiveCamera camera;
        Point3DAnimationUsingKeyFrames animationKeyFramesBox;
        Point3DAnimationUsingKeyFrames animationKeyFramesCameraPosition;
        Vector3DAnimationUsingKeyFrames animationKeyFramesCameraLookDirection;
        DoubleAnimationUsingKeyFrames animationFieldView;
        Point3DAnimationUsingKeyFrames animationKeyFramesBLocks;

        Dictionary<int, Dictionary<int, Cube>> position = new Dictionary<int, Dictionary<int, Cube>>();
        readonly TranslateTransform3D translate = new TranslateTransform3D(-225, -225, -125);
        Duration BlockMovementTime = new Duration(new TimeSpan(0, 0, 1));
        TimeSpan BlockBeginTime = new TimeSpan(0, 0, 0);
        List<Color> colorsCollection;
        List<ModelVisual3D> cubesCollection;
        Point3D cameraPosition = new Point3D(00, 125, 855);
        Vector3D cameraLookDirection = new Vector3D(0, -125, -855);

        TranslateTransform3D Translate
        {
            get
            {
                return translate;
            }
        }

        public Cube Magnet
        {
            get;
            private set;
        }

        public Model3DGroup TargetFigureGroup
        {
            get;
            private set;
        }

        public List<Color> ColorsCollection
        {
            get
            {
                if (colorsCollection == null)
                {
                    colorsCollection = new List<Color>();
                    colorsCollection.Add(Colors.Red);
                    colorsCollection.Add(Colors.Blue);
                    colorsCollection.Add(Colors.Green);
                }

                return colorsCollection;
            }
        }

        public List<ModelVisual3D> CubesCollection
        {
            get
            {
                return cubesCollection;
            }
        }

        public Point3D CameraPosition
        {
            get
            {
                return this.cameraPosition;
            }
            set
            {
                this.cameraPosition = value;
                NotifyPropertyChanged("CameraPosition");
            }
        }

        public int stepCount;
        public int StepCount
        {
            get
            {
                return stepCount;
            }
            set
            {
                this.stepCount = value;
                NotifyPropertyChanged("StepCount");
            }
        }

        public Vector3D CameraLookDirection
        {
            get
            {
                return this.cameraLookDirection;
            }
            set
            {
                this.cameraLookDirection = value;
                NotifyPropertyChanged("CameraLookDirection");
            }
        }

        public double FieldofView
        {
            get
            {
                return this.fieldofView;
            }
            set
            {
                this.fieldofView = value;
                NotifyPropertyChanged("FieldofView");
            }
        }

        private PerspectiveCamera ViewModelCamera
        {
            get
            {
                if (camera == null)
                {
                    camera = new PerspectiveCamera(CameraPosition, CameraLookDirection, new Vector3D(0, 0, 0), FieldofView);
                    camera.Changed += new EventHandler(Camera_changed);
                }

                return camera;
            }
        }

        public DelegateCommand<KeyEventArgs> KeyDownCommand
        {
            get;
            private set;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangedEventHandler CollectionChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Camera_changed(object sender, EventArgs e)
        {
            CameraLookDirection = (sender as PerspectiveCamera).LookDirection;
            CameraPosition = (sender as PerspectiveCamera).Position;
            FieldofView = (sender as PerspectiveCamera).FieldOfView;
        }

        /// <summary>
        /// 
        /// </summary>
        public MagnetViewModel()
        {
            this.cubesCollection = new List<ModelVisual3D>();

            Initialize();

            this.KeyDownCommand = new DelegateCommand<KeyEventArgs>(
            (s) => { MoveCube(s); },
            (s) => { return true; }
            );
        }

        private void MoveCube(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.Left:
                case System.Windows.Input.Key.F:
                    MoveCube(this.Magnet, Direction.Left);
                    break;
                case System.Windows.Input.Key.Right:
                case System.Windows.Input.Key.H:
                    MoveCube(this.Magnet, Direction.Right);
                    break;
                case System.Windows.Input.Key.Up:
                case System.Windows.Input.Key.T:
                    MoveCube(this.Magnet, Direction.Up);
                    break;
                case System.Windows.Input.Key.Down:
                case System.Windows.Input.Key.V:
                    MoveCube(this.Magnet, Direction.Down);
                    break;
                case System.Windows.Input.Key.PageUp:
                case System.Windows.Input.Key.G:
                    MoveCube(this.Magnet, Direction.Front);
                    break;
                case System.Windows.Input.Key.PageDown:
                case System.Windows.Input.Key.Y:
                    MoveCube(this.Magnet, Direction.Back);
                    break;
                case System.Windows.Input.Key.Enter:
                case System.Windows.Input.Key.Space:
                    Magnet_Click(null, null);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Initialize()
        {
            this.TargetFigureGroup = TargetFigure();
            this.PlaceCubes();
            CalculateDistance(this.ViewModelCamera);
        }

        /// <summary>
        /// 
        /// </summary>
        private Model3DGroup TargetFigure()
        {
            Model3DGroup targetFigureGroup = new Model3DGroup();

            double cubeLength = Constants.TargetCubeLength;

            Cube cubeMain = new Cube();

            List<Point3D> pointCollection = new List<Point3D>();

            pointCollection.Add(new Point3D(0, 0, 0));
            pointCollection.Add(new Point3D(2 * cubeLength, 0, 0));
            pointCollection.Add(new Point3D(1 * cubeLength, 1 * cubeLength, 0));
            pointCollection.Add(new Point3D(cubeLength, -1 * cubeLength, 0));

            pointCollection.Add(new Point3D(1 * cubeLength, 0, 1 * cubeLength));
            pointCollection.Add(new Point3D(cubeLength, 0, -1 * cubeLength));

            for (int i = 0; i <= 5; i++)
            {
                cubeMain = new Cube();
                cubeMain.Transform = Translate;
                cubeMain.opacity = 1;
                cubeMain.WidthHeightDepth = cubeLength;
                cubeMain.StartingPointCube = pointCollection[i];
                cubeMain.color = ColorsCollection[i % 3];
                targetFigureGroup.Children.Add(cubeMain.Content);
            }

            cubeMain = new Cube();
            cubeMain.Transform = Translate;
            cubeMain.opacity = 1;
            cubeMain.WidthHeightDepth = cubeLength;
            cubeMain.StartingPointCube = new Point3D(cubeLength, 0, 0);
            cubeMain.color = Colors.Yellow;
            targetFigureGroup.Children.Add(cubeMain.Content);

            return targetFigureGroup;
        }

        /// <summary>
        /// Create animation using key frames
        /// </summary>
        private void PlaceCubes()
        {
            int cubesPerFloor = (int)((Constants.BlocksInXdirection) * (Constants.BlocksInZdirection));

            for (int i = 0; i < Constants.NoofFloor; i++)
            {
                Dictionary<int, Cube> floor = new Dictionary<int, Cube>();
                for (int j = 0; j < cubesPerFloor; j++)
                {
                    floor.Add(j, null);
                }
                position.Add(i, floor);
            }

            this.Magnet = PlaceCube(Constants.MagnetInitialXPos, Constants.MagnetInitialYPos, Constants.MagnetInitialZPos, Colors.Yellow);

            this.MagnetFloorNo = Constants.MagnetInitialYPos;
            this.MagnetPositionOnFloor = Constants.BlocksInXdirection * Constants.MagnetInitialZPos + Constants.MagnetInitialXPos;

            this.position[this.MagnetFloorNo][this.MagnetPositionOnFloor] = this.Magnet;
            this.Magnet.IsMovingCube = true;
            Random randomCube = new Random(1000);
            Random randomDirection = new Random();

            int xcoor, ycoor, zcoor;
            int floorNo = -1;
            int positionOnFloor = randomCube.Next(0, cubesPerFloor);
            Random randomSteps = new Random();
            for (int i = 1; i <= TotalCubes; i++)
            {
                positionOnFloor = randomCube.Next(0, cubesPerFloor);

                ///int randomirection = randomDirection.Next(6);

                Color color = ColorsCollection[i % 3];

                floorNo = (floorNo + 3) % Constants.NoofFloor;

                //This position is unoccupied
                if (position[floorNo][positionOnFloor] == null)
                {
                    xcoor = (int)(positionOnFloor % (Constants.BlocksInXdirection));

                    ycoor = floorNo;

                    zcoor = (int)(positionOnFloor / (Constants.BlocksInZdirection));

                    position[floorNo][positionOnFloor] = PlaceCube(xcoor, floorNo, zcoor, color);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UP_Click(object sender, RoutedEventArgs e)
        {
            MoveCube(this.Magnet, Direction.Up);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DOWN_Click(object sender, RoutedEventArgs e)
        {
            MoveCube(this.Magnet, Direction.Down);
        }

        /// <summary>
        /// 
        /// </summary>
        private Cube PlaceCube(int xCoor, int yCoor, int zCoor, Color color)
        {
            double cubeLength = Constants.CubeLength;
            Cube cube3d = new Cube();
            cube3d.Transform = Translate;
            cube3d.color = color;
            cube3d.WidthHeightDepth = Constants.CubeLength;
            cube3d.opacity = 1;
            cube3d.StartingPointCube = new Point3D(cubeLength * xCoor, cubeLength * yCoor, cubeLength * zCoor);
            cubesCollection.Add(cube3d);
            return cube3d;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cube3d"></param>
        /// <param name="step"></param>
        /// <param name="direction"></param>
        private void MoveBlocks(Cube cube3d, int step, Direction direction)
        {
            if (animationKeyFramesBLocks != null)
            {
                ///  animationKeyFramesBLocks.Completed -= AnimationKeyFramesBLocks_Completed;
            }

            animationKeyFramesBLocks = new Point3DAnimationUsingKeyFrames();
            animationKeyFramesBLocks.Completed += new EventHandler(AnimationKeyFramesBLocks_Completed);

            double cubeLength = Magnet.WidthHeightDepth;
            animationKeyFramesBLocks.Duration = BlockMovementTime;
            animationKeyFramesBLocks.BeginTime = BlockBeginTime;

            Point3D cubeLocation = cube3d.StartingPointCube;

            for (int i = 0; i < cubeLength * step; i++)
            {
                switch (direction)
                {
                    case Direction.Up:

                        cubeLocation.Y++;
                        goto default;

                    case Direction.Down:

                        cubeLocation.Y--;
                        goto default;

                    case Direction.Left:

                        cubeLocation.X--;
                        goto default;
                    case Direction.Right:

                        cubeLocation.X++;
                        goto default;
                    case Direction.Front:

                        cubeLocation.Z++;
                        goto default;

                    case Direction.Back:
                        cubeLocation.Z--;
                        goto default;

                    default:

                        LinearPoint3DKeyFrame linear3dkeyframeBox = new LinearPoint3DKeyFrame(cubeLocation);
                        animationKeyFramesBLocks.KeyFrames.Add(linear3dkeyframeBox);
                        break;
                }
            }

            CountMovingBlocks++;
            cube3d.BeginAnimation(Cube.StartingPointCubeProperty, animationKeyFramesBLocks, HandoffBehavior.SnapshotAndReplace);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnimationKeyFramesBLocks_Completed(object sender, EventArgs e)
        {
            CountMovingBlocks = CountMovingBlocks - 1;
            this.CheckCompletness();
            /// AnimationClock timeLine = sender as AnimationClock;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MovingCube"></param>
        /// <param name="direction"></param>
        private void MoveCube(Cube MovingCube, Direction direction)
        {
            if (this.cubeAnimationCompleted == true && this.positionAnimationCompleted == true && this.cameraLookdirectionAnimationCompleted == true && this.fieldViewAnimationCompleted == true && CountMovingBlocks == 0)
            {
                Point3D cameraPosition = this.ViewModelCamera.Position;
                Vector3D cameraLookDirection = this.ViewModelCamera.LookDirection;
                double cameraFieldOfView = this.ViewModelCamera.FieldOfView;
                double cubeWidthHeight = (MovingCube.WidthHeightDepth);

                if (animationKeyFramesBox != null)
                {
                    animationKeyFramesBox.Completed -= AnimationKeyFramesBox_Completed;
                }

                animationKeyFramesBox = new Point3DAnimationUsingKeyFrames();
                animationKeyFramesBox.Completed += new EventHandler(AnimationKeyFramesBox_Completed);

                if (animationFieldView != null)
                {
                    animationFieldView.Completed -= new EventHandler(AnimationFieldView_Completed);
                }

                animationFieldView = new DoubleAnimationUsingKeyFrames();
                animationFieldView.Completed += new EventHandler(AnimationFieldView_Completed);

                if (animationKeyFramesCameraPosition != null)
                {
                    animationKeyFramesCameraPosition.Completed -= new EventHandler(AnimationKeyFramesCameraPosition_Completed);
                }

                animationKeyFramesCameraPosition = new Point3DAnimationUsingKeyFrames();
                animationKeyFramesCameraPosition.Completed += new EventHandler(AnimationKeyFramesCameraPosition_Completed);

                if (animationKeyFramesCameraLookDirection != null)
                {
                    animationKeyFramesCameraLookDirection.Completed -= new EventHandler(AnimationKeyFramesCameraLookDirection_Completed);
                }

                animationKeyFramesCameraLookDirection = new Vector3DAnimationUsingKeyFrames();
                animationKeyFramesCameraLookDirection.Completed += new EventHandler(AnimationKeyFramesCameraLookDirection_Completed);

                animationKeyFramesBox.Duration = BlockMovementTime;
                animationFieldView.Duration = BlockMovementTime;
                animationKeyFramesCameraPosition.Duration = BlockMovementTime;
                animationKeyFramesCameraLookDirection.Duration = BlockMovementTime;

                animationKeyFramesBox.BeginTime = BlockBeginTime;
                animationFieldView.BeginTime = BlockBeginTime;
                animationKeyFramesCameraPosition.BeginTime = BlockBeginTime;
                animationKeyFramesCameraLookDirection.BeginTime = BlockBeginTime;

                Point3D cubeLocation = new Point3D(MovingCube.StartingPointCube.X, MovingCube.StartingPointCube.Y, MovingCube.StartingPointCube.Z);

                ///Check if Magnet can move in right direction
                bool canMove = false;
                int counter;
                int emptyPositionOrFloor = 0;

                switch (direction)
                {
                    case Direction.Up:
                        for (counter = MagnetFloorNo + 1; counter < Constants.NoofFloor; counter++)
                        {
                            if (position[counter][MagnetPositionOnFloor] == null)
                            {
                                emptyPositionOrFloor = counter;
                                canMove = true;
                                break;
                            }
                        }

                        break;
                    case Direction.Down:
                        for (counter = MagnetFloorNo - 1; counter >= 0; counter--)
                        {
                            if (position[counter][MagnetPositionOnFloor] == null)
                            {
                                emptyPositionOrFloor = counter;
                                canMove = true;
                                break;
                            }
                        }
                        break;

                    case Direction.Left:
                        int extremeLeftPos = MagnetPositionOnFloor - MagnetPositionOnFloor % Constants.BlocksInXdirection;

                        for (counter = MagnetPositionOnFloor - 1; counter >= extremeLeftPos; counter--)
                        {
                            if (position[MagnetFloorNo][counter] == null)
                            {
                                emptyPositionOrFloor = counter;
                                canMove = true;
                                break;
                            }
                        }

                        break;

                    case Direction.Right:
                        int extremeRightPos = MagnetPositionOnFloor + Constants.BlocksInXdirection - MagnetPositionOnFloor % Constants.BlocksInXdirection;

                        for (counter = MagnetPositionOnFloor + 1; counter < extremeRightPos; counter++)
                        {
                            if (position[MagnetFloorNo][counter] == null)
                            {
                                emptyPositionOrFloor = counter;
                                canMove = true;
                                break;
                            }
                        }
                        break;

                    case Direction.Front:
                        int extremeFrontPos = Constants.BlocksInXdirection * Constants.BlocksInZdirection;

                        for (counter = MagnetPositionOnFloor; counter < extremeFrontPos; counter = counter + Constants.BlocksInXdirection)
                        {
                            if (position[MagnetFloorNo][counter] == null)
                            {
                                emptyPositionOrFloor = counter;
                                canMove = true;
                                break;
                            }
                        }
                        break;
                    case Direction.Back:
                        int extremeBackPos = 0;

                        for (counter = MagnetPositionOnFloor; counter >= extremeBackPos; counter = counter - Constants.BlocksInXdirection)
                        {
                            if (position[MagnetFloorNo][counter] == null)
                            {
                                emptyPositionOrFloor = counter;
                                canMove = true;
                                break;
                            }
                        }

                        break;

                    default:
                        break;
                }

                if (canMove == true)
                {
                    this.StepCount++;

                    for (int i = 0; i < cubeWidthHeight; i++)
                    {
                        switch (direction)
                        {
                            case Direction.Up:
                                if (cubeLocation.Y <= Math.Ceiling((Constants.NoofFloor - 1) * cubeWidthHeight))
                                {
                                    cubeLocation.Y++;

                                    cameraPosition.Y = cameraPosition.Y + Constants.LeftRightCameraDelta;
                                    cameraLookDirection.Y = cameraLookDirection.Y - Constants.LeftRightCameraDelta;

                                    goto default;
                                }
                                break;
                            case Direction.Down:
                                if (cubeLocation.Y >= 0)
                                {
                                    cubeLocation.Y--;

                                    cameraPosition.Y = cameraPosition.Y - Constants.LeftRightCameraDelta;
                                    cameraLookDirection.Y = cameraLookDirection.Y + Constants.LeftRightCameraDelta;

                                    goto default;
                                }
                                break;

                            case Direction.Left:
                                if (cubeLocation.X > 0)
                                {
                                    cubeLocation.X--;

                                    cameraPosition.X = cameraPosition.X - Constants.LeftRightCameraDelta;
                                    cameraLookDirection.X = cameraLookDirection.X + Constants.LeftRightCameraDelta;

                                    goto default;
                                }
                                break;

                            case Direction.Right:
                                if (cubeLocation.X <= Math.Ceiling((Constants.BlocksInXdirection - 1) * cubeWidthHeight))
                                {
                                    cubeLocation.X++;

                                    cameraPosition.X = cameraPosition.X + Constants.LeftRightCameraDelta;
                                    cameraLookDirection.X = cameraLookDirection.X - Constants.LeftRightCameraDelta;

                                    goto default;
                                }
                                break;

                            case Direction.Front:
                                if (cubeLocation.Z <= Math.Ceiling(((Constants.BlocksInZdirection - 1)) * cubeWidthHeight))
                                {
                                    cubeLocation.Z++;
                                    cameraPosition.Z = cameraPosition.Z + 1;
                                    cameraLookDirection.Z = cameraLookDirection.Z - 1;

                                    cameraFieldOfView = cameraFieldOfView - .05;

                                    goto default;
                                }
                                break;
                            case Direction.Back:
                                if (cubeLocation.Z >= 0)
                                {
                                    cubeLocation.Z--;

                                    cameraPosition.Z = cameraPosition.Z - 1;
                                    cameraLookDirection.Z = cameraLookDirection.Z + 1;

                                    cameraFieldOfView = cameraFieldOfView + .05;

                                    goto default;
                                }

                                break;

                            default:
                                LinearPoint3DKeyFrame linear3dkeyframeBox = new LinearPoint3DKeyFrame(cubeLocation);
                                animationKeyFramesBox.KeyFrames.Add(linear3dkeyframeBox);

                                LinearPoint3DKeyFrame linear3dkeyframeCameraPosion = new LinearPoint3DKeyFrame(cameraPosition);

                                animationKeyFramesCameraPosition.KeyFrames.Add(linear3dkeyframeCameraPosion);

                                LinearVector3DKeyFrame linear3dkeyframeCameraLookDirection = new LinearVector3DKeyFrame(cameraLookDirection);

                                animationKeyFramesCameraLookDirection.KeyFrames.Add(linear3dkeyframeCameraLookDirection);

                                LinearDoubleKeyFrame linearDoubleKeyFrame = new LinearDoubleKeyFrame(cameraFieldOfView);

                                animationFieldView.KeyFrames.Add(linearDoubleKeyFrame);

                                break;
                        }
                    }

                    switch (direction)
                    {
                        case Direction.Up:
                            if (cubeLocation.Y <= Math.Ceiling((Constants.NoofFloor - 1) * cubeWidthHeight))
                            {
                                for (counter = emptyPositionOrFloor - 1; counter > MagnetFloorNo; counter--)
                                {
                                    this.MoveBlocks(position[counter][MagnetPositionOnFloor], 1, Direction.Up);
                                    position[counter + 1][MagnetPositionOnFloor] = position[counter][MagnetPositionOnFloor];
                                }

                                MagnetFloorNo++;
                                position[MagnetFloorNo][MagnetPositionOnFloor] = position[MagnetFloorNo - 1][MagnetPositionOnFloor];
                                position[MagnetFloorNo - 1][MagnetPositionOnFloor] = null;
                                goto default;
                            }

                            break;
                        case Direction.Down:
                            if (Math.Ceiling(cubeLocation.Y) >= 0)
                            {
                                for (counter = emptyPositionOrFloor + 1; counter < MagnetFloorNo; counter++)
                                {
                                    this.MoveBlocks(position[counter][MagnetPositionOnFloor], 1, Direction.Down);
                                    position[counter - 1][MagnetPositionOnFloor] = position[counter][MagnetPositionOnFloor];
                                }

                                position[MagnetFloorNo - 1][MagnetPositionOnFloor] = position[MagnetFloorNo][MagnetPositionOnFloor];
                                position[MagnetFloorNo][MagnetPositionOnFloor] = null;

                                MagnetFloorNo--;

                                goto default;
                            }

                            break;
                        case Direction.Left:
                            if (cubeLocation.X >= 0)
                            {
                                for (counter = emptyPositionOrFloor + 1; counter < MagnetPositionOnFloor; counter++)
                                {
                                    this.MoveBlocks(position[MagnetFloorNo][counter], 1, Direction.Left);

                                    position[MagnetFloorNo][counter - 1] = position[MagnetFloorNo][counter];
                                }

                                position[MagnetFloorNo][MagnetPositionOnFloor - 1] = position[MagnetFloorNo][MagnetPositionOnFloor];
                                position[MagnetFloorNo][MagnetPositionOnFloor] = null;

                                MagnetPositionOnFloor--;

                                goto default;
                            }

                            break;
                        case Direction.Right:

                            if (cubeLocation.X <= Math.Ceiling((Constants.BlocksInXdirection - 1) * cubeWidthHeight))
                            {
                                for (counter = emptyPositionOrFloor - 1; counter > MagnetPositionOnFloor; counter--)
                                {
                                    this.MoveBlocks(position[MagnetFloorNo][counter], 1, Direction.Right);

                                    position[MagnetFloorNo][counter + 1] = position[MagnetFloorNo][counter];
                                }

                                position[MagnetFloorNo][MagnetPositionOnFloor + 1] = position[MagnetFloorNo][MagnetPositionOnFloor];
                                position[MagnetFloorNo][MagnetPositionOnFloor] = null;

                                MagnetPositionOnFloor++;

                                goto default;
                            }

                            break;
                        case Direction.Front:
                            if (cubeLocation.Z <= Math.Ceiling(((Constants.BlocksInZdirection - 1)) * cubeWidthHeight))
                            {
                                for (counter = emptyPositionOrFloor - Constants.BlocksInXdirection; counter > MagnetPositionOnFloor; counter = counter - Constants.BlocksInXdirection)
                                {
                                    this.MoveBlocks(position[MagnetFloorNo][counter], 1, Direction.Front);

                                    position[MagnetFloorNo][counter + Constants.BlocksInXdirection] = position[MagnetFloorNo][counter];
                                }

                                position[MagnetFloorNo][MagnetPositionOnFloor + Constants.BlocksInXdirection] = position[MagnetFloorNo][MagnetPositionOnFloor];
                                position[MagnetFloorNo][MagnetPositionOnFloor] = null;

                                MagnetPositionOnFloor = MagnetPositionOnFloor + Constants.BlocksInXdirection;

                                goto default;
                            }
                            break;

                        case Direction.Back:
                            if (cubeLocation.Z >= 0)
                            {
                                for (counter = emptyPositionOrFloor + Constants.BlocksInXdirection; counter < MagnetPositionOnFloor; counter = counter + Constants.BlocksInXdirection)
                                {
                                    this.MoveBlocks(position[MagnetFloorNo][counter], 1, Direction.Back);

                                    position[MagnetFloorNo][counter - Constants.BlocksInXdirection] = position[MagnetFloorNo][counter];
                                }

                                position[MagnetFloorNo][MagnetPositionOnFloor - Constants.BlocksInXdirection] = position[MagnetFloorNo][MagnetPositionOnFloor];
                                position[MagnetFloorNo][MagnetPositionOnFloor] = null;

                                MagnetPositionOnFloor = MagnetPositionOnFloor - Constants.BlocksInXdirection;

                                goto default;
                            }

                            break;

                        default:

                            ApplyAnimation(animationKeyFramesBox, animationKeyFramesCameraPosition, animationKeyFramesCameraLookDirection, animationFieldView);
                            break;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnimationFieldView_Completed(object sender, EventArgs e)
        {
            fieldViewAnimationCompleted = true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnimationKeyFramesCameraLookDirection_Completed(object sender, EventArgs e)
        {
            positionAnimationCompleted = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnimationKeyFramesCameraPosition_Completed(object sender, EventArgs e)
        {
            cameraLookdirectionAnimationCompleted = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnimationKeyFramesBox_Completed(object sender, EventArgs e)
        {
            cubeAnimationCompleted = true;
            CheckCompletness();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveLeft(object sender, RoutedEventArgs e)
        {
            MoveCube(this.Magnet, Direction.Left);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveRight(object sender, RoutedEventArgs e)
        {
            MoveCube(this.Magnet, Direction.Right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveFront(object sender, RoutedEventArgs e)
        {
            MoveCube(this.Magnet, Direction.Front);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back(object sender, RoutedEventArgs e)
        {
            MoveCube(this.Magnet, Direction.Back);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="animationKeyFrames"></param>
        /// <param name="animationKeyFramesCameraPosition"></param>
        /// <param name="animationKeyFramesCameraLookDirection"></param>
        private void ApplyAnimation(Point3DAnimationUsingKeyFrames animationKeyFrames,
            Point3DAnimationUsingKeyFrames animationKeyFramesCameraPosition,
            Vector3DAnimationUsingKeyFrames animationKeyFramesCameraLookDirection,
            DoubleAnimationUsingKeyFrames animationKeyFramesFieldofView)
        {
            double factor = 5;

            List<Cube> cubeCollection = CalculateDistance(camera);

            for (int i = 0; i < cubeCollection.Count; i++)
            {
                DoubleAnimationUsingKeyFrames opacitykeyFrame = new DoubleAnimationUsingKeyFrames();

                double oldOpacity = 0;
                double newOpacity;
                double delta = 0;
                if ((cubeCollection[i].NewDistanceFromViewer - cubeCollection[i].OldDistanceFromViewer) > 0)
                {
                    oldOpacity = .8;
                    delta = (.1) / factor;
                }
                else
                {
                    oldOpacity = 1;
                    delta = -(.2) / factor;
                }

                for (int count = 1; count < factor; count++)
                {
                    newOpacity = oldOpacity + delta * count;
                    if (newOpacity > 0 && newOpacity <= 1)
                    {
                        LinearDoubleKeyFrame linearkeyFrame = new LinearDoubleKeyFrame(newOpacity);

                        opacitykeyFrame.KeyFrames.Add(linearkeyFrame);
                    }
                }

                cubeCollection[i].BeginAnimation(Cube.opacityProperty, opacitykeyFrame, HandoffBehavior.SnapshotAndReplace);
            }

            Magnet.BeginAnimation(Cube.StartingPointCubeProperty, animationKeyFrames, HandoffBehavior.Compose);

            this.ViewModelCamera.BeginAnimation(PerspectiveCamera.PositionProperty, animationKeyFramesCameraPosition, HandoffBehavior.Compose);
            this.ViewModelCamera.BeginAnimation(PerspectiveCamera.LookDirectionProperty, animationKeyFramesCameraLookDirection, HandoffBehavior.Compose);
            this.ViewModelCamera.BeginAnimation(PerspectiveCamera.FieldOfViewProperty, animationKeyFramesFieldofView, HandoffBehavior.Compose);

            this.cubeAnimationCompleted = false;
            this.positionAnimationCompleted = false;
            this.cameraLookdirectionAnimationCompleted = false;
            this.fieldViewAnimationCompleted = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        private List<Cube> CalculateDistance(PerspectiveCamera camera)
        {
            Cube cube;
            Vector3D vector;
            List<Cube> cubeCollection = new List<Cube>();
            for (int floor = 0; floor < Constants.NoofFloor; floor++)
            {
                for (int i = 0; i < Constants.BlocksInXdirection * Constants.BlocksInZdirection; i++)
                {
                    if (!((position[floor][i] == null) || (floor == MagnetFloorNo && i == MagnetPositionOnFloor)))
                    {
                        cube = position[floor][i] as Cube;
                        vector = Point3D.Subtract(camera.Position, cube.StartingPointCube);
                        cube.OldDistanceFromViewer = cube.NewDistanceFromViewer;
                        cube.NewDistanceFromViewer = vector.Length;
                        cubeCollection.Add(cube);
                    }
                }
            }

            return cubeCollection.OrderByDescending(x => x.NewDistanceFromViewer).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Magnet_Click(object sender, RoutedEventArgs e)
        {
            ////Find first cube in up direction
            int Step = 0;
            int counter;
            bool canMove = false;
            for (counter = MagnetFloorNo + 1; counter < Constants.NoofFloor; counter++)
            {
                if (position[counter][MagnetPositionOnFloor] != null)
                {
                    canMove = true;
                    break;
                }

                Step++;
            }

            if (canMove == true && Step > 0)
            {
                this.MoveBlocks(position[counter][MagnetPositionOnFloor], Step, Direction.Down);
                position[MagnetFloorNo + 1][MagnetPositionOnFloor] = position[counter][MagnetPositionOnFloor];
                position[counter][MagnetPositionOnFloor] = null;
            }

            ///Find cube in down direction
            counter = 0;
            canMove = false;
            Step = 0;
            for (counter = MagnetFloorNo - 1; counter >= 0; counter--)
            {
                if (position[counter][MagnetPositionOnFloor] != null)
                {
                    canMove = true;
                    break;
                }

                Step++;
            }

            if (canMove == true && Step > 0)
            {
                this.MoveBlocks(position[counter][MagnetPositionOnFloor], Step, Direction.Up);
                position[MagnetFloorNo - 1][MagnetPositionOnFloor] = position[counter][MagnetPositionOnFloor];
                position[counter][MagnetPositionOnFloor] = null;
            }

            ///Find cube in right direction
            counter = 0;
            canMove = false;
            Step = 0;
            int ExtremeRight = (MagnetPositionOnFloor + Constants.BlocksInXdirection) - (MagnetPositionOnFloor + Constants.BlocksInXdirection) % Constants.BlocksInXdirection;

            for (counter = MagnetPositionOnFloor + 1; counter < ExtremeRight; counter++)
            {
                if (position[MagnetFloorNo][counter] != null)
                {
                    canMove = true;
                    break;
                }

                Step++;
            }

            if (canMove == true && Step > 0)
            {
                this.MoveBlocks(position[MagnetFloorNo][counter], Step, Direction.Left);
                position[MagnetFloorNo][MagnetPositionOnFloor + 1] = position[MagnetFloorNo][counter];
                position[MagnetFloorNo][counter] = null;
            }

            ///Find cube in left direction
            counter = 0;
            canMove = false;
            Step = 0;
            int ExtremeLeft = MagnetPositionOnFloor - MagnetPositionOnFloor % Constants.BlocksInXdirection;

            for (counter = MagnetPositionOnFloor - 1; counter >= ExtremeLeft; counter--)
            {
                if (position[MagnetFloorNo][counter] != null)
                {
                    canMove = true;
                    break;
                }

                Step++;
            }

            if (canMove == true && Step > 0)
            {
                this.MoveBlocks(position[MagnetFloorNo][counter], Step, Direction.Right);
                position[MagnetFloorNo][MagnetPositionOnFloor - 1] = position[MagnetFloorNo][counter];
                position[MagnetFloorNo][counter] = null;
            }

            ///Find cube in front direction
            counter = 0;
            canMove = false;
            Step = 0;
            int ExtremeFront = Constants.BlocksInZdirection * Constants.BlocksInXdirection;

            for (counter = MagnetPositionOnFloor + Constants.BlocksInXdirection; counter < ExtremeFront; counter = counter + Constants.BlocksInXdirection)
            {
                if (position[MagnetFloorNo][counter] != null)
                {
                    canMove = true;
                    break;
                }

                Step++;
            }

            if (canMove == true && Step > 0)
            {
                this.MoveBlocks(position[MagnetFloorNo][counter], Step, Direction.Back);
                position[MagnetFloorNo][MagnetPositionOnFloor + Constants.BlocksInXdirection] = position[MagnetFloorNo][counter];
                position[MagnetFloorNo][counter] = null;
            }

            ///Find cube in back direction
            counter = 0;
            canMove = false;
            Step = 0;
            int ExtremeBack = -1;

            for (counter = MagnetPositionOnFloor - Constants.BlocksInXdirection; counter > ExtremeBack; counter = counter - Constants.BlocksInXdirection)
            {
                if (position[MagnetFloorNo][counter] != null)
                {
                    canMove = true;
                    break;
                }

                Step++;
            }

            if (canMove == true && Step > 0)
            {
                this.MoveBlocks(position[MagnetFloorNo][counter], Step, Direction.Front);
                position[MagnetFloorNo][MagnetPositionOnFloor - Constants.BlocksInXdirection] = position[MagnetFloorNo][counter];
                position[MagnetFloorNo][counter] = null;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CheckCompletness()
        {
            int cubesPerFloor = (int)((Constants.BlocksInXdirection) * (Constants.BlocksInZdirection));
            double cubeLength = Constants.CubeLength;

            //Check on left
            int extremLeftPos = MagnetPositionOnFloor - MagnetPositionOnFloor % Constants.BlocksInXdirection;
            bool pass = false;
            if (MagnetPositionOnFloor - 1 >= extremLeftPos && position[MagnetFloorNo][MagnetPositionOnFloor - 1] != null && position[MagnetFloorNo][MagnetPositionOnFloor - 1].color == Colors.Red)
            {
                int extremeRightPos = MagnetPositionOnFloor + Constants.BlocksInXdirection - MagnetPositionOnFloor % Constants.BlocksInXdirection;

                if (MagnetPositionOnFloor + 1 < extremeRightPos && position[MagnetFloorNo][MagnetPositionOnFloor + 1] != null && position[MagnetFloorNo][MagnetPositionOnFloor + 1].color == Colors.Blue)
                {
                    int extremeFrontPos = Constants.BlocksInXdirection * Constants.BlocksInZdirection;

                    if (MagnetPositionOnFloor + Constants.BlocksInXdirection <= extremeFrontPos && position[MagnetFloorNo][MagnetPositionOnFloor + Constants.BlocksInXdirection] != null && position[MagnetFloorNo][MagnetPositionOnFloor + Constants.BlocksInXdirection].color == Colors.Blue)
                    {
                        int extremeBackPos = 0;
                        if (MagnetPositionOnFloor - Constants.BlocksInXdirection >= extremeBackPos && position[MagnetFloorNo][MagnetPositionOnFloor - Constants.BlocksInXdirection] != null && position[MagnetFloorNo][MagnetPositionOnFloor - Constants.BlocksInXdirection].color == Colors.Green)
                        {
                            int downPos = 0;
                            if (MagnetFloorNo - 1 >= downPos && position[MagnetFloorNo - 1][MagnetPositionOnFloor] != null && position[MagnetFloorNo - 1][MagnetPositionOnFloor].color == Colors.Red)
                            {
                                int upPos = Constants.NoofFloor - 1;
                                if (MagnetFloorNo + 1 <= upPos && position[MagnetFloorNo + 1][MagnetPositionOnFloor] != null && position[MagnetFloorNo + 1][MagnetPositionOnFloor].color == Colors.Green)
                                {
                                    List<Cube> remove = new List<Cube>();

                                    Cube cube = position[MagnetFloorNo][MagnetPositionOnFloor - 1];
                                    this.CubesCollection.Remove(cube);
                                    remove.Add(cube);

                                    cube = position[MagnetFloorNo][MagnetPositionOnFloor + 1];
                                    this.CubesCollection.Remove(cube);
                                    remove.Add(cube);

                                    cube = position[MagnetFloorNo][MagnetPositionOnFloor + Constants.BlocksInXdirection];
                                    this.CubesCollection.Remove(cube);
                                    remove.Add(cube);

                                    cube = position[MagnetFloorNo][MagnetPositionOnFloor - Constants.BlocksInXdirection];
                                    this.CubesCollection.Remove(cube);
                                    remove.Add(cube);

                                    cube = position[MagnetFloorNo - 1][MagnetPositionOnFloor];
                                    this.CubesCollection.Remove(cube);
                                    remove.Add(cube);

                                    cube = position[MagnetFloorNo + 1][MagnetPositionOnFloor];
                                    this.CubesCollection.Remove(cube);
                                    remove.Add(cube);

                                    if (CollectionChanged != null)
                                    {
                                        CollectionChanged(remove, null);
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }
    }
}