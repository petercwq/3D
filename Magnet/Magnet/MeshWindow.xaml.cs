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
using System.IO;
using System.Net;
using System.Windows.Media.Media3D;
using System.ComponentModel;

namespace DNA1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Map obj = new Map();
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = obj;
           // circlecoordinates();
            //TriangleCoordinates();
           // Sphercoordinates();

           // GetCylinderCoordinates();
          //
        }


        public void Sphercoordinates()
        {
            double x, y, z, r = 5;

            double startx = 6, starty = -15, startz = -16;
            StringBuilder BUILDER = new StringBuilder();

            FileStream file = File.OpenWrite("SphereCoordinates.txt");

            file.Close();

            BUILDER.Append(startx);
            BUILDER.Append(" ");
            BUILDER.Append(starty);
            BUILDER.Append(" ");
            BUILDER.Append(-32);

            BUILDER.Append(",");
            for (int k = 0; k < 90; k = k + 5)
            {
                for (int i = 0; i < 180; i = i + 1)
                {
                    x = r * Math.Cos((Math.PI / 180) * i) * Math.Sin((Math.PI / 180) * k) + startx;
                    y = r * Math.Sin((Math.PI / 180) * i) * Math.Sin((Math.PI / 180) * k) + starty;
                    z = r * Math.Cos((Math.PI / 180) * k) + startz;
                    BUILDER.Append(x);
                    BUILDER.Append(" ");
                    BUILDER.Append(y);
                    BUILDER.Append(" ");
                    BUILDER.Append(z);

                    BUILDER.Append(",");
                }
            }
            try
            {
                File.WriteAllText("SphereCoordinates.txt", BUILDER.ToString(), Encoding.Unicode);
            }
            catch (Exception e)
            {

            }

            StringBuilder TriangleBuilder = new StringBuilder();
            FileStream file1 = File.OpenWrite("SphereCoordinatesTriangle1.txt");
            file1.Close();
            for (int K = 0; K < 15 * 60; K++)
            {

                TriangleBuilder.Append((K) % (360 * 180));
                TriangleBuilder.Append(" ");
                TriangleBuilder.Append((K + 1) % (360 * 180));
                TriangleBuilder.Append(" ");
                TriangleBuilder.Append((K + 4) % (360 * 180));

                TriangleBuilder.Append(",");
            }
            try
            {
                File.WriteAllText("SphereCoordinatesTriangle1.txt", TriangleBuilder.ToString(), Encoding.ASCII);
            }
            catch (Exception e)
            {

            }

        }
        public void TriangleCoordinates()
        {

            StringBuilder BUILDER = new StringBuilder();
            FileStream file = File.OpenWrite("newfile1.txt");
            file.Close();

            for (int K = 0; K < 360; K++)
            {

                BUILDER.Append(K);
                BUILDER.Append(" ");
                BUILDER.Append((K + 16) % 360);
                BUILDER.Append(" ");
                BUILDER.Append((K + 24) % 360);

                BUILDER.Append(",");

            }
            try
            {
                File.WriteAllText("newfile1.txt", BUILDER.ToString(), Encoding.ASCII);
            }
            catch (Exception E)
            {

            }
        }

        public void circlecoordinates()
        {
            //0 -10 0
            //0 -15 0
            //0 -15 40,

            //0 -6 44
            double x, y, z, r = 5;
            double j = -6;
            double k = 44;

            StringBuilder BUILDER = new StringBuilder();
            FileStream file = File.OpenWrite("circlefile.txt");
            file.Close();

            //BUILDER.Append(0);
            //BUILDER.Append(" ");
            //BUILDER.Append(-6);
            //BUILDER.Append(" ");
            //BUILDER.Append(44);

            //BUILDER.Append(",");

            for (int i = 0; i < 360; i = i + 1)
            {
                x = 0;
                y = r * Math.Cos((Math.PI / 180) * i) + j;
                z = r * Math.Sin((Math.PI / 180) * i) + k;

                BUILDER.Append(x);
                BUILDER.Append(" ");
                BUILDER.Append(y);
                BUILDER.Append(" ");
                BUILDER.Append(z);

                BUILDER.Append(",");
            }
            try
            {

                File.WriteAllText("circlefile.txt", BUILDER.ToString(), Encoding.ASCII);
            }
            catch (Exception e)
            {

            }

            StringBuilder BUILDER1 = new StringBuilder();
            FileStream file1 = File.OpenWrite("circlefilecoor.txt");
            file1.Close();

            for (int K = 0; K < 360; K++)
            {
                BUILDER1.Append(K);
                BUILDER1.Append(" ");
                BUILDER1.Append((K + 7) % 360);
                BUILDER1.Append(" ");
                BUILDER1.Append((K + 14) % 360);
                BUILDER1.Append(",");
            }
            try
            {
                File.WriteAllText("circlefilecoor.txt", BUILDER1.ToString(), Encoding.ASCII);
            }
            catch (Exception E)
            {

            }
        }

        public void GetCylinderCoordinates()
        {
            double x, y, z, radius = 2;

            double startx = 6, starty = -25, startz = -6;
            double cylinderLength = 14;
            StringBuilder BUILDER = new StringBuilder();

            FileStream file = File.OpenWrite("CylinderCoordinates.txt");

            file.Close();

            BUILDER.Append(startx);
            BUILDER.Append(" ");
            BUILDER.Append(starty);
            BUILDER.Append(" ");
            BUILDER.Append(1);

            BUILDER.Append(",");
            for (int k = 0; k < cylinderLength; k = k + 1)
            {
                for (int i = 0; i < 360; i = i + 1)
                {
                    x = radius * Math.Cos((Math.PI / 180) * i) + startx;
                    y = radius * Math.Sin((Math.PI / 180) * i) + starty;
                    z = k + startz;
                    BUILDER.Append(x);
                    BUILDER.Append(" ");
                    BUILDER.Append(y);
                    BUILDER.Append(" ");
                    BUILDER.Append(z);

                    BUILDER.Append(",");
                }
            }
            try
            {
                File.WriteAllText("CylinderCoordinates.txt", BUILDER.ToString(), Encoding.Unicode);
            }
            catch (Exception e)
            {

            }

            StringBuilder TriangleBuilder = new StringBuilder();
            FileStream file1 = File.OpenWrite("CylinderCoordinatesTriangle1.txt");
            file1.Close();

            for (int K = 0; K < 360 * cylinderLength; K++)
            {

                TriangleBuilder.Append(0);
                TriangleBuilder.Append(" ");
                TriangleBuilder.Append((K + 1) % (360 * cylinderLength));
                TriangleBuilder.Append(" ");
                TriangleBuilder.Append((K + 2) % (360 * cylinderLength));

                TriangleBuilder.Append(",");

            }
            try
            {
                File.WriteAllText("CylinderCoordinatesTriangle1.txt", TriangleBuilder.ToString(), Encoding.ASCII);
            }
            catch (Exception e)
            {

            }

        }

        private void CameraZLook_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            imagebrush.ImageSource = obj.BackImage;
            imagebrushback.ImageSource = obj.BackImage;
        }

    }


    //public class MultiViewAngleCoverter : IMultiValueConverter
    //{

    //    #region IMultiValueConverter Members

    //    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        //Point3D pt = new Point3D();
    //        //double zoomval = (double)values[2];

    //        //pt = new Point3D((double)values[0], (double)values[1], 7 * ((double)zoomval + 16));
    //        // MessageBox.Show(70- 7* (double)zoomval + " ");
    //        return new Point3D((double)values[0], (double)values[1],((double)values[2]));
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion
    //}

    //public class TeamMemberImageBrushConverter : IValueConverter
    //{

    //    #region IValueConverter Members

    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        BitmapImage _i = (BitmapImage)value;
    //        ImageBrush _b = new ImageBrush();
    //        if (_i != null)
    //        {
    //            _b.ImageSource = _i;
    //        }
    //        return _b;

    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion
    //}
}