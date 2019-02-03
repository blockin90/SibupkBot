using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UPK.View
{
    /// <summary>
    /// Логика взаимодействия для TeacherReportsView.xaml
    /// </summary>
    public partial class TeacherReportsView : UserControl
    {
        public TeacherReportsView()
        {
            InitializeComponent();
            //13 месяц пустой:
            cbMonth.ItemsSource = DateTimeFormatInfo.CurrentInfo.MonthNames.Where( m => m != String.Empty);
        }
    }
}
