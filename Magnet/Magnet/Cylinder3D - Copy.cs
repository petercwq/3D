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
            int circumferenceDivision = 8;
            
            int yDirectionDivision = 10;
            
            double maxTheta = DegToRad(360.0);
          
            double minYCoor = 0;
            double maxYCoor = Length;

            double dt = maxTheta / circumferenceDivision;
            double dy = (maxYCoor - minYCoor) / yDirectionDivision;

            MeshGeometry3D mesh = new MeshGeometry3D();

            for (int i =0; i <= yDirectionDivision; i++)
            {
                double y = minYCoor + i * dy;

                for (int j = 0; j <= circumferenceDivision; j++)
                {
                    double t = j * dt;

                    mesh.Positions.Add(GetPosition(t, y));
                }
            }

            for (int i = 0; i < yDirectionDivision; i++)
            {
                for (int j = 0; j < circumferenceDivision; j++)
                {
                    int x0 = j;
                    int x1 = (j + 1);
                    int y0 = i * (circumferenceDivision + 1);
                    int y1 = (i + 1) * (circumferenceDivision + 1);

                    mesh.TriangleIndices.Add(x0 + y0);
                    mesh.TriangleIndices.Add(x0 + y1);
                    mesh.TriangleIndices.Add(x1 + y0);

                    mesh.TriangleIndices.Add(x1 + y0);
                    mesh.TriangleIndices.Add(x0 + y1);
                    mesh.TriangleIndices.Add(x1 + y1);
                }
            }
        
            mesh.Freeze();
            return mesh;
        }
    }
}
