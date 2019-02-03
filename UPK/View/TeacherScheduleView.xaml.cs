using System.Windows;
using System.Windows.Controls;
using UpkModel.Database;
using UpkServices;
using UpkViewModel;

namespace UPK.View
{
    /// <summary>
    /// Логика взаимодействия для TeacherScheduleView.xaml
    /// </summary>
    public partial class TeacherScheduleView : UserControl
    {
        public TeacherScheduleView()
        {
            InitializeComponent();
        //    DataContext = new TeacherScheduleViewModel(null);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            popupDateSelector.IsOpen = true;
        }
    }
}
