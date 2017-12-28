using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Magnet
{
    public abstract class Primitive3D : ModelVisual3D
    {
        public Primitive3D()
        {
            Content = _content;
            _content.Geometry = Tessellate();
        }

        public static DependencyProperty RadiusProperty =
        DependencyProperty.Register("Radius", typeof(double), typeof(Primitive3D), new PropertyMetadata(OnRadiusChanged));

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }


        public static DependencyProperty LengthProperty =
         DependencyProperty.Register(
             "Length",
             typeof(double),
             typeof(Primitive3D), new PropertyMetadata(new PropertyChangedCallback(OnLengthChanged)));

        public double Length
        {
            get { return (double)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        internal static void OnRadiusChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            Primitive3D p = ((Primitive3D)sender);
            p.Radius = (double)(e.NewValue);
            p._content.Geometry = p.Tessellate();
        }

        internal static void OnLengthChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            Primitive3D p = ((Primitive3D)sender);
            p.Length = (double)(e.NewValue);
            p._content.Geometry = p.Tessellate();
        }

        public static DependencyProperty MaterialProperty =
            DependencyProperty.Register(
                "Material",
                typeof(Material),
                typeof(Primitive3D), new PropertyMetadata(
                    null, new PropertyChangedCallback(OnMaterialChanged)));

        public Material Material
        {
            get { return (Material)GetValue(MaterialProperty); }
            set { SetValue(MaterialProperty, value); }
        }

        public static DependencyProperty BackMaterialProperty =
           DependencyProperty.Register(
               "BackMaterial",
               typeof(Material),
               typeof(Primitive3D), new PropertyMetadata(
                   null, new PropertyChangedCallback(OnBackMaterialChanged)));

        public Material BackMaterial
        {
            get { return (Material)GetValue(BackMaterialProperty); }
            set { SetValue(BackMaterialProperty, value); }
        }

        internal static void OnBackMaterialChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            Primitive3D p = ((Primitive3D)sender);
            p._content.BackMaterial = p.BackMaterial;
            p._content.Geometry = p.Tessellate();

        }
        internal static void OnMaterialChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            Primitive3D p = ((Primitive3D)sender);

            p._content.Material = p.Material;
            p._content.Geometry = p.Tessellate();
        }

        internal static void OnGeometryChanged(DependencyObject d)
        {
            Primitive3D p = ((Primitive3D)d);

            p._content.Geometry = p.Tessellate();

        }

        internal double DegToRad(double degrees)
        {
            return (degrees / 180.0) * Math.PI;
        }

        internal abstract Geometry3D Tessellate();

        internal readonly GeometryModel3D _content = new GeometryModel3D();
    }
}
