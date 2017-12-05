using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Diagnostics;

namespace magic_cube {
    public static class Helpers {
        /// <summary>
        /// Create a triangle which can be used for more complex models
        /// </summary>
        /// <param name="p0">The first position of the mesh</param>
        /// <param name="p1">The second position of the mesh</param>
        /// <param name="p2">The third position of the mesh</param>
        /// <param name="m">A <see cref="Material"/> to be applied to the triangle</param>
        /// <returns><see cref="GeometryModel3D"/></returns>
        public static GeometryModel3D createTriangleModel(Point3D p0, Point3D p1, Point3D p2, Material m) {
            MeshGeometry3D triangleMesh = new MeshGeometry3D();
            triangleMesh.Positions.Add(p0);
            triangleMesh.Positions.Add(p1);
            triangleMesh.Positions.Add(p2);

            triangleMesh.TriangleIndices.Add(0);
            triangleMesh.TriangleIndices.Add(1);
            triangleMesh.TriangleIndices.Add(2);

            Vector3D normal = calculateNormal(p0, p1, p2);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);

            return new GeometryModel3D(triangleMesh, m);
        }

        public static MyModelVisual3D createTriangleModel(Point3D p0, Point3D p1, Point3D p2, Material m, string t) {
            MyModelVisual3D retval = new MyModelVisual3D();
            retval.Content = createTriangleModel(p0, p1, p2, m);
            retval.Tag = t;
            return retval;
        }
        
        public static Model3DGroup createRectangleModel(Point3D[] p, Material m, bool up=true) {
            if (p.Length != 4) {
                return null;
            }

            Model3DGroup rect = new Model3DGroup();

            if (up) {
                rect.Children.Add(createTriangleModel(p[0], p[1], p[2], m));
                rect.Children.Add(createTriangleModel(p[0], p[2], p[3], m));
            }
            else {
                rect.Children.Add(createTriangleModel(p[0], p[2], p[1], m));
                rect.Children.Add(createTriangleModel(p[0], p[3], p[2], m));
            }

            return rect;
        }


        public static MyModelVisual3D createRectangleModel(Point3D[] p, Material m, string tag, bool up=true) {
            MyModelVisual3D rect = new MyModelVisual3D();

            rect.Children.Add(createRectangleModel(p, m, tag, up));

            return rect;
        }

        /// <summary>
        /// Calculate the normal of a plane
        /// </summary>
        /// <param name="p0">The first point of the plane</param>
        /// <param name="p1">The second point of the plane</param>
        /// <param name="p2">The third point of the plane</param>
        /// <returns><see cref="Vector3D"/> representing the plane's normal</returns>
        private static Vector3D calculateNormal(Point3D p0, Point3D p1, Point3D p2) {
            Vector3D v1 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D v2 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);

            return Vector3D.CrossProduct(v1, v2);
        }



        public static MyModelVisual3D createTouchFaces(double len, int size, Transform3D rotations, Material touchFaceMaterial) {
            double offset = len / size;
            double middle = len / 2;
            double small_num = Math.Pow(10, -5);

            MyModelVisual3D touchFaces = new MyModelVisual3D();
            MyModelVisual3D touchFace;

            touchFaces.Transform = rotations;

            for (int y = 0; y < size; y++) {
                for (int z = 0; z < size; z++) {
                    for (int x = 0; x < size; x++) {
                        if (y == size - 1) { //up
                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                                
                                new Point3D(-middle + x*offset,       middle+small_num, -middle),
                                new Point3D(-middle + x*offset,       middle+small_num, middle),
                                new Point3D(-middle + (x+1) * offset, middle+small_num, middle),       
                                new Point3D(-middle + (x+1) * offset, middle+small_num, -middle),
                                }, touchFaceMaterial);

                            touchFace.Tag = "UV" + x;

                            touchFaces.Children.Add(touchFace);

                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                                
                                new Point3D(-middle, middle+small_num, -middle + z*offset),
                                new Point3D(-middle, middle+small_num, -middle + (z+1)*offset),
                                new Point3D(middle,  middle+small_num, -middle + (z+1)*offset),       
                                new Point3D(middle,  middle+small_num, -middle + z*offset),
                                }, touchFaceMaterial);

                            touchFace.Tag = "UH" + z;

                            touchFaces.Children.Add(touchFace);
                        }

                        if (y == 0) { //down
                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                                
                                new Point3D(-middle + x*offset,       -middle-small_num, -middle),
                                new Point3D(-middle + x*offset,       -middle-small_num, middle),
                                new Point3D(-middle + (x+1) * offset, -middle-small_num, middle),       
                                new Point3D(-middle + (x+1) * offset, -middle-small_num, -middle),
                                }, touchFaceMaterial, false);

                            touchFace.Tag = "DV" + x;

                            touchFaces.Children.Add(touchFace);

                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                                
                                new Point3D(-middle, -middle-small_num, -middle + z*offset),
                                new Point3D(-middle, -middle-small_num, -middle + (z+1)*offset),
                                new Point3D(middle,  -middle-small_num, -middle + (z+1)*offset),       
                                new Point3D(middle,  -middle-small_num, -middle + z*offset),
                                }, touchFaceMaterial, false);

                            touchFace.Tag = "DH" + z;

                            touchFaces.Children.Add(touchFace);
                        }

                        if (z == size - 1) { //front                            
                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                                
                                new Point3D(-middle + x*offset,     middle,  middle+small_num),
                                new Point3D(-middle + x*offset,     -middle, middle+small_num),
                                new Point3D(-middle + (x+1)*offset, -middle, middle+small_num),       
                                new Point3D(-middle + (x+1)*offset, middle,  middle+small_num),
                                }, touchFaceMaterial);

                            touchFace.Tag = "FV" + x;

                            touchFaces.Children.Add(touchFace);

                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                  
                                new Point3D(-middle, middle - y*offset,     middle+small_num),
                                new Point3D(-middle, middle - (y+1)*offset, middle+small_num),
                                new Point3D(middle,  middle - (y+1)*offset, middle+small_num),       
                                new Point3D(middle,  middle - y*offset,     middle+small_num),
                                }, touchFaceMaterial);

                            touchFace.Tag = "FH" + y;

                            touchFaces.Children.Add(touchFace);
                        }

                        if (z == 0) { //back
                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                                
                                new Point3D(-middle + x*offset,       middle,  -middle-small_num),
                                new Point3D(-middle + x*offset,       -middle, -middle-small_num),
                                new Point3D(-middle + (x+1) * offset, -middle, -middle-small_num),       
                                new Point3D(-middle + (x+1) * offset, middle,  -middle-small_num),
                                }, touchFaceMaterial, false);

                            touchFace.Tag = "BV" + x;

                            touchFaces.Children.Add(touchFace);

                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                  
                                new Point3D(-middle, middle - y*offset,     -middle-small_num),
                                new Point3D(-middle, middle - (y+1)*offset, -middle-small_num),
                                new Point3D(middle,  middle - (y+1)*offset, -middle-small_num),       
                                new Point3D(middle,  middle - y*offset,     -middle-small_num),
                                }, touchFaceMaterial, false);

                            touchFace.Tag = "BH" + y;

                            touchFaces.Children.Add(touchFace);
                        }

                        if (x == size - 1) { //right          
                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                                
                                new Point3D(middle+small_num, middle, -middle + (z+1)*offset),
                                new Point3D(middle+small_num, -middle, -middle + (z+1)*offset),
                                new Point3D(middle+small_num, -middle, -middle + z*offset),       
                                new Point3D(middle+small_num, middle, -middle + z*offset),
                                }, touchFaceMaterial);

                            touchFace.Tag = "RV" + z;

                            touchFaces.Children.Add(touchFace);

                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                           
                                new Point3D(middle+small_num, -middle + (y+1)*offset, middle),
                                new Point3D(middle+small_num, -middle + y*offset,     middle),
                                new Point3D(middle+small_num, -middle + y*offset,     -middle),       
                                new Point3D(middle+small_num, -middle + (y+1)*offset, -middle),
                                }, touchFaceMaterial);

                            touchFace.Tag = "RH" + y;

                            touchFaces.Children.Add(touchFace);
                        }

                        if (x == 0) { //left
                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                                
                                new Point3D(-middle-small_num, middle,  -middle + (z+1)*offset),
                                new Point3D(-middle-small_num, -middle, -middle + (z+1)*offset),
                                new Point3D(-middle-small_num, -middle, -middle + z*offset),       
                                new Point3D(-middle-small_num, middle,  -middle + z*offset),
                                }, touchFaceMaterial, false);

                            touchFace.Tag = "LV" + z;

                            touchFaces.Children.Add(touchFace);

                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                           
                                new Point3D(-middle-small_num, -middle + (y+1)*offset, middle),
                                new Point3D(-middle-small_num, -middle + y*offset,     middle),
                                new Point3D(-middle-small_num, -middle + y*offset,     -middle),       
                                new Point3D(-middle-small_num, -middle + (y+1)*offset, -middle),
                                }, touchFaceMaterial, false);

                            touchFace.Tag = "LH" + y;

                            touchFaces.Children.Add(touchFace);
                        }
                    }
                }
            }

            return touchFaces;
        }
    }
}