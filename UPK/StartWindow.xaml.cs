using UpkModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UPK.View;
using UpkViewModel;
using UpkModel.Database;
using UpkServices;
using UPK.Services;
using UpkServices.UI;

namespace SupkServices
{
    /// <summary>
    /// Логика взаимодействия для StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();

            ServiceProvider.RegisterService(typeof(IFileDialogService), typeof(WpfFileDialogService));
            ServiceProvider.RegisterService(typeof(IMessageService), new WpfMessageService());

           // DataContext = new MainTeacherViewModel();
        }
        
        #region Top menu button handlers
        private void WindowHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        
        private void MaximizeRestore_Click(object sender,  RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Minimize_Click(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }
        #endregion
    }
}
