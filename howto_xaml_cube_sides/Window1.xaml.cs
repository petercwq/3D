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
using System.Windows.Media.Media3D;

namespace howto_xaml_cube_sides
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        // Save the current image.
        private void mnuSave_Click(Object sender, RoutedEventArgs e)
        {
            // Draw the viewport into a RenderTargetBitmap.
            RenderTargetBitmap bm = new RenderTargetBitmap(
                (int)dockCube.ActualWidth, (int)dockCube.ActualHeight,
                96, 96, PixelFormats.Pbgra32);
            bm.Render(dockCube);

            // Make a PNG encoder.
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bm));

            // Save the file.
            using (FileStream fs = new FileStream("Saved.png",
                FileMode.Create, FileAccess.Write, FileShare.None))
            {
                encoder.Save(fs);
            }

            System.Media.SystemSounds.Beep.Play();
        }

        // Move the camera to the indicated position looking back at the origin.
        private void PositionCamera(float x, float y, float z, float yup)
        {
            hscroll.Value = 0;
            vscroll.Value = 0;
            PerspectiveCamera the_camera = viewCube.Camera as PerspectiveCamera;
            the_camera.Position = new Point3D(x, y, z);
            the_camera.LookDirection = new Vector3D(-x, -y, -z);
            the_camera.UpDirection = new Vector3D(0, yup, 0);

            Console.WriteLine(the_camera.Position.ToString());
            Console.WriteLine(the_camera.LookDirection.ToString());
            Console.WriteLine(the_camera.UpDirection.ToString());
            Console.WriteLine("**********");
        }

        // Move the camera to a specific position.
        private void btnView_Click(Object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string txt = item.Header.ToString().Replace("(", "").Replace(")", "");
            string[] values = txt.Split(',');
            float x = 3 * float.Parse(values[0]);
            float y = 3 * float.Parse(values[1]);
            float z = 3 * float.Parse(values[2]);
            float yup = y > 0 ? 1 : -1;
            PositionCamera(x, y, z, yup);
        }
    }
}
