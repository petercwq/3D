using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Magnet
{
    public sealed class Cylinder3D : Primitive3D
    {
        public Cylinder3D()
        {
        }
       
        internal Point3D GetPosition(double t, double y)
        {
            double x = Radius * Math.Cos(t);
            double z = Radius * Math.Sin(t);
            return new Point3D(x, y, z);
        }
      
        internal override Geometry3D Tessellate()
        {
            int circumferenceDivision = 20;
            
            int lengthDivision = 20;
            
            double maxTheta = DegToRad(360.0);
        
            double minYCoor = 0;
            double maxYCoor = Length;

            double dt = maxTheta / circumferenceDivision;
            double dy = (maxYCoor - minYCoor) / lengthDivision;

            MeshGeometry3D mesh = new MeshGeometry3D();

            for (int i =0; i <= lengthDivision; i++)
            {
                double y = minYCoor + i * dy;

                for (int j = 0; j < circumferenceDivision; j++)
                {
                    double t = j * dt;

                    mesh.Positions.Add(GetPosition(t, y));
                }
            }

            for (int i = 0; i < lengthDivision; i++)
            {
                for (int j = 0; j < circumferenceDivision; j++)
                {
                    int x0 = j % circumferenceDivision + i * circumferenceDivision;//0
                    int x1 = (j + 1) % circumferenceDivision + i * circumferenceDivision;//1
                    int x2 = j + circumferenceDivision + i * circumferenceDivision;//4

                    int x3 = x1;//1
                    int x4 = x3 + circumferenceDivision;//5
                    int x5 = x2;//4
                   
                    mesh.TriangleIndices.Add(x0);
                    mesh.TriangleIndices.Add(x2);
                    mesh.TriangleIndices.Add(x1);

                    mesh.TriangleIndices.Add(x3);
                    mesh.TriangleIndices.Add(x5);
                    mesh.TriangleIndices.Add(x4);

                    //mesh.TriangleIndices.Add(x0);
                    //mesh.TriangleIndices.Add(x4);
                    //mesh.TriangleIndices.Add(x1);

                    //mesh.TriangleIndices.Add(x0);
                    //mesh.TriangleIndices.Add(x2);
                    //mesh.TriangleIndices.Add(x4);
                }
            }
        
            mesh.Freeze();
            return mesh;
        }
    }
}
