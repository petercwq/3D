/// <summary>
/// 
/// </summary>
namespace Magnet
{
    using System.Windows;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    enum Direction
    {
        Left,
        Right,
        Up,
        Down,
        Front,
        Back,
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MagnetViewModel viewModel = new MagnetViewModel();
            this.DataContext = viewModel;
            viewModel.CollectionChanged += new System.ComponentModel.PropertyChangedEventHandler(ViewModel_CollectionChangedChanged);

            for (int k = 0; k < viewModel.CubesCollection.Count; k++)
            {
                this.ViewPort3dPentagon.Children.Add(viewModel.CubesCollection[k]);
            }

            this.ViewPort3dPentagon.Focusable = true;
            this.ViewPort3dPentagon.Focus();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ViewModel_CollectionChangedChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            List<Cube> removeCubes = sender as  List<Cube>;

            for (int k = 0; k < removeCubes.Count; k++)
            {
                this.ViewPort3dPentagon.Children.Remove(removeCubes[k]);
            }
        }
    }
}