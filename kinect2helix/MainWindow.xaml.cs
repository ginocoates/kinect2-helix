using kinect2helix.ViewModel;
using System.Windows;

namespace kinect2helix
{
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel
        {
            get
            {
                return this.DataContext as MainViewModel;
            }
        }

        public HelixToolkit.Wpf.HelixViewport3D Viewport
        {
            get
            {
                return this.mainViewport;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // attach the kinect scene to the viewport to display it
            this.ViewModel.KinectScene.Attach(this.Viewport);
        }
    }
}
