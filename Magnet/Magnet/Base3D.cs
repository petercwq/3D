using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Magnet
{
   abstract class  Base3D : ModelVisual3D
    {
        public Base3D()
        {
            Content = _content;

            _content.Geometry = Draw();
        }

        public static DependencyProperty StartingPointCubeProperty = DependencyProperty.Register("StartingPointCube", typeof(Point3D), typeof(Base3D), new PropertyMetadata(OnPoint3dChanged));

        public Point3D StartingPointCube
        {
            get { return (Point3D)GetValue(StartingPointCubeProperty); }
            set
            {
                SetValue(StartingPointCubeProperty, value);
            }
        }

        internal static void OnPoint3dChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            Base3D p = ((Base3D)sender);
            p.StartingPointCube = (Point3D)(e.NewValue);
            p._content.Geometry = p.Draw();
        }

        public static DependencyProperty WidthHeightDepthProperty = DependencyProperty.Register("WidthHeightDepth", typeof(double), typeof(Base3D), new PropertyMetadata(OnwidthHeightDepthChanged));

        public double WidthHeightDepth
        {
            get { return (double)GetValue(WidthHeightDepthProperty); }
            set { SetValue(WidthHeightDepthProperty, value); }
        }

        internal static void OnwidthHeightDepthChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            Base3D p = ((Base3D)sender);
            p.WidthHeightDepth = (double)(e.NewValue);
            p._content.Geometry = p.Draw();
        }

        public static DependencyProperty colorProperty = DependencyProperty.Register("color", typeof(Color), typeof(Base3D), new PropertyMetadata(colorPropertyChanged));

        public Color color
        {
            get { return (Color)GetValue(colorProperty); }
            set { SetValue(colorProperty, value); }
        }

        internal static void colorPropertyChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            Base3D p = ((Base3D)sender);
            p.color = (Color)(e.NewValue);
            if( p.Draw() != null)
            {
            p._content.Geometry = p.Draw();
            }
        }

        public static DependencyProperty opacityProperty = DependencyProperty.Register("opacity", typeof(double), typeof(Base3D), new PropertyMetadata(opacityPropertyChanged));

        public double opacity
        {
            get { return (double)GetValue(opacityProperty); }
            set { SetValue(opacityProperty, value); }
        }

        internal static void opacityPropertyChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            Base3D p = ((Base3D)sender);
            if (p.opacity != (double)(e.NewValue))
            {
                p.opacity = (double)(e.NewValue);
                p._content.Geometry = p.Draw();
            }
        }

        internal abstract Geometry3D Draw();

        internal readonly GeometryModel3D _content = new GeometryModel3D();
    }
}
