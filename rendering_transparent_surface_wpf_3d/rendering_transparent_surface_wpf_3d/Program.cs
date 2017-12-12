using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;

namespace rendering_transparent_surface_wpf_3d
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Window qWindow = new Window();
            qWindow.Title = "Transparent Rendering";
            qWindow.Width = 400;
            qWindow.Height = 300;
            qWindow.Content = TransparentScene();
            qWindow.ShowDialog();
        }

        static Viewport3D TransparentScene()
        {
            // Define the camera
            PerspectiveCamera qCamera = new PerspectiveCamera();
            qCamera.Position = new Point3D(0, .25, 2.25);
            qCamera.LookDirection = new Vector3D(0, -.05, -1);
            qCamera.UpDirection = new Vector3D(0, 1, 0);
            qCamera.FieldOfView = 60;

            // Define a lighting model
            DirectionalLight qLight = new DirectionalLight();
            qLight.Color = Colors.White;
            qLight.Direction = new Vector3D(-0.5, -0.25, -0.5);

            // Define the animated rotation transformation
            RotateTransform3D qRotation =
                new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 1));
            DoubleAnimation qAnimation = new DoubleAnimation();
            qAnimation.From = 1;
            qAnimation.To = 361;
            qAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(5000));
            qAnimation.RepeatBehavior = RepeatBehavior.Forever;
            qRotation.Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, qAnimation);

            // Define the geometry
            const double kdSqrt2 = 1.4142135623730950488016887242097;
            const double kdSqrt6 = 2.4494897427831780981972840747059;
            // Create a collection of vertex positions
            Point3D[] qaV = new Point3D[4]{
                new Point3D(0.0, 1.0, 0.0),
                new Point3D(2.0 * kdSqrt2 / 3.0, -1.0 / 3.0, 0.0),
                new Point3D(-kdSqrt2 / 3.0, -1.0 / 3.0, -kdSqrt6 / 3.0),
                new Point3D(-kdSqrt2 / 3.0, -1.0 / 3.0, kdSqrt6 / 3.0)};
            Point3DCollection qPoints = new Point3DCollection();
            // Designate Vertices
            // My Scheme (0, 1, 2), (1, 0, 3), (2, 3, 0), (3, 2, 1)
            for (int i = 0; i < 12; ++i)
            {
                if ((i / 3) % 2 == 0)
                {
                    qPoints.Add(qaV[i % 4]);
                }
                else
                {
                    qPoints.Add(qaV[(i * 3) % 4]);
                }
            }
            // Designate Triangles
            Int32Collection qTriangles = new Int32Collection();
            for (int i = 0; i < 12; ++i)
            {
                qTriangles.Add(i);
            }
            Int32Collection qBackTriangles = new Int32Collection();
            // Designate Back Triangles in the opposite orientation
            for (int i = 0; i < 12; ++i)
            {
                qBackTriangles.Add(3 * (i / 3) + (2 * (i % 3) % 3));
            }

            // Inner Tetrahedron: Define the mesh, material and transformation.
            MeshGeometry3D qFrontMesh = new MeshGeometry3D();
            qFrontMesh.Positions = qPoints;
            qFrontMesh.TriangleIndices = qTriangles;
            GeometryModel3D qInnerGeometry = new GeometryModel3D();
            qInnerGeometry.Geometry = qFrontMesh;
            // *** Material ***
            DiffuseMaterial qDiffGreen =
                new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(255, 0, 128, 0)));
            SpecularMaterial qSpecWhite = new
                SpecularMaterial(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)), 30.0);
            MaterialGroup qInnerMaterial = new MaterialGroup();
            qInnerMaterial.Children.Add(qDiffGreen);
            qInnerMaterial.Children.Add(qSpecWhite);
            qInnerGeometry.Material = qInnerMaterial;
            // *** Transformation ***
            ScaleTransform3D qScale = new ScaleTransform3D(new Vector3D(.5, .5, .5));
            Transform3DGroup myTransformGroup = new Transform3DGroup();
            myTransformGroup.Children.Add(qRotation);
            myTransformGroup.Children.Add(qScale);
            qInnerGeometry.Transform = myTransformGroup;

            // Outer Tetrahedron (semi-transparent) : Define the mesh, material and transformation.
            GeometryModel3D qOuterGeometry = new GeometryModel3D();
            qOuterGeometry.Geometry = qFrontMesh;
            // *** Material ***
            DiffuseMaterial qDiffTransYellow =
                new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(64, 255, 255, 0)));
            SpecularMaterial qSpecTransWhite =
                new SpecularMaterial(new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)), 30.0);
            MaterialGroup qOuterMaterial = new MaterialGroup();
            qOuterMaterial.Children.Add(qDiffTransYellow);
            qOuterMaterial.Children.Add(qSpecTransWhite);
            qOuterGeometry.Material = qOuterMaterial;
            // *** Transformation ***
            qOuterGeometry.Transform = qRotation;

            // Outer Tetrahedron (solid back) : Define the mesh, material and transformation.
            MeshGeometry3D qBackMesh = new MeshGeometry3D();
            qBackMesh.Positions = qPoints;
            qBackMesh.TriangleIndices = qBackTriangles;
            GeometryModel3D qBackGeometry = new GeometryModel3D();
            qBackGeometry.Geometry = qBackMesh;
            // *** Material ***
            DiffuseMaterial qDiffBrown =
                new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(255, 200, 175, 0)));
            qBackGeometry.Material = qDiffBrown;
            // *** Transformation ***
            qBackGeometry.Transform = qRotation;

            // Collect the components
            Model3DGroup qModelGroup = new Model3DGroup();
            qModelGroup.Children.Add(qLight);
            qModelGroup.Children.Add(qBackGeometry);
            qModelGroup.Children.Add(qInnerGeometry);
            qModelGroup.Children.Add(qOuterGeometry);
            ModelVisual3D qVisual = new ModelVisual3D();
            qVisual.Content = qModelGroup;
            Viewport3D qViewport = new Viewport3D();
            qViewport.Children.Add(qVisual);
            qViewport.Camera = qCamera;

            return qViewport;
        }
    }
}