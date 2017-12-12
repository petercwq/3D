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

namespace WpfMyCube
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            System.Drawing.KnownColor t = new System.Drawing.KnownColor();
            foreach (System.Drawing.KnownColor kc in System.Enum.GetValues(t.GetType()))
            {
                System.Drawing.ColorConverter cc = new System.Drawing.ColorConverter();
                System.Drawing.Color c = System.Drawing.Color.FromName(kc.ToString());

                if (!c.IsSystemColor)
                    cbColors.Items.Add(c);
            }

            cbColors.SelectedIndex = 0;

            Render();
        }

        private void rotateX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RotateX(e.NewValue);
        }

        private void rotationY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RotateY(e.NewValue);
        }

        private void rotationZ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RotateZ(e.NewValue);
        }

        private void cbColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Drawing.Color color = (System.Drawing.Color)cbColors.SelectedItem;

            CubeColor = System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
            Render();

        }
        public Color CubeColor {get; set;}

        public void RotateX(double angle)
        {
            rotX.Angle = angle;
        }

        public void RotateY(double angle)
        {
            rotY.Angle = angle;
        }

        public void RotateZ(double angle)
        {
            rotZ.Angle = angle;
        }

        public void Render()
        {
            CubeBuilder cubeBuilder = new CubeBuilder(CubeColor);

            // origin
            mainViewport.Children.Add(cubeBuilder.Create(0, 0, 0));

            //side 1
            mainViewport.Children.Add(cubeBuilder.Create(6, 0, 0));
            mainViewport.Children.Add(cubeBuilder.Create(12, 0, 0));
            mainViewport.Children.Add(cubeBuilder.Create(18, 0, 0));
            mainViewport.Children.Add(cubeBuilder.Create(24, 0, 0));

            //side 2
            mainViewport.Children.Add(cubeBuilder.Create(24, 6, 0));
            mainViewport.Children.Add(cubeBuilder.Create(24, 12, 0));

            //side 3
            mainViewport.Children.Add(cubeBuilder.Create(24, 18, 0));
            mainViewport.Children.Add(cubeBuilder.Create(18, 18, 0));
            mainViewport.Children.Add(cubeBuilder.Create(12, 18, 0));
            mainViewport.Children.Add(cubeBuilder.Create(6, 18, 0));
            mainViewport.Children.Add(cubeBuilder.Create(0, 18, 0));

            //side 4
            mainViewport.Children.Add(cubeBuilder.Create(0, 12, 0));
            mainViewport.Children.Add(cubeBuilder.Create(0, 6, 0));

            //corner 1
            mainViewport.Children.Add(cubeBuilder.Create(0, 0, 6));
            mainViewport.Children.Add(cubeBuilder.Create(0, 0, 12));
            //mainViewport.Children.Add(cubeBuilder.Create(0, 0, 18));
            //mainViewport.Children.Add(cubeBuilder.Create(0, 0, 24));

            //other corners
            mainViewport.Children.Add(cubeBuilder.Create(24, 0, 6));
            mainViewport.Children.Add(cubeBuilder.Create(0, 18, 6));
            mainViewport.Children.Add(cubeBuilder.Create(24, 18, 6));


        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            mCamera.Position = new System.Windows.Media.Media3D.Point3D(
                mCamera.Position.X,
                mCamera.Position.Y,
                mCamera.Position.Z - e.Delta / 250D);

        }

        private void mBtn_Click(object sender, RoutedEventArgs e)
        {
        }


    }



}
