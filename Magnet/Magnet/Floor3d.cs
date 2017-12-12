using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;

namespace Magnet
{
    /// <summary>
    /// 
    /// </summary>
 public   class Floor3d : Primitive3D
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

            for (int k = 0; k < 150; k++)
            {
                //ignorepositions.Add( rand.Next()% (6 *6 * 6) );
            }

            int xfactor = 5;
            int yfactor = 0;
            int zfactor = 5;
            int counter = 0;
            int space = 15;
        
            /// this.ViewPort3d.Children.Add(Circuit.CircuitBox(0 , 0 , 0 , 10));
            for (int i = -1 * xfactor; i <= 1 * xfactor; i++)
            {
              ///  for (int j = 0; j <= 0; j++)
                //for (int j = -1 * yfactor; j <= 1 * yfactor; j++)
                {
                    for (int k = -1 * zfactor; k <= 1 * zfactor + 1; k++)
                    {
                        if (!ignorepositions.Contains(counter++))
                        {
                           this.CreateTile(space * i, space * this.Level, space * k, 14, ref modelGroup);
                        }
                        //this.ViewPort3d.Children.Add(Circuit.CircuitBox(-20*i, 0, 0, 10));
                        //this.ViewPort3d.Children.Add(Circuit.CircuitBox(0*i, 0, 0, 10));
                        //this.ViewPort3d.Children.Add(Circuit.CircuitBox(20*i, 0, 0, 10));
                        //this.ViewPort3d.Children.Add(Circuit.CircuitBox(40*i, 0, 0, 10));

                    }
                }
            }

            return modelGroup;
        }

        private void CreateTile(double x, double y, double z, double widthHeightDepth, ref Model3DGroup model3dGroup)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(new Point3D(x, y, z));
            mesh.Positions.Add(new Point3D(x, y + widthHeightDepth, z));
            mesh.Positions.Add(new Point3D(x, y + widthHeightDepth, z + widthHeightDepth));
            mesh.Positions.Add(new Point3D(x, y, z + widthHeightDepth));

            mesh.Positions.Add(new Point3D(x + widthHeightDepth, y, z));
            mesh.Positions.Add(new Point3D(x + widthHeightDepth, y + widthHeightDepth, z));
            mesh.Positions.Add(new Point3D(x + widthHeightDepth, y + widthHeightDepth, z + widthHeightDepth));
            mesh.Positions.Add(new Point3D(x + widthHeightDepth, y, z + widthHeightDepth));

            ///front
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);

            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(6);

            /// right set up triangle indices
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(7);

            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(5);

            ///top
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(6);

            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(1);

            ///left
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(1);

            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(3);

            ///back
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(4);

            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);

            ///bottom
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(0);

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(7);

            /// 2 3 1  2 1 0  7 1 3  7 5 1  6 5 7  6 4 5  6 2 0  2 0 4  2 7 3  2 6 7  0 1 5  0 5 4

            // set up texture coords
            //mesh.TextureCoordinates.Add(new Point(0, 0));
            //mesh.TextureCoordinates.Add(new Point(1, 0));
            //mesh.TextureCoordinates.Add(new Point(0, 1));
            //mesh.TextureCoordinates.Add(new Point(1, 1));

            //mesh.TextureCoordinates.Add(new Point(0, 0));
            //mesh.TextureCoordinates.Add(new Point(0, 1));
            //mesh.TextureCoordinates.Add(new Point(-.5, .5));
            //mesh.TextureCoordinates.Add(new Point(-.7, -.5));

            // set up the brush

            SolidColorBrush brush = new SolidColorBrush(Colors.LightBlue); // new ImageBrush(new BitmapImage(new Uri(imagePath, UriKind.Relative)));

            SolidColorBrush backbrush = new SolidColorBrush(Colors.LightBlue);
            // create a geometry model based on the mesh and give it a material based on an image

            GeometryModel3D geom = new GeometryModel3D(mesh,this.Material);
            //GeometryModel3D geom = new GeometryModel3D(mesh, new EmissiveMaterial(brush));
           // geom.BackMaterial = this.BackMaterial;

            /// add the object
            model3dGroup.Children.Add(geom);

            //PointLight light = new PointLight();

            //light.Position = new Point3D(x + widthHeightDepth / 2, y + widthHeightDepth / 2, z + widthHeightDepth / 2);
            //light.Color = Colors.Blue;

            //// = new Vector3D(x + widthHeightDepth, y + widthHeightDepth, z + widthHeightDepth);

           /// model3dGroup.Children.Add(light);
        }
    }
}
