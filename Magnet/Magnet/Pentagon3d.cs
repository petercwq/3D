using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace Magnet
{
    //enum Axis
    //{
    //    East,
    //    NorthEast,
    //    North,
    //    NorthWest,
    //    West,
    //    SouthWest,
    //    South,
    //    SouthWest
    //}

    /// <summary>
    /// 
    /// </summary>
    class Pentagon3d : Primitive3D
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override Geometry3D Tessellate()
        {
            this.Content = CreateFloor();
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Model3DGroup CreateFloor()
        {
            Model3DGroup modelGroup = new Model3DGroup();
            List<int> ignorepositions = new List<int>();

            Random rand = new Random();

            double cubeLength = 15.5;
            double BASE = Math.Cos(Math.PI / 6) * cubeLength;
            double altitude = Math.Sin(Math.PI / 6) * cubeLength;

            double pentagonInXdirection = 10;
            double pentagonInZdirection = 10;

            double xCorrection = 0;

            for (double zCoor = 0; zCoor <= pentagonInZdirection; zCoor++)
            {
                if (zCoor % 2 != 0)
                {
                    xCorrection = BASE;
                }
                else
                {
                    xCorrection = 0;
                }

                for (double xCoor = 0; xCoor < pentagonInXdirection; xCoor++)
                {
                    this.CreatePentagon(xCoor * 2 * BASE + xCorrection, cubeLength, 0 + zCoor * (altitude + cubeLength), ref modelGroup);
                }
            }

            return modelGroup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="model3dGroup"></param>
        private void CreatePentagon(double x, double y, double z, ref Model3DGroup model3dGroup)
        {
            model3dGroup.Children.Add(GetModel3D(x,y,z,14,1.5,Colors.Blue));
            model3dGroup.Children.Add(GetModel3D(x, y, z, 15, 0, Colors.Red)); 
         
            //PointLight light = new PointLight();

            //light.Position = new Point3D(x + widthHeightDepth / 2, y + widthHeightDepth / 2, z + widthHeightDepth / 2);
            //light.Color = Colors.Blue;

            //// = new Vector3D(x + widthHeightDepth, y + widthHeightDepth, z + widthHeightDepth);

            /// model3dGroup.Children.Add(light);
        }

        private GeometryModel3D GetModel3D(double x, double y, double z, double cubeLengthOutline, double correction, Color color)
        {
            double BASEOutline = Math.Cos(Math.PI / 6) * cubeLengthOutline;
            double ALTITUDEOutline = Math.Sin(Math.PI / 6) * cubeLengthOutline;

            MeshGeometry3D meshOutline = new MeshGeometry3D();
            meshOutline.Positions.Add(new Point3D(x, y, z + correction));//0
            meshOutline.Positions.Add(new Point3D(x - BASEOutline, y, z + ALTITUDEOutline));//1
            meshOutline.Positions.Add(new Point3D(x - BASEOutline, y, z + ALTITUDEOutline + cubeLengthOutline));//2
            meshOutline.Positions.Add(new Point3D(x, y, z + 2 * ALTITUDEOutline + cubeLengthOutline));//3

            meshOutline.Positions.Add(new Point3D(x + BASEOutline, y, z + ALTITUDEOutline + cubeLengthOutline));//4
            meshOutline.Positions.Add(new Point3D(x + BASEOutline, y, z + ALTITUDEOutline));//5

            double height = cubeLengthOutline/2;
            meshOutline.Positions.Add(new Point3D(x, y + height, z + correction));//0
            meshOutline.Positions.Add(new Point3D(x - BASEOutline, y + height, z + ALTITUDEOutline));//1
            meshOutline.Positions.Add(new Point3D(x - BASEOutline, y + height, z + ALTITUDEOutline + cubeLengthOutline));//2
            meshOutline.Positions.Add(new Point3D(x, y + height, z + 2 * ALTITUDEOutline + cubeLengthOutline));//3

            meshOutline.Positions.Add(new Point3D(x + BASEOutline, y + height, z + ALTITUDEOutline + cubeLengthOutline));//4
            meshOutline.Positions.Add(new Point3D(x + BASEOutline, y + height, z + ALTITUDEOutline));//5
            
            meshOutline.TriangleIndices.Add(0);
            meshOutline.TriangleIndices.Add(1);
            meshOutline.TriangleIndices.Add(5);

            meshOutline.TriangleIndices.Add(5);
            meshOutline.TriangleIndices.Add(1);
            meshOutline.TriangleIndices.Add(4);

            /// right set up triangle indices
            meshOutline.TriangleIndices.Add(4);
            meshOutline.TriangleIndices.Add(1);
            meshOutline.TriangleIndices.Add(2);

            meshOutline.TriangleIndices.Add(4);
            meshOutline.TriangleIndices.Add(2);
            meshOutline.TriangleIndices.Add(3);

            /////////////////////////////////////////////
            //meshOutline.TriangleIndices.Add(6);
            //meshOutline.TriangleIndices.Add(0);
            //meshOutline.TriangleIndices.Add(1);
           
            //meshOutline.TriangleIndices.Add(1);
            //meshOutline.TriangleIndices.Add(7);
            //meshOutline.TriangleIndices.Add(6);
            ////
            //meshOutline.TriangleIndices.Add(7);
            //meshOutline.TriangleIndices.Add(1);
            //meshOutline.TriangleIndices.Add(2);

            //meshOutline.TriangleIndices.Add(2);
            //meshOutline.TriangleIndices.Add(8);
            //meshOutline.TriangleIndices.Add(7);
            ////
            //meshOutline.TriangleIndices.Add(8);
            //meshOutline.TriangleIndices.Add(2);
            //meshOutline.TriangleIndices.Add(3);
            
            //meshOutline.TriangleIndices.Add(3);
            //meshOutline.TriangleIndices.Add(9);
            //meshOutline.TriangleIndices.Add(8);
            ////
            //meshOutline.TriangleIndices.Add(9);
            //meshOutline.TriangleIndices.Add(3);
            //meshOutline.TriangleIndices.Add(4);
            
            //meshOutline.TriangleIndices.Add(4);
            //meshOutline.TriangleIndices.Add(10);
            //meshOutline.TriangleIndices.Add(9);
            /////
            //meshOutline.TriangleIndices.Add(10);
            //meshOutline.TriangleIndices.Add(4);
            //meshOutline.TriangleIndices.Add(5);
           
            //meshOutline.TriangleIndices.Add(5);
            //meshOutline.TriangleIndices.Add(11);
            //meshOutline.TriangleIndices.Add(10);
          
            /////
            //meshOutline.TriangleIndices.Add(6);
            //meshOutline.TriangleIndices.Add(11);
            //meshOutline.TriangleIndices.Add(5);
            
            //meshOutline.TriangleIndices.Add(5);
            //meshOutline.TriangleIndices.Add(0);
            //meshOutline.TriangleIndices.Add(6);
            ///////

            //meshOutline.TriangleIndices.Add(6);
            //meshOutline.TriangleIndices.Add(7);
            //meshOutline.TriangleIndices.Add(11);

            //meshOutline.TriangleIndices.Add(8);
            //meshOutline.TriangleIndices.Add(9);
            //meshOutline.TriangleIndices.Add(10);


            //meshOutline.TriangleIndices.Add(7);
            //meshOutline.TriangleIndices.Add(8);
            //meshOutline.TriangleIndices.Add(10);

            //meshOutline.TriangleIndices.Add(11);
            //meshOutline.TriangleIndices.Add(7);
            //meshOutline.TriangleIndices.Add(10);

            meshOutline.Normals.Add(new Vector3D(0, 1, 5));
            meshOutline.Normals.Add(new Vector3D(5, 1, 4));
            meshOutline.Normals.Add(new Vector3D(4, 1, 2));
            meshOutline.Normals.Add(new Vector3D(4, 2, 3));

            SolidColorBrush brushoutline = new SolidColorBrush(color);
            GeometryModel3D geomOutline = new GeometryModel3D(meshOutline, new EmissiveMaterial(brushoutline));
            geomOutline.BackMaterial = new EmissiveMaterial(new SolidColorBrush(Colors.Transparent));

            return geomOutline;
        }
    }
}
