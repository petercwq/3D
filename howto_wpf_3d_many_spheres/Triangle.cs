using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media.Media3D;

namespace howto_wpf_3d_many_spheres
{
    class Triangle
    {
        public Point3D[] Points;
        public Triangle(params Point3D[] points)
        {
            Points = points;
        }

        // Subdivide this triangle and put the
        // new triangles in the list triangles.
        public void Subdivide(List<Triangle> triangles, Point3D center, double radius)
        {
            // Find the dividing points.
            Vector3D v01 = Points[1] - Points[0];
            Vector3D v02 = Points[2] - Points[0];
            Vector3D v12 = Points[2] - Points[1];
            Point3D A = Points[0] + v01 * 1.0 / 3.0;
            Point3D B = Points[0] + v02 * 1.0 / 3.0;
            Point3D C = Points[0] + v01 * 2.0 / 3.0;
            Point3D D = Points[0] + v01 * 2.0 / 3.0 + v12 * 1.0 / 3.0;
            Point3D E = Points[0] + v02 * 2.0 / 3.0;
            Point3D F = Points[1] + v12 * 1.0 / 3.0;
            Point3D G = Points[1] + v12 * 2.0 / 3.0;

            // Normalize the points.
            NormalizePoint(ref A, center, radius);
            NormalizePoint(ref B, center, radius);
            NormalizePoint(ref C, center, radius);
            NormalizePoint(ref D, center, radius);
            NormalizePoint(ref E, center, radius);
            NormalizePoint(ref F, center, radius);
            NormalizePoint(ref G, center, radius);

            // Make the triangles.
            triangles.Add(new Triangle(Points[0], A, B));
            triangles.Add(new Triangle(A, C, D));
            triangles.Add(new Triangle(A, D, B));
            triangles.Add(new Triangle(B, D, E));
            triangles.Add(new Triangle(C, Points[1], F));
            triangles.Add(new Triangle(C, F, D));
            triangles.Add(new Triangle(D, F, G));
            triangles.Add(new Triangle(D, G, E));
            triangles.Add(new Triangle(E, G, Points[2]));
        }

        // Make the point the indicated distance away from the center.
        private void NormalizePoint(ref Point3D point, Point3D center, double distance)
        {
            Vector3D vector = point - center;
            point = center + vector / vector.Length * distance;
        }

        // Make triangles to stellate this triangle.
        public void Stellate(List<Triangle> triangles, Point3D center, double radius)
        {
            // Find the point in the middle of the triangle.
            Point3D peak = new Point3D(
                (Points[0].X + Points[1].X + Points[2].X) / 3.0,
                (Points[0].Y + Points[1].Y + Points[2].Y) / 3.0,
                (Points[0].Z + Points[1].Z + Points[2].Z) / 3.0);

            // Give the peak its desired radius.
            NormalizePoint(ref peak, center, radius);

            // Make the new triangles.
            triangles.Add(new Triangle(Points[0], Points[1], peak));
            triangles.Add(new Triangle(Points[1], Points[2], peak));
            triangles.Add(new Triangle(Points[2], Points[0], peak));
        }
    }
}
