### WPF 3D

## Articles
- [三维图形概述](https://msdn.microsoft.com/zh-cn/library/ms747437%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396)
- [三维变换概述](https://msdn.microsoft.com/zh-cn/library/ms753347(v=vs.110).aspx)
- [最大程度地提高 WPF 三维性能](https://msdn.microsoft.com/zh-cn/library/bb613553(v=vs.110).aspx)
- [Rendering Transparent 3D Surfaces in WPF with C#](http://xoax.net/blog/rendering-transparent-3d-surfaces-in-wpf-with-c/)
- [Getting started with 3D in WPF](http://www.barth-dev.de/getting-started-3d-wpf/)
- [Make a stellate geodesic sphere with WPF and C#](http://csharphelper.com/blog/2015/12/make-a-stellate-geodesic-sphere-with-wpf-and-c/)
- [Let the user select and deselect 3D objects using WPF and C#](http://csharphelper.com/blog/2014/10/let-the-user-select-and-deselect-3d-objects-using-wpf-and-c/)
- [Magnet: A mind teaser in 3D](https://www.codeproject.com/Articles/679993/Magnet-A-mind-teaser-in-D)
- [Wpf Cube Three Dee](http://www.pererikstrandberg.se/blog/index.cgi?page=WpfCubeThreeDee)
- [WPF 3D Article, Tutorial with Chart Graphics C# Code](http://www.ucancode.net/WPF-3D-Article-Tutorial-with-Chart-Graphics-CSharp-Code.htm)
- [WPF: Rubik's Cube](https://www.codeproject.com/Articles/322872/WPF-Rubiks-Cube)
- 

Axis-angle rotations work well for static transformations and some animations. However, consider rotating a cube model 60 degrees around the X axis, then 45 degrees around the Z axis. You can describe this transformation as two discrete affine transformations, or as a matrix. However, it might be difficult to smoothly animate a rotation defined this way. Although the beginning and ending positions of the model computed by either approach are the same, the intermediate positions taken by the model are computationally uncertain. Quaternions represent an alternative way to compute the interpolation between the start and end of a rotation.

A quaternion represents an axis in 3-D space and a rotation around that axis. For example, a quaternion might represent a (1,1,2) axis and a rotation of 50 degrees. Quaternions’ power in defining rotations comes from the two operations that you can perform on them: composition and interpolation. The composition of two quaternions applied to a geometry means "rotate the geometry around axis2 by rotation2, then rotate it around axis1 by rotation1." By using composition, you can combine the two rotations on the geometry to get a single quaternion that represents the result. Because quaternion interpolation can calculate a smooth and reasonable path from one axis and orientation to another, you can interpolate from the original to the composed quaternion to achieve a smooth transition from one to the other, enabling you to animate the transformation. For models that you want to animate, you can specify a destination Quaternion for the rotation by using a QuaternionRotation3D for the Rotation property.