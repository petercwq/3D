using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace howto_3D_select_objects
{
    public static class MeshExtensions
    {
        // Return a MeshGeometry3D representing this mesh's triangle normals.
        public static MeshGeometry3D ToVertexNormals(this MeshGeometry3D mesh,
            double length, double thickness)
        {
            // Copy existing vertex normals.
            Vector3D[] vertex_normals = new Vector3D[mesh.Positions.Count];
            for (int i = 0; i < mesh.Normals.Count; i++)
                vertex_normals[i] = mesh.Normals[i];

            // Calculate missing vetex normals.
            for (int vertex = mesh.Normals.Count; vertex < mesh.Positions.Count; vertex++)
            {
                Vector3D total_vector = new Vector3D(0, 0, 0);
                int num_triangles = 0;

                // Find the triangles that contain this vertex.
                for (int triangle = 0; triangle < mesh.TriangleIndices.Count; triangle += 3)
                {
                    // See if this triangle contains the vertex.
                    int vertex1 = mesh.TriangleIndices[triangle];
                    int vertex2 = mesh.TriangleIndices[triangle + 1];
                    int vertex3 = mesh.TriangleIndices[triangle + 2];
                    if ((vertex1 == vertex) ||
                        (vertex2 == vertex) ||
                        (vertex3 == vertex))
                    {
                        // This triangle contains this vertex.
                        // Calculate its surface normal.
                        Vector3D normal = FindTriangleNormal(
                            mesh.Positions[vertex1],
                            mesh.Positions[vertex2],
                            mesh.Positions[vertex3]);

                        // Add the new normal to the total.
                        total_vector = new Vector3D(
                            total_vector.X + normal.X,
                            total_vector.Y + normal.Y,
                            total_vector.Z + normal.Z);
                        num_triangles++;
                    }
                }

                // Set the vertex's normal.
                if (num_triangles > 0)
                    vertex_normals[vertex] = new Vector3D(
                        total_vector.X / num_triangles,
                        total_vector.Y / num_triangles,
                        total_vector.Z / num_triangles);
            }

            // Make a mesh to hold the normals.
            MeshGeometry3D normals = new MeshGeometry3D();

            // Convert the normal vectors into segments.
            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                // Set the normal vector's length.
                vertex_normals[i] = vertex_normals[i].Scale(length);

                // Find the other end point.
                Point3D endpoint = mesh.Positions[i] + vertex_normals[i];

                // Create the segment.
                AddSegment(normals, mesh.Positions[i], endpoint, thickness);
            }

            return normals;
        }

        // Return a MeshGeometry3D representing this mesh's triangle normals.
        public static MeshGeometry3D ToTriangleNormals(this MeshGeometry3D mesh,
            double length, double thickness)
        {
            // Make a mesh to hold the normals.
            MeshGeometry3D normals = new MeshGeometry3D();

            // Loop through the mesh's triangles.
            for (int triangle = 0; triangle < mesh.TriangleIndices.Count; triangle += 3)
            {
                // Get the triangle's vertices.
                Point3D point1 = mesh.Positions[mesh.TriangleIndices[triangle]];
                Point3D point2 = mesh.Positions[mesh.TriangleIndices[triangle + 1]];
                Point3D point3 = mesh.Positions[mesh.TriangleIndices[triangle + 2]];

                // Make the triangle's normal
                AddTriangleNormal(mesh, normals,
                    point1, point2, point3, length, thickness);
            }

            return normals;
        }

        // Add a segment representing the triangle's normal to the normals mesh.
        private static void AddTriangleNormal(MeshGeometry3D mesh,
            MeshGeometry3D normals, Point3D point1, Point3D point2, Point3D point3,
            double length, double thickness)
        {
            // Get the triangle's normal.
            Vector3D n = FindTriangleNormal(point1, point2, point3);

            // Set the length.
            n = n.Scale(length);

            // Find the center of the triangle.
            Point3D endpoint1 = new Point3D(
                (point1.X + point2.X + point3.X) / 3.0,
                (point1.Y + point2.Y + point3.Y) / 3.0,
                (point1.Z + point2.Z + point3.Z) / 3.0);

            // Find the segment's other end point.
            Point3D endpoint2 = endpoint1 + n;

            // Create the segment.
            AddSegment(normals, endpoint1, endpoint2, thickness);
        }

        // Calculate a triangle's normal vector.
        public static Vector3D FindTriangleNormal(Point3D point1, Point3D point2, Point3D point3)
        {
            // Get two edge vectors.
            Vector3D v1 = point2 - point1;
            Vector3D v2 = point3 - point2;

            // Get the cross product.
            Vector3D n = Vector3D.CrossProduct(v1, v2);

            // Normalize.
            n.Normalize();

            return n;
        }

        // Return a MeshGeometry3D representing this mesh's wireframe.
        public static MeshGeometry3D ToWireframe(this MeshGeometry3D mesh, double thickness)
        {
            // Make a dictionary in case triangles share segments
            // so we don't draw the same segment twice.
            Dictionary<int, int> already_drawn = new Dictionary<int, int>();

            // Make a mesh to hold the wireframe.
            MeshGeometry3D wireframe = new MeshGeometry3D();

            // Loop through the mesh's triangles.
            for (int triangle = 0; triangle < mesh.TriangleIndices.Count; triangle += 3)
            {
                // Get the triangle's corner indices.
                int index1 = mesh.TriangleIndices[triangle];
                int index2 = mesh.TriangleIndices[triangle + 1];
                int index3 = mesh.TriangleIndices[triangle + 2];

                // Make the triangle's three segments.
                AddTriangleSegment(mesh, wireframe, already_drawn, index1, index2, thickness);
                AddTriangleSegment(mesh, wireframe, already_drawn, index2, index3, thickness);
                AddTriangleSegment(mesh, wireframe, already_drawn, index3, index1, thickness);
            }

            return wireframe;
        }

        // Add the triangle's three segments to the wireframe mesh.
        private static void AddTriangleSegment(MeshGeometry3D mesh,
            MeshGeometry3D wireframe, Dictionary<int, int> already_drawn,
            int index1, int index2, double thickness)
        {
            // Get a unique ID for a segment connecting the two points.
            if (index1 > index2)
            {
                int temp = index1;
                index1 = index2;
                index2 = temp;
            }
            int segment_id = index1 * mesh.Positions.Count + index2;

            // If we've already added this segment for
            // another triangle, do nothing.
            if (already_drawn.ContainsKey(segment_id)) return;
            already_drawn.Add(segment_id, segment_id);

            // Create the segment.
            AddSegment(wireframe, mesh.Positions[index1], mesh.Positions[index2], thickness);
        }

        // Add a triangle to the indicated mesh.
        // Do not reuse points so triangles don't share normals.
        private static void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
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

        // Make a thin rectangular prism between the two points.
        // If extend is true, extend the segment by half the
        // thickness so segments with the same end points meet nicely.
        // If up is missing, create a perpendicular vector to use.
        public static void AddSegment(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, double thickness, bool extend)
        {
            // Find an up vector that is not colinear with the segment.
            // Start with a vector parallel to the Y axis.
            Vector3D up = new Vector3D(0, 1, 0);

            // If the segment and up vector point in more or less the
            // same direction, use an up vector parallel to the X axis.
            Vector3D segment = point2 - point1;
            segment.Normalize();
            if (Math.Abs(Vector3D.DotProduct(up, segment)) > 0.9)
                up = new Vector3D(1, 0, 0);

            // Add the segment.
            AddSegment(mesh, point1, point2, up, thickness, extend);
        }

        public static void AddSegment(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, double thickness)
        {
            AddSegment(mesh, point1, point2, thickness, false);
        }
        public static void AddSegment(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Vector3D up, double thickness)
        {
            AddSegment(mesh, point1, point2, up, thickness, false);
        }
        public static void AddSegment(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Vector3D up, double thickness,
            bool extend)
        {
            // Get the segment's vector.
            Vector3D v = point2 - point1;

            if (extend)
            {
                // Increase the segment's length on both ends by thickness / 2.
                Vector3D n = v.Scale(thickness / 2.0);
                point1 -= n;
                point2 += n;
            }

            // Get the scaled up vector.
            Vector3D n1 = up.Scale(thickness / 2.0);

            // Get another scaled perpendicular vector.
            Vector3D n2 = Vector3D.CrossProduct(v, n1);
            n2 = n2.Scale(thickness / 2.0);

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

        // Set the vector's length.
        public static Vector3D Scale(this Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return new Vector3D(
                vector.X * scale,
                vector.Y * scale,
                vector.Z * scale);
        }

        // Make an arrow.
        public static void AddArrow(this MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Vector3D up,
            double barb_length)
        {
            // Make the shaft.
            AddSegment(mesh, point1, point2, 0.05, true);

            // Get a unit vector in the direction of the segment.
            Vector3D v = point2 - point1;
            v.Normalize();

            // Get a perpendicular unit vector in the plane of the arrowhead.
            Vector3D perp = Vector3D.CrossProduct(v, up);
            perp.Normalize();

            // Calculate the arrowhead end points.
            Vector3D v1 = (-v + perp).Scale(barb_length);
            Vector3D v2 = (-v - perp).Scale(barb_length);

            // Draw the arrowhead.
            AddSegment(mesh, point2, point2 + v1, up, 0.05);
            AddSegment(mesh, point2, point2 + v2, up, 0.05);
        }

        // Make an axis with tic marks.
        public static void AddAxis(this MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Vector3D up,
            double tic_diameter, double tic_separation)
        {
            // Make the shaft.
            AddSegment(mesh, point1, point2, 0.05, true);

            // Get a unit vector in the direction of the segment.
            Vector3D v = point2 - point1;
            double length = v.Length;
            v.Normalize();

            // Find the position of the first tic mark.
            Point3D tic_point1 = point1 + v * (tic_separation - 0.025);

            // Make tic marks.
            int num_tics = (int)(length / tic_separation) - 1;
            for (int i = 0; i < num_tics; i++)
            {
                Point3D tic_point2 = tic_point1 + v * 0.05;
                AddSegment(mesh, tic_point1, tic_point2, tic_diameter);
                tic_point1 += v * tic_separation;
            }
        }

        // Make a box with the indicated dimensions and
        // corner with minimal X, Y, and Z coordinates.
        public static void AddBox(this MeshGeometry3D mesh,
            float x, float y, float z, float dx, float dy, float dz)
        {
            // Bottom (min Y).
            AddTriangle(mesh,
                new Point3D(x, y, z),
                new Point3D(x + dx, y, z),
                new Point3D(x + dx, y, z + dz));
            AddTriangle(mesh,
                new Point3D(x, y, z),
                new Point3D(x + dx, y, z + dz),
                new Point3D(x, y, z + dz));

            // Top (max Y).
            AddTriangle(mesh,
                new Point3D(x, y + dy, z),
                new Point3D(x, y + dy, z + dz),
                new Point3D(x + dx, y + dy, z + dz));
            AddTriangle(mesh,
                new Point3D(x, y + dy, z),
                new Point3D(x + dx, y + dy, z + dz),
                new Point3D(x + dx, y + dy, z));

            // Left (min X).
            AddTriangle(mesh,
                new Point3D(x, y, z),
                new Point3D(x, y, z + dz),
                new Point3D(x, y + dy, z + dz));
            AddTriangle(mesh,
                new Point3D(x, y, z),
                new Point3D(x, y + dy, z + dz),
                new Point3D(x, y + dy, z));

            // Right (max X).
            AddTriangle(mesh,
                new Point3D(x + dx, y, z),
                new Point3D(x + dx, y + dy, z),
                new Point3D(x + dx, y + dy, z + dz));
            AddTriangle(mesh,
                new Point3D(x + dx, y, z),
                new Point3D(x + dx, y + dy, z + dz),
                new Point3D(x + dx, y, z + dz));

            // Front (max Z).
            AddTriangle(mesh,
                new Point3D(x, y, z + dz),
                new Point3D(x + dx, y, z + dz),
                new Point3D(x + dx, y + dy, z + dz));
            AddTriangle(mesh,
                new Point3D(x, y, z + dz),
                new Point3D(x + dx, y + dy, z + dz),
                new Point3D(x, y + dy, z + dz));

            // Back (min Z).
            AddTriangle(mesh,
                new Point3D(x, y, z),
                new Point3D(x, y + dy, z),
                new Point3D(x + dx, y + dy, z));
            AddTriangle(mesh,
                new Point3D(x, y, z),
                new Point3D(x + dx, y + dy, z),
                new Point3D(x + dx, y, z));
        }

        // Add axes with the given lengths.
        public static MeshGeometry3D XAxisArrow(float length)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.AddArrow(new Point3D(0, 0, 0), new Point3D(length, 0, 0),
                new Vector3D(0, 1, 0), 0.5);
            return mesh;
        }
        public static MeshGeometry3D YAxisArrow(float length)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.AddArrow(new Point3D(0, 0, 0), new Point3D(0, length, 0),
                new Vector3D(1, 0, 0), 0.5);
            return mesh;
        }
        public static MeshGeometry3D ZAxisArrow(float length)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.AddArrow(new Point3D(0, 0, 0), new Point3D(0, 0, length),
                new Vector3D(0, 1, 0), 0.5);
            return mesh;
        }

        // Give the mesh a diffuse material.
        public static GeometryModel3D SetMaterial(this MeshGeometry3D mesh, Brush brush, bool set_back_material)
        {
            DiffuseMaterial material = new DiffuseMaterial(brush);
            GeometryModel3D model = new GeometryModel3D(mesh, material);
            if (set_back_material) model.BackMaterial = material;
            return model;
        }

        // Add a cage.
        public static void AddCage(this MeshGeometry3D mesh,
            float x, float y, float z, float dx, float dy, float dz, double thickness)
        {
            float xmin = x;
            float xmax = x + dx;
            float ymin = y;
            float ymax = y + dy;
            float zmin = z;
            float zmax = z + dz;

            // Top.
            Vector3D up = new Vector3D(0, 1, 0);
            AddSegment(mesh, new Point3D(xmax, ymax, zmax), new Point3D(xmax, ymax, zmin), up, thickness, true);
            AddSegment(mesh, new Point3D(xmax, ymax, zmin), new Point3D(xmin, ymax, zmin), up, thickness, true);
            AddSegment(mesh, new Point3D(xmin, ymax, zmin), new Point3D(xmin, ymax, zmax), up, thickness, true);
            AddSegment(mesh, new Point3D(xmin, ymax, zmax), new Point3D(xmax, ymax, zmax), up, thickness, true);

            // Bottom.
            AddSegment(mesh, new Point3D(xmax, ymin, zmax), new Point3D(xmax, ymin, zmin), up, thickness, true);
            AddSegment(mesh, new Point3D(xmax, ymin, zmin), new Point3D(xmin, ymin, zmin), up, thickness, true);
            AddSegment(mesh, new Point3D(xmin, ymin, zmin), new Point3D(xmin, ymin, zmax), up, thickness, true);
            AddSegment(mesh, new Point3D(xmin, ymin, zmax), new Point3D(xmax, ymin, zmax), up, thickness, true);

            // Sides.
            Vector3D right = new Vector3D(1, 0, 0);
            AddSegment(mesh, new Point3D(xmax, ymin, zmax), new Point3D(xmax, ymax, zmax), right, thickness, true);
            AddSegment(mesh, new Point3D(xmax, ymin, zmin), new Point3D(xmax, ymax, zmin), right, thickness, true);
            AddSegment(mesh, new Point3D(xmin, ymin, zmax), new Point3D(xmin, ymax, zmax), right, thickness, true);
            AddSegment(mesh, new Point3D(xmin, ymin, zmin), new Point3D(xmin, ymax, zmin), right, thickness, true);
        }

        // Add a cylinder with smooth sides.
        public static void AddSmoothCylinder(this MeshGeometry3D mesh,
            Point3D end_point, Vector3D axis, double radius, int num_sides)
        {
            // Get two vectors perpendicular to the axis.
            Vector3D v1;
            if ((axis.Z < -0.01) || (axis.Z > 0.01))
                v1 = new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y);
            else
                v1 = new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);
            Vector3D v2 = Vector3D.CrossProduct(v1, axis);

            // Make the vectors have length radius.
            v1 *= (radius / v1.Length);
            v2 *= (radius / v2.Length);

            // Make the top end cap.
            // Make the end point.
            int pt0 = mesh.Positions.Count; // Index of end_point.
            mesh.Positions.Add(end_point);

            // Make the top points.
            double theta = 0;
            double dtheta = 2 * Math.PI / num_sides;
            for (int i = 0; i < num_sides; i++)
            {
                mesh.Positions.Add(end_point +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2);
                theta += dtheta;
            }

            // Make the top triangles.
            int pt1 = mesh.Positions.Count - 1; // Index of last point.
            int pt2 = pt0 + 1;                  // Index of first point.
            for (int i = 0; i < num_sides; i++)
            {
                mesh.TriangleIndices.Add(pt0);
                mesh.TriangleIndices.Add(pt1);
                mesh.TriangleIndices.Add(pt2);
                pt1 = pt2++;
            }

            // Make the bottom end cap.
            // Make the end point.
            pt0 = mesh.Positions.Count; // Index of end_point2.
            Point3D end_point2 = end_point + axis;
            mesh.Positions.Add(end_point2);

            // Make the bottom points.
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                mesh.Positions.Add(end_point2 +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2);
                theta += dtheta;
            }

            // Make the bottom triangles.
            theta = 0;
            pt1 = mesh.Positions.Count - 1; // Index of last point.
            pt2 = pt0 + 1;                  // Index of first point.
            for (int i = 0; i < num_sides; i++)
            {
                mesh.TriangleIndices.Add(num_sides + 1);    // end_point2
                mesh.TriangleIndices.Add(pt2);
                mesh.TriangleIndices.Add(pt1);
                pt1 = pt2++;
            }

            // Make the sides.
            // Add the points to the mesh.
            int first_side_point = mesh.Positions.Count;
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2;
                mesh.Positions.Add(p1);
                Point3D p2 = p1 + axis;
                mesh.Positions.Add(p2);
                theta += dtheta;
            }

            // Make the side triangles.
            pt1 = mesh.Positions.Count - 2;
            pt2 = pt1 + 1;
            int pt3 = first_side_point;
            int pt4 = pt3 + 1;
            for (int i = 0; i < num_sides; i++)
            {
                mesh.TriangleIndices.Add(pt1);
                mesh.TriangleIndices.Add(pt2);
                mesh.TriangleIndices.Add(pt4);

                mesh.TriangleIndices.Add(pt1);
                mesh.TriangleIndices.Add(pt4);
                mesh.TriangleIndices.Add(pt3);

                pt1 = pt3;
                pt3 += 2;
                pt2 = pt4;
                pt4 += 2;
            }
        }

        // Add a triangle to the indicated mesh.
        // Reuse points so triangles share normals.
        public static void AddSmoothTriangle(this MeshGeometry3D mesh,
            Dictionary<Point3D, int> dict,
            Point3D point1, Point3D point2, Point3D point3)
        {
            int index1, index2, index3;

            // Find or create the points.
            if (dict.ContainsKey(point1)) index1 = dict[point1];
            else
            {
                index1 = mesh.Positions.Count;
                mesh.Positions.Add(point1);
                dict.Add(point1, index1);
            }

            if (dict.ContainsKey(point2)) index2 = dict[point2];
            else
            {
                index2 = mesh.Positions.Count;
                mesh.Positions.Add(point2);
                dict.Add(point2, index2);
            }

            if (dict.ContainsKey(point3)) index3 = dict[point3];
            else
            {
                index3 = mesh.Positions.Count;
                mesh.Positions.Add(point3);
                dict.Add(point3, index3);
            }

            // If two or more of the points are
            // the same, it's not a triangle.
            if ((index1 == index2) ||
                (index2 == index3) ||
                (index3 == index1)) return;

            // Create the triangle.
            mesh.TriangleIndices.Add(index1);
            mesh.TriangleIndices.Add(index2);
            mesh.TriangleIndices.Add(index3);
        }

        // Add a sphere.
        public static void AddSmoothSphere(this MeshGeometry3D mesh,
            Point3D center, double radius, int num_phi, int num_theta)
        {
            // Make a dictionary to track the sphere's points.
            Dictionary<Point3D, int> dict = new Dictionary<Point3D, int>();

            double phi0, theta0;
            double dphi = Math.PI / num_phi;
            double dtheta = 2 * Math.PI / num_theta;

            phi0 = 0;
            double y0 = radius * Math.Cos(phi0);
            double r0 = radius * Math.Sin(phi0);
            for (int i = 0; i < num_phi; i++)
            {
                double phi1 = phi0 + dphi;
                double y1 = radius * Math.Cos(phi1);
                double r1 = radius * Math.Sin(phi1);

                // Point ptAB has phi value A and theta value B.
                // For example, pt01 has phi = phi0 and theta = theta1.
                // Find the points with theta = theta0.
                theta0 = 0;
                Point3D pt00 = new Point3D(
                    center.X + r0 * Math.Cos(theta0),
                    center.Y + y0,
                    center.Z + r0 * Math.Sin(theta0));
                Point3D pt10 = new Point3D(
                    center.X + r1 * Math.Cos(theta0),
                    center.Y + y1,
                    center.Z + r1 * Math.Sin(theta0));
                for (int j = 0; j < num_theta; j++)
                {
                    // Find the points with theta = theta1.
                    double theta1 = theta0 + dtheta;
                    Point3D pt01 = new Point3D(
                        center.X + r0 * Math.Cos(theta1),
                        center.Y + y0,
                        center.Z + r0 * Math.Sin(theta1));
                    Point3D pt11 = new Point3D(
                        center.X + r1 * Math.Cos(theta1),
                        center.Y + y1,
                        center.Z + r1 * Math.Sin(theta1));

                    // Create the triangles.
                    AddSmoothTriangle(mesh, dict, pt00, pt11, pt10);
                    AddSmoothTriangle(mesh, dict, pt00, pt01, pt11);

                    // Move to the next value of theta.
                    theta0 = theta1;
                    pt00 = pt01;
                    pt10 = pt11;
                }

                // Move to the next value of phi.
                phi0 = phi1;
                y0 = y1;
                r0 = r1;
            }
        }

        // Add a cone.
        public static void AddCone(this MeshGeometry3D mesh, Point3D end_point,
            Vector3D axis, double radius1, double radius2, int num_sides)
        {
            // Get two vectors perpendicular to the axis.
            Vector3D top_v1;
            if ((axis.Z < -0.01) || (axis.Z > 0.01))
                top_v1 = new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y);
            else
                top_v1 = new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);
            Vector3D top_v2 = Vector3D.CrossProduct(top_v1, axis);

            Vector3D bot_v1 = top_v1;
            Vector3D bot_v2 = top_v2;

            // Make the vectors have length radius.
            top_v1 *= (radius1 / top_v1.Length);
            top_v2 *= (radius1 / top_v2.Length);

            bot_v1 *= (radius2 / bot_v1.Length);
            bot_v2 *= (radius2 / bot_v2.Length);

            // Make the top end cap.
            double theta = 0;
            double dtheta = 2 * Math.PI / num_sides;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2;
                theta += dtheta;
                Point3D p2 = end_point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2;
                AddTriangle(mesh, end_point, p1, p2);
            }

            // Make the bottom end cap.
            Point3D end_point2 = end_point + axis;
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point2 +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2;
                theta += dtheta;
                Point3D p2 = end_point2 +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2;
                AddTriangle(mesh, end_point2, p2, p1);
            }

            // Make the sides.
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2;
                Point3D p3 = end_point + axis +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2;
                theta += dtheta;
                Point3D p2 = end_point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2;
                Point3D p4 = end_point + axis +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2;

                AddTriangle(mesh, p1, p3, p2);
                AddTriangle(mesh, p2, p3, p4);
            }
        }

        /// <summary>
        /// Creates a ModelVisual3D containing a text label.
        /// </summary>
        /// <param name="text">The string</param>
        /// <param name="textColor">The color of the text.</param>
        /// <param name="bDoubleSided">Visible from both sides?</param>
        /// <param name="height">Height of the characters</param>
        /// <param name="center">The center of the label</param>
        /// <param name="horizontalDir">Horizontal direction of the label</param>
        /// <param name="verticalDir">Vertical direction of the label</param>
        /// <returns>Suitable for adding to your Viewport3D</returns>
        public static GeometryModel3D To3DLabel(this string text,
            Point3D center,
            Vector3D horizontalDir,
            Vector3D verticalDir,
            Brush textColor,
            double height,
            string font = "Arial",
            bool bDoubleSided = true)
        {
            // First we need a textblock containing the text of our label
            TextBlock tb = new TextBlock(new Run(text));
            tb.Foreground = textColor;
            tb.FontFamily = new FontFamily(font);

            // Now use that TextBlock as the brush for a material
            DiffuseMaterial mat = new DiffuseMaterial();
            mat.Brush = new VisualBrush(tb);

            // We just assume the characters are square
            double width = text.Length * height;

            // Since the parameter coming in was the center of the label,
            // we need to find the four corners
            // p0 is the lower left corner
            // p1 is the upper left
            // p2 is the lower right
            // p3 is the upper right
            Point3D p0 = center - width / 2 * horizontalDir - height / 2 * verticalDir;
            Point3D p1 = p0 + verticalDir * 1 * height;
            Point3D p2 = p0 + horizontalDir * width;
            Point3D p3 = p0 + verticalDir * 1 * height + horizontalDir * width;

            // Now build the geometry for the sign.  It's just a
            // rectangle made of two triangles, on each side.

            MeshGeometry3D mg = new MeshGeometry3D();
            mg.Positions = new Point3DCollection();
            mg.Positions.Add(p0);    // 0
            mg.Positions.Add(p1);    // 1
            mg.Positions.Add(p2);    // 2
            mg.Positions.Add(p3);    // 3

            if (bDoubleSided)
            {
                mg.Positions.Add(p0);    // 4
                mg.Positions.Add(p1);    // 5
                mg.Positions.Add(p2);    // 6
                mg.Positions.Add(p3);    // 7
            }

            mg.TriangleIndices.Add(0);
            mg.TriangleIndices.Add(3);
            mg.TriangleIndices.Add(1);
            mg.TriangleIndices.Add(0);
            mg.TriangleIndices.Add(2);
            mg.TriangleIndices.Add(3);

            if (bDoubleSided)
            {
                mg.TriangleIndices.Add(4);
                mg.TriangleIndices.Add(5);
                mg.TriangleIndices.Add(7);
                mg.TriangleIndices.Add(4);
                mg.TriangleIndices.Add(7);
                mg.TriangleIndices.Add(6);
            }

            // These texture coordinates basically stretch the
            // TextBox brush to cover the full side of the label.

            mg.TextureCoordinates.Add(new Point(0, 1));
            mg.TextureCoordinates.Add(new Point(0, 0));
            mg.TextureCoordinates.Add(new Point(1, 1));
            mg.TextureCoordinates.Add(new Point(1, 0));

            if (bDoubleSided)
            {
                mg.TextureCoordinates.Add(new Point(1, 1));
                mg.TextureCoordinates.Add(new Point(1, 0));
                mg.TextureCoordinates.Add(new Point(0, 1));
                mg.TextureCoordinates.Add(new Point(0, 0));
            }

            // And that's all.  Return the result.
            return new GeometryModel3D(mg, mat); ;
        }
    }
}
