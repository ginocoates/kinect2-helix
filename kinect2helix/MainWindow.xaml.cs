using kinect2helix.ViewModel;
using System.Windows;

namespace kinect2helix
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); 
            this.Loaded += MainWindow_Loaded;
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {   
            // attach the kinect scene to the viewport to display it
            (this.DataContext as MainViewModel).KinectScene.Attach(this.mainViewport);            
        }       
    }
}
