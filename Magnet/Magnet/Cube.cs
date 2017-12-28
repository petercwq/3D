using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Magnet
{
    /// <summary>
    /// 
    /// </summary>
    class Cube : Base3D
    {
        public double NewDistanceFromViewer = 0;
        public double OldDistanceFromViewer = 0;

        public bool IsMovingCube = false;

        internal override Geometry3D Draw()
        {
            Model3DGroup modelGroup = new Model3DGroup();
            double correction = Constants.MeshCylinderRadius;
            Point3D point3d = new Point3D(this.StartingPointCube.X + correction, this.StartingPointCube.Y + correction, this.StartingPointCube.Z + correction);

            double widthHeightDepth = base.WidthHeightDepth - correction;

            // add a light
            /// modelGroup.Children.Add(new AmbientLight());

            // set up the mesh
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(new Point3D(point3d.X, point3d.Y, point3d.Z));
            mesh.Positions.Add(new Point3D(point3d.X, point3d.Y + widthHeightDepth, point3d.Z));
            mesh.Positions.Add(new Point3D(point3d.X, point3d.Y + widthHeightDepth, point3d.Z + widthHeightDepth));
            mesh.Positions.Add(new Point3D(point3d.X, point3d.Y, point3d.Z + widthHeightDepth));

            mesh.Positions.Add(new Point3D(point3d.X + widthHeightDepth, point3d.Y, point3d.Z));
            mesh.Positions.Add(new Point3D(point3d.X + widthHeightDepth, point3d.Y + widthHeightDepth, point3d.Z));
            mesh.Positions.Add(new Point3D(point3d.X + widthHeightDepth, point3d.Y + widthHeightDepth, point3d.Z + widthHeightDepth));
            mesh.Positions.Add(new Point3D(point3d.X + widthHeightDepth, point3d.Y, point3d.Z + widthHeightDepth));

            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);

            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(7);

            /// set up triangle indices
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(7);

            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(5);

            ///
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(6);

            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(1);

            ///back surface start
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(3);

            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(3);

            ///
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(4);

            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);

            ///
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(0);

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(7);

            /// 6 2 3  6 3 7  5 6 7  7 4 5  1 2 6  6 5 1  2 1 3  1 0 3  1 5 4  4 0 1  7 3 0  0 4 7 

            // set up texture coords
            mesh.TextureCoordinates.Add(new Point(1, 0));
            mesh.TextureCoordinates.Add(new Point(0, 0));
            mesh.TextureCoordinates.Add(new Point(0, 1));
            mesh.TextureCoordinates.Add(new Point(1, 1));

            //set up the brush

            SolidColorBrush brush = new SolidColorBrush(color); // new ImageBrush(new BitmapImage(new Uri(imagePath, UriKind.Relative)));
            brush.Opacity = this.opacity;
            brush.Freeze();
            mesh.Freeze();
            GeometryModel3D geom;
            if (this.IsMovingCube == true)
            {
                PointLight light = new PointLight();
                light.Position = new Point3D(point3d.X + widthHeightDepth, point3d.Y + widthHeightDepth, point3d.Z + widthHeightDepth);
                light.Color = Colors.Red;
                modelGroup.Children.Add(light);
                geom = new GeometryModel3D(mesh, new DiffuseMaterial(brush));
            }
            else
            {
                geom = new GeometryModel3D(mesh, new DiffuseMaterial(brush));
            }
            ///  geom.BackMaterial = new DiffuseMaterial(brush);

            /// add the object
            modelGroup.Children.Add(geom);

            this.Content = modelGroup;

            return null;
        }
    }
}
