using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Threading;

namespace Magnet
{
    class Map : INotifyPropertyChanged
    {
        private int latitude=20;
        private int longitude=50;
        private int zoom=14;
        private BitmapImage backImage;
        BitmapImage localimage;
        EventWaitHandle handle = new EventWaitHandle(true, EventResetMode.AutoReset);

        public Map()
        {
           GetImage();
       
        }

        
        Color color = Colors.Green;
        
        public int Longitude
        {
            get
            {
                return longitude;
            }
            set
            {
                longitude = value;
                GetImage();
            }
        }
        public int Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                zoom = value;
                GetImage();
            }
        }
        public int Latitude
        {
            get
            {
                return latitude;
            }
            set
            {
                latitude = value;
                GetImage();
            }
        }
        public BitmapImage BackImage
        {
            get
            {
                return backImage;
            }
            set
            {
                this.backImage = value;
                              
                
                this.RaisePropertyChange("BackImage");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChange(string propertyname)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        #endregion

        public void GetImage()
        {
            //if (handle.WaitOne(100))
            //{
            //    string address = "http://maps.google.com/maps/api/staticmap?center=" + this.Latitude + "," + this.Longitude + "&format=jpg&zoom=" + this.Zoom + "&maptype=hybrid&size=400x400&sensor=false";

            //    Uri url = new Uri(address);

            //    localimage = new BitmapImage(url);
            //    this.BackImage = localimage;
            //   // localimage.DownloadCompleted+=new EventHandler(localimage_DownloadCompleted);
            //    handle.Set();
            //}
        }
        void localimage_DownloadCompleted(object sender, EventArgs e)
        {
            this.BackImage = sender as BitmapImage;

            (sender as BitmapImage).DownloadCompleted -= localimage_DownloadCompleted;
        }
    }
}
