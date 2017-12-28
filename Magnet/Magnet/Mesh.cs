using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Magnet
{
    /// <summary>
    /// 
    /// </summary>
    class Mesh : Primitive3D
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override Geometry3D Tessellate()
        {
            this.Content = this.CreateMesh();
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Model3DGroup CreateMesh()
        {
            Model3DGroup modelGroup = new Model3DGroup();

            double cylinderRadius = Constants.MeshCylinderRadius;
            double cubeLength = Constants.CubeLength;

            SolidColorBrush colorBrush = new SolidColorBrush(Constants.MeshColor);
            colorBrush.Opacity = Constants.MeshCylinderOpacity;
            colorBrush.Freeze();

            ///Y direction pilliars
            for (double zDirCount = 0; zDirCount < Constants.BlocksInZdirection + 1; zDirCount = zDirCount + 1)
            {
                for (double xDirCount = 0; xDirCount < Constants.BlocksInXdirection + 1; xDirCount++)
                {
                    Cylinder3D cylinder = new Cylinder3D();
                    cylinder.Length = Math.Abs(Constants.NoofFloor * cubeLength);
                    cylinder.Radius = cylinderRadius;

                    cylinder.Material = new DiffuseMaterial(colorBrush);
                    cylinder.BackMaterial = new DiffuseMaterial(colorBrush);

                    TranslateTransform3D Transalte = new TranslateTransform3D();
                    Transalte.OffsetX = 0 + xDirCount * cubeLength;
                    Transalte.OffsetY = 0;
                    Transalte.OffsetZ = 0 + zDirCount * cubeLength;

                    cylinder._content.Transform = Transalte;
                    modelGroup.Children.Add(cylinder._content);
                }
            }

            //// x direction
            for (int levels = 0; levels <= Constants.NoofFloor; levels++)
            {
                for (double zDirCount = 0; zDirCount < Constants.BlocksInZdirection + 1; zDirCount = zDirCount + 1)
                {
                    Cylinder3D cylinder = new Cylinder3D();
                    cylinder.Length = Math.Abs(Constants.BlocksInXdirection * cubeLength);
                    cylinder.Radius = cylinderRadius;
                    cylinder.Material = new DiffuseMaterial(colorBrush);
                    cylinder.BackMaterial = new DiffuseMaterial(colorBrush);

                    TranslateTransform3D Transalte = new TranslateTransform3D();
                    Transalte.OffsetX = 0;
                    Transalte.OffsetY = 0 + levels * cubeLength;
                    Transalte.OffsetZ = 0 + zDirCount * cubeLength;

                    RotateTransform3D Rotate = new RotateTransform3D();
                    Rotate.Rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), -90);

                    Transform3DGroup myTransformGroup = new Transform3DGroup();
                    myTransformGroup.Children.Add(Rotate);
                    myTransformGroup.Children.Add(Transalte);
                    cylinder._content.Transform = myTransformGroup;
                    modelGroup.Children.Add(cylinder._content);
                }
            }

            ////// z direction
            for (int levels = 0; levels <= Constants.NoofFloor; levels++)
            {
                for (double xDirCount = 0; xDirCount < Constants.BlocksInXdirection + 1; xDirCount++)
                {
                    Cylinder3D cylinder = new Cylinder3D();

                    cylinder.Length = Math.Abs((Constants.BlocksInZdirection) * cubeLength);
                    cylinder.Radius = cylinderRadius;

                    cylinder.Material = new DiffuseMaterial(colorBrush);
                    cylinder.BackMaterial = new DiffuseMaterial(colorBrush);

                    TranslateTransform3D Transalte = new TranslateTransform3D();
                    Transalte.OffsetX = 0 + xDirCount * cubeLength;
                    Transalte.OffsetY = 0 + cubeLength * levels;
                    Transalte.OffsetZ = 0;

                    RotateTransform3D Rotate = new RotateTransform3D();
                    Rotate.Rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90);
                    Transform3DGroup myTransformGroup = new Transform3DGroup();

                    myTransformGroup.Children.Add(Rotate);
                    myTransformGroup.Children.Add(Transalte);

                    cylinder._content.Transform = myTransformGroup;
                    modelGroup.Children.Add(cylinder._content);
                }
            }
            return modelGroup;
        }
    }
}
