using System.Windows.Media;

namespace Magnet
{
    class Constants
    {
        /// <summary>
        /// Length, width,height of cube
        /// </summary>
        public const double CubeLength = 80;

        /// <summary>
        /// Length, width,height of cube in figure
        /// </summary>
        public const double TargetCubeLength = 30;

        /// <summary>
        /// No. of blocks in z direction
        /// </summary>
        public const int BlocksInZdirection = 5;

        /// <summary>
        /// No. of blocks in x direction
        /// </summary>
        public const int BlocksInXdirection = 5;

        /// <summary>
        /// No. of floors in mesh
        /// </summary>
        public const int NoofFloor = 5;

        /// <summary>
        /// Factor by which to change camera postion/lookdirection
        /// </summary>
        public const double LeftRightCameraDelta = 2;

        /// <summary>
        /// Start with 0.
        /// </summary>
        public const int MagnetInitialXPos = 2;
        public const int MagnetInitialYPos = 2;
        public const int MagnetInitialZPos = 4;

        public const double MeshCylinderRadius = 0.6;
        public const double MeshCylinderOpacity = 0.8;

        public static readonly Color MeshColor = Colors.Gray;
    }
}
