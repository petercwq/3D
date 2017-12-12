using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Magnet
{
    public sealed class Sphere3D : Primitive3D
    {

        private Point GetTextureCoordinate(double t, double y)
        {
            Matrix TYtoUV = new Matrix();
            //TYtoUV.Scale(80 / (2 * Math.PI), -0.5);

            Point p = new Point(t, y);
            p = p * TYtoUV;

            return p;
        }

        internal override Geometry3D Tessellate()
        {
            //double r = this.Radius;
            //double x,y,z;
            
            //MeshGeometry3D mesh = new MeshGeometry3D();
            
            //for (int k = 0; k < 180; k = k + 18)
            //{
            //    for (int i = 0; i < 360; i = i + 18)
            //    {
            //        x = r * Math.Cos((Math.PI / 180) * i) * Math.Cos((Math.PI / 180) * k) ;
            //        y = r * Math.Sin((Math.PI / 180) * k);
            //        z = r * Math.Sin((Math.PI / 180) * i) * Math.Cos((Math.PI / 180) * k) ;
                    
            //        mesh.Positions.Add(new Point3D(x,y,z));
                 
            //    }
            // }
            //double stepangle = (Math.PI/180)*18 ;//Here step angle is 1
            
            //double r2 = 360 / stepangle;
            //double p2 = 180 / stepangle;

            //for (double e = -r2; e < r2; e++)
            //{
            //    double cy = Math.Cos(e * stepangle);
            //    double cy1 = Math.Cos((e +1) * stepangle);

            //    double sy = Math.Sin(e * stepangle);
            //    double sy1 = Math.Sin((e + 1) * stepangle);


            //    for (double i = -p2; i < p2; i++)
            //    {
            //        double ci = Math.Cos((i) * stepangle);

            //        double si = Math.Sin(i * stepangle);
              

            //        mesh.TriangleIndices.Add((int) (r*ci*cy));

            //        mesh.TriangleIndices.Add((int)(r * sy));

            //        mesh.TriangleIndices.Add((int)(r * si * cy));

            //        mesh.TextureCoordinates.Add(new Point(stepangle * i, cy1));

            //        mesh.TriangleIndices.Add((int)(r * ci * cy1));

            //        mesh.TriangleIndices.Add((int)(r * sy1));

            //        mesh.TriangleIndices.Add((int)(r * si * cy1));

            //        mesh.TextureCoordinates.Add(new  Point(stepangle * i, cy1));
            //    }
            //}


            //mesh.Freeze();


            int n = 100;
            double r = this.Radius;
            int e;
            double segmentRad = Math.PI / 2 / (n + 1);
            int numberOfSeparators = 4 * n + 4;

            Point3DCollection points = new Point3DCollection();
            Int32Collection triangleIndices = new Int32Collection();
            MeshGeometry3D mesh = new MeshGeometry3D();

            for (e = -n; e <= n; e++)
            {
                double r_e = r * Math.Cos(segmentRad * e);
                double y_e = r * Math.Sin(segmentRad * e);

                for (int s = 0; s <= (numberOfSeparators - 1); s++)
                {
                    double z_s = r_e * Math.Sin(segmentRad * s) * (-1);
                    double x_s = r_e * Math.Cos(segmentRad * s);
                    points.Add(new Point3D(x_s, y_e, z_s));
                    mesh.Positions.Add(new Point3D(x_s, y_e, z_s));
                    //mesh.Normals.Add((Vector3D)(new Point3D(z_s ,y_e,x_s)));
                    mesh.TextureCoordinates.Add(GetTextureCoordinate(segmentRad * s, y_e));
                }
            }

            mesh.Positions.Add(new Point3D(0, r, 0));
            mesh.Positions.Add(new Point3D(0, -1 * r, 0));

            for (e = 0; e < 2 * n; e++)
            {
                for (int i = 0; i < numberOfSeparators; i++)
                {
                    mesh.TriangleIndices.Add(e * numberOfSeparators + i);
                    mesh.TriangleIndices.Add(e * numberOfSeparators + i +
                                        numberOfSeparators);
                    mesh.TriangleIndices.Add(e * numberOfSeparators + (i + 1) %
                                        numberOfSeparators + numberOfSeparators);

                    mesh.TriangleIndices.Add(e * numberOfSeparators + (i + 1) %
                                        numberOfSeparators + numberOfSeparators);
                    mesh.TriangleIndices.Add(e * numberOfSeparators +
                                       (i + 1) % numberOfSeparators);
                    mesh.TriangleIndices.Add(e * numberOfSeparators + i);
                }
            }

            for (int i = 0; i < numberOfSeparators; i++)
            {
                mesh.TriangleIndices.Add(e * numberOfSeparators + i);
                mesh.TriangleIndices.Add(e * numberOfSeparators + (i + 1) %
                                    numberOfSeparators);
                mesh.TriangleIndices.Add(numberOfSeparators * (2 * n + 1));
            }

            for (int i = 0; i < numberOfSeparators; i++)
            {
                mesh.TriangleIndices.Add(i);
                mesh.TriangleIndices.Add((i + 1) % numberOfSeparators);
                mesh.TriangleIndices.Add(numberOfSeparators * (2 * n + 1) + 1);
            }

            mesh.Freeze();
            return mesh;
        }
    }
}
