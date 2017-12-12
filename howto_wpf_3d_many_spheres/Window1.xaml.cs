using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Media.Media3D;

namespace howto_wpf_3d_many_spheres
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        // The main object model group.
        private Model3DGroup MainModelGroup = new Model3DGroup();

        // Lights.
        private List<Light> Lights = new List<Light>();

        // The camera.
        private PerspectiveCamera TheCamera;

        // The camera's current location.
        private double CameraPhi, CameraTheta, CameraR;

        // The level of division.
        private int Level = 1;

        // The radius of the stellate peaks.
        private double StellateR = 0.96;

        // Total number of triangles.
        private int TotalTriangles = 0;

        // Create the scene.
        // MainViewport is the Viewport3D defined
        // in the XAML code that displays everything.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Give the camera its initial position.
            TheCamera = new PerspectiveCamera();
            TheCamera.FieldOfView = 60;
            MainViewport.Camera = TheCamera;
            CameraTheta = scrTheta.Value * Math.PI / 180.0;
            CameraPhi = scrPhi.Value * Math.PI / 180.0;
            CameraR = double.Parse(lblDistance.Content.ToString());
            PositionCamera();

            // Define lights.
            DefineLights();

            // Define the models.
            DefineModels();

            Console.WriteLine("Total triangles: " + TotalTriangles);

            // Add the group of models to a ModelVisual3D.
            ModelVisual3D model_visual = new ModelVisual3D();
            model_visual.Content = MainModelGroup;

            // Display the main visual to the viewportt.
            MainViewport.Children.Add(model_visual);
        }

        // Define the lights.
        private void DefineLights()
        {
            Color color64 = Color.FromArgb(255, 128, 128, 64);
            Color color128 = Color.FromArgb(255, 255, 255, 128);
            Lights.Add(new AmbientLight(color64));
            Lights.Add(new DirectionalLight(color64,
                new Vector3D(-1.0, -2.0, -3.0)));
            Lights.Add(new DirectionalLight(color64,
                new Vector3D(1.0, 2.0, 3.0)));

            foreach (Light light in Lights)
                MainModelGroup.Children.Add(light);
        }

        // Return triangles that define a stellate sphere.
        private Triangle[] MakeTriangles(double side_length)
        {
            // Get the vertices.
            Point3D[] points = MakeVertices(side_length);

            // Make the triangles.
            List<Triangle> triangles = new List<Triangle>();
            triangles.Add(new Triangle(points[0], points[2], points[1]));
            triangles.Add(new Triangle(points[0], points[3], points[2]));
            triangles.Add(new Triangle(points[0], points[4], points[3]));
            triangles.Add(new Triangle(points[0], points[5], points[4]));
            triangles.Add(new Triangle(points[0], points[1], points[5]));

            triangles.Add(new Triangle(points[1], points[2], points[9]));
            triangles.Add(new Triangle(points[2], points[3], points[10]));
            triangles.Add(new Triangle(points[3], points[4], points[6]));
            triangles.Add(new Triangle(points[4], points[5], points[7]));
            triangles.Add(new Triangle(points[5], points[1], points[8]));

            triangles.Add(new Triangle(points[6], points[4], points[7]));
            triangles.Add(new Triangle(points[7], points[5], points[8]));
            triangles.Add(new Triangle(points[8], points[1], points[9]));
            triangles.Add(new Triangle(points[9], points[2], points[10]));
            triangles.Add(new Triangle(points[10], points[3], points[6]));

            triangles.Add(new Triangle(points[11], points[6], points[7]));
            triangles.Add(new Triangle(points[11], points[7], points[8]));
            triangles.Add(new Triangle(points[11], points[8], points[9]));
            triangles.Add(new Triangle(points[11], points[9], points[10]));
            triangles.Add(new Triangle(points[11], points[10], points[6]));

            // The radius is the distance from
            // the top point to the origin.
            double radius = points[0].Y;

            // Divide the triangles if desired
            // to make the geodesic sphere.
            Point3D origin = new Point3D(0, 0, 0);
            for (int i = 0; i < Level; i++)
            {
                List<Triangle> new_triangles = new List<Triangle>();
                foreach (Triangle triangle in triangles)
                {
                    triangle.Subdivide(new_triangles, origin, radius);
                }
                triangles = new_triangles;
            }

            // Stellate.
            List<Triangle> stellate_triangles = new List<Triangle>();
            foreach (Triangle triangle in triangles)
            {
                triangle.Stellate(stellate_triangles, origin, StellateR);
            }
            triangles = stellate_triangles;

            return triangles.ToArray();
        }

        // Return the vertices for an icosahedron.
        private Point3D[] MakeVertices(double side_length)
        {
            // t1 and t3 are actually not used in calculations.
            double S = side_length;
            //double t1 = 2.0 * Math.PI / 5;
            double t2 = Math.PI / 10.0;
            double t4 = Math.PI / 5.0;
            //double t3 = -3.0 * Math.PI / 10.0;
            double R = (S / 2.0) / Math.Sin(t4);
            double H = Math.Cos(t4) * R;
            double Cx = R * Math.Sin(t2);
            double Cz = R * Math.Cos(t2);
            double H1 = Math.Sqrt(S * S - R * R);
            double H2 = Math.Sqrt((H + R) * (H + R) - H * H);
            double Y2 = (H2 - H1) / 2.0;
            double Y1 = Y2 + H1;

            List<Point3D> points = new List<Point3D>();
            points.Add(new Point3D(0, Y1, 0));
            points.Add(new Point3D(R, Y2, 0));
            points.Add(new Point3D(Cx, Y2, Cz));
            points.Add(new Point3D(-H, Y2, S / 2));
            points.Add(new Point3D(-H, Y2, -S / 2));
            points.Add(new Point3D(Cx, Y2, -Cz));
            points.Add(new Point3D(-R, -Y2, 0));
            points.Add(new Point3D(-Cx, -Y2, -Cz));
            points.Add(new Point3D(H, -Y2, -S / 2));
            points.Add(new Point3D(H, -Y2, S / 2));
            points.Add(new Point3D(-Cx, -Y2, Cz));
            points.Add(new Point3D(0, -Y1, 0));

            return points.ToArray();
        }

        // Create the solid and skeleton models.
        private void DefineModels()
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            Cursor = Cursors.Wait;

            // Make some spheres.
            const int max = 5;
            const int xmin = -max;
            const int xmax = max;
            const int ymin = -max;
            const int ymax = max;
            const int zmin = -max;
            const int zmax = max;
            double dr = 255 / (xmax - xmin);
            double dg = 255 / (ymax - ymin);
            double db = 255 / (zmax - zmin);

            for (int x = xmin; x <= xmax; x++)
            {
                // Make sure the program doesn't think it's stuck.
                Application.Current.Dispatcher.Invoke(
                    System.Windows.Threading.DispatcherPriority.Background,
                    new Action(delegate { }));

                for (int y = ymin; y <= ymax; y++)
                {
                    for (int z = zmin; z <= zmax; z++)
                    {
                        // Get the stellate sphere's triangles.
                        Triangle[] triangles = MakeTriangles(1);

                        // Create the WPF triangles for the stellate sphere.
                        MeshGeometry3D solid_mesh = new MeshGeometry3D();
                        foreach (Triangle triangle in triangles)
                        {
                            AddTriangle(solid_mesh, triangle);
                        }

                        // Make the sphere's material.
                        byte r = (byte)((x - xmin) * dr);
                        byte g = (byte)((y - ymin) * dg);
                        byte b = (byte)((z - zmin) * db);
                        Color color = Color.FromArgb(255, r, g, b);
                        SolidColorBrush solid_brush = new SolidColorBrush(color);
                        DiffuseMaterial solid_material = new DiffuseMaterial(solid_brush);
                        GeometryModel3D solid_model =
                            new GeometryModel3D(solid_mesh, solid_material);

                        // Scale to make it smaller.
                        const double scale = 0.4;
                        ScaleTransform3D scale_transform =
                            new ScaleTransform3D(
                                scale, scale, scale,
                                0, 0, 0);

                        // Translate to center at (x, y, z).
                        TranslateTransform3D translate_transform =
                            new TranslateTransform3D(x, y, z);

                        // Transform the model.
                        Transform3DGroup transform_group = new Transform3DGroup();
                        transform_group.Children.Add(scale_transform);
                        transform_group.Children.Add(translate_transform);
                        solid_model.Transform = transform_group;

                        MainModelGroup.Children.Add(solid_model);
                    }
                }
            }

            Cursor = null;

            // Display the startup time.
            watch.Stop();
            Console.WriteLine(watch.Elapsed.TotalSeconds.ToString("0.00"));
        }

        // Add a rectangle to the indicated mesh.
        // Do not reuse existing points but reuse these points
        // so new rectangles don't share normals with old ones.
        private void AddRectangle(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Point3D point3, Point3D point4)
        {
            // Create the points.
            int index1 = mesh.Positions.Count;
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);
            mesh.Positions.Add(point4);

            // Create the triangles.
            mesh.TriangleIndices.Add(index1);
            mesh.TriangleIndices.Add(index1 + 1);
            mesh.TriangleIndices.Add(index1 + 2);

            mesh.TriangleIndices.Add(index1);
            mesh.TriangleIndices.Add(index1 + 2);
            mesh.TriangleIndices.Add(index1 + 3);
        }

        // Add a triangle to the indicated mesh.
        // Do not reuse points so triangles don't share normals.
        private void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {
            // Create the points.
            int index1 = mesh.Positions.Count;
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1);
        }

        // Add a triangle (from a Triangle) to the indicated mesh.
        // Do not reuse points so triangles don't share normals.
        private void AddTriangle(MeshGeometry3D mesh, Triangle triangle)
        {
            TotalTriangles++;

            // Create the points.
            int index1 = mesh.Positions.Count;
            mesh.Positions.Add(triangle.Points[0]);
            mesh.Positions.Add(triangle.Points[1]);
            mesh.Positions.Add(triangle.Points[2]);

            // Create the triangle.
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1);
        }

        // Make a thin rectangular prism between the two points.
        private void AddSegment(MeshGeometry3D mesh, Point3D point1, Point3D point2, Vector3D up)
        {
            const double thickness = 0.01;

            // Get the segment's vector.
            Vector3D v = point2 - point1;

            // Get the scaled up vector.
            Vector3D n1 = ScaleVector(up, thickness / 2.0);

            // Get another scaled perpendicular vector.
            Vector3D n2 = Vector3D.CrossProduct(v, n1);
            n2 = ScaleVector(n2, thickness / 2.0);

            // Make a skinny box.
            // p1pm means point1 PLUS n1 MINUS n2.
            Point3D p1pp = point1 + n1 + n2;
            Point3D p1mp = point1 - n1 + n2;
            Point3D p1pm = point1 + n1 - n2;
            Point3D p1mm = point1 - n1 - n2;
            Point3D p2pp = point2 + n1 + n2;
            Point3D p2mp = point2 - n1 + n2;
            Point3D p2pm = point2 + n1 - n2;
            Point3D p2mm = point2 - n1 - n2;

            // Sides.
            AddTriangle(mesh, p1pp, p1mp, p2mp);
            AddTriangle(mesh, p1pp, p2mp, p2pp);

            AddTriangle(mesh, p1pp, p2pp, p2pm);
            AddTriangle(mesh, p1pp, p2pm, p1pm);

            AddTriangle(mesh, p1pm, p2pm, p2mm);
            AddTriangle(mesh, p1pm, p2mm, p1mm);

            AddTriangle(mesh, p1mm, p2mm, p2mp);
            AddTriangle(mesh, p1mm, p2mp, p1mp);

            // Ends.
            AddTriangle(mesh, p1pp, p1pm, p1mm);
            AddTriangle(mesh, p1pp, p1mm, p1mp);

            AddTriangle(mesh, p2pp, p2mp, p2mm);
            AddTriangle(mesh, p2pp, p2mm, p2pm);
        }

        // Add a cage.
        private void AddCage(MeshGeometry3D mesh)
        {
            // Top.
            Vector3D up = new Vector3D(0, 1, 0);
            AddSegment(mesh, new Point3D(1, 1, 1), new Point3D(1, 1, -1), up);
            AddSegment(mesh, new Point3D(1, 1, -1), new Point3D(-1, 1, -1), up);
            AddSegment(mesh, new Point3D(-1, 1, -1), new Point3D(-1, 1, 1), up);
            AddSegment(mesh, new Point3D(-1, 1, 1), new Point3D(1, 1, 1), up);

            // Bottom.
            AddSegment(mesh, new Point3D(1, -1, 1), new Point3D(1, -1, -1), up);
            AddSegment(mesh, new Point3D(1, -1, -1), new Point3D(-1, -1, -1), up);
            AddSegment(mesh, new Point3D(-1, -1, -1), new Point3D(-1, -1, 1), up);
            AddSegment(mesh, new Point3D(-1, -1, 1), new Point3D(1, -1, 1), up);

            // Sides.
            Vector3D right = new Vector3D(1, 0, 0);
            AddSegment(mesh, new Point3D(1, -1, 1), new Point3D(1, 1, 1), right);
            AddSegment(mesh, new Point3D(1, -1, -1), new Point3D(1, 1, -1), right);
            AddSegment(mesh, new Point3D(-1, -1, 1), new Point3D(-1, 1, 1), right);
            AddSegment(mesh, new Point3D(-1, -1, -1), new Point3D(-1, 1, -1), right);
        }

        // Set the vector's length.
        private Vector3D ScaleVector(Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return new Vector3D(
                vector.X * scale,
                vector.Y * scale,
                vector.Z * scale);
        }

        // Position the camera.
        private void PositionCamera()
        {
            // Calculate the camera's position in Cartesian coordinates.
            double y = CameraR * Math.Sin(CameraPhi);
            double hyp = CameraR * Math.Cos(CameraPhi);
            double x = hyp * Math.Cos(CameraTheta);
            double z = hyp * Math.Sin(CameraTheta);
            TheCamera.Position = new Point3D(x, y, z);

            // Look toward the origin.
            TheCamera.LookDirection = new Vector3D(-x, -y, -z);

            // Set the Up direction.
            TheCamera.UpDirection = new Vector3D(0, 1, 0);
        }

        // Change the camera's phi value.
        private void scrPhi_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            CameraPhi = scrPhi.Value * Math.PI / 180.0;
            PositionCamera();
        }

        // Change the camera's theta value.
        private void scrTheta_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            CameraTheta = scrTheta.Value * Math.PI / 180.0;
            PositionCamera();
        }

        private void scrDistance_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            CameraR = scrDistance.Value;
            PositionCamera();
        }
    }
}
