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
        TranslateTransform3D transalteback;
        TranslateTransform3D TransalteForCameraBackForYAxis
        {
            get
            {
                if (transalteback == null)
                {
                    transalteback = new TranslateTransform3D();
                    {
                        transalteback.OffsetX = 0;
                        transalteback.OffsetY = 0;
                        transalteback.OffsetZ = 0;
                    }
                }
                return transalteback;
            }
        }

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

            double cylinderRadius = 1;

            double cubeLength = Constants.CubeLength;

            SolidColorBrush colorBrush = new SolidColorBrush(Colors.Black);
            colorBrush.Opacity = 1;
            colorBrush.Freeze();
          
            double xCoordinate = 0;
            double yCoordinate = 0;
            double zCoordinate = 0;

            ///Y direction pilliars
            for (double zDirCount = 0; zDirCount < Constants.BlocksInZdirection + 1; zDirCount = zDirCount + 1)
            {
                for (double xDirCount = 0; xDirCount < Constants.BlocksInXdirection + 1; xDirCount++)
                {
                    xCoordinate = 0 + xDirCount *  cubeLength;
                    zCoordinate = 0 + zDirCount *  cubeLength;
                    yCoordinate = 0;

                    Cylinder3D cylinder = new Cylinder3D();

                    cylinder.Length = Math.Abs(Constants.NoofFloor * cubeLength);
                    cylinder.Radius = cylinderRadius;

                    cylinder.Material = new DiffuseMaterial(colorBrush);
                    cylinder.BackMaterial = new DiffuseMaterial(colorBrush);

                    TranslateTransform3D Transalte = new TranslateTransform3D();
                    Transalte.OffsetX = xCoordinate;
                    Transalte.OffsetY = yCoordinate;
                    Transalte.OffsetZ = zCoordinate;

                    cylinder._content.Transform = Transalte;
                    modelGroup.Children.Add(cylinder._content);
                }
            }

            //// x direction
            for (int levels = 0; levels <= Constants.NoofFloor; levels++)
            {
                for (double zDirCount = 0; zDirCount < Constants.BlocksInZdirection + 1; zDirCount = zDirCount + 1)
                {
                    zCoordinate = 0 + zDirCount * (cubeLength);

                    Cylinder3D cylinder = new Cylinder3D();

                    cylinder.Length = Math.Abs((Constants.BlocksInXdirection) * cubeLength + 1);
                    cylinder.Radius = cylinderRadius;
                    cylinder.Material = new DiffuseMaterial(colorBrush);
                    cylinder.BackMaterial = new DiffuseMaterial(colorBrush);

                    TranslateTransform3D Transalte = new TranslateTransform3D();

                    Transalte.OffsetX = xCoordinate + 1;
                    Transalte.OffsetY = yCoordinate + cubeLength * levels;
                    Transalte.OffsetZ = zCoordinate;

                    RotateTransform3D Rotate = new RotateTransform3D();
                    Vector3D vector3d = new Vector3D(0, 0, 1);
                    Rotate.Rotation = new AxisAngleRotation3D(vector3d, 90);

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
                 
                    xCoordinate = 0 + xDirCount * cubeLength;

                    Cylinder3D cylinder = new Cylinder3D();

                    cylinder.Length = Math.Abs((Constants.BlocksInZdirection) * cubeLength);
                    cylinder.Radius = cylinderRadius;

                    cylinder.Material = new DiffuseMaterial(colorBrush);
                    cylinder.BackMaterial = new DiffuseMaterial(colorBrush);

                    TranslateTransform3D Transalte = new TranslateTransform3D();
                    Transalte.OffsetX = xCoordinate;
                    Transalte.OffsetY = yCoordinate + cubeLength * levels;

                    Transalte.OffsetZ = zCoordinate - cylinder.Length;

                    RotateTransform3D Rotate = new RotateTransform3D();

                    Vector3D vector3d = new Vector3D(1, 0, 0);

                    Rotate.Rotation = new AxisAngleRotation3D(vector3d, 90);
                    Transform3DGroup myTransformGroup = new Transform3DGroup();

                    myTransformGroup.Children.Add(Rotate);
                    myTransformGroup.Children.Add(Transalte);

                    cylinder._content.Transform = myTransformGroup;
                    modelGroup.Children.Add(cylinder._content);
                }
            }

            RotateTransform3D rotateMesh = new RotateTransform3D();
            Vector3D vector3dMesh = new Vector3D(0, 1, 0);
            rotateMesh.Rotation = new AxisAngleRotation3D(vector3dMesh, 0);

            Transform3DGroup transformGroupMesh = new Transform3DGroup();
         
            transformGroupMesh.Children.Add(rotateMesh);
            transformGroupMesh.Children.Add(TransalteForCameraBackForYAxis);

            return modelGroup;
        }
    }
}
