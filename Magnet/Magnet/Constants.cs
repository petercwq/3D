using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magnet
{
    class Constants
    {
        /// <summary>
        /// Length, width,height of cube
        /// </summary>
        public static double CubeLength = 100;

        /// <summary>
        /// Length, width,height of cube in figure
        /// </summary>
        public static double TargetCubeLength = 30;

        /// <summary>
        /// No. of blocks in z direction
        /// </summary>
        public static int BlocksInZdirection = 5;

        /// <summary>
        /// No. of blocks in x direction
        /// </summary>
        public static int BlocksInXdirection = 5;

        /// <summary>
        /// No. of floors in mesh
        /// </summary>
        public static int NoofFloor = 5;

        /// <summary>
        /// Factor by which to change camera postion/lookdirection
        /// </summary>
        public static double LeftRightCameraDelta = 1.5;

        /// <summary>
        /// Start with 0.
        /// </summary>
        public static int MagnetBlockXDirection = 2;
        public static int MagnetBlockYDirection = 2;
        public static int MagnetBlockZDirection = 4;
    }
}
