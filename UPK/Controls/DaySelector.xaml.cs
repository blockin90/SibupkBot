using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UPK.Controls
{
    /// <summary>
    /// Логика взаимодействия для DaySelector.xaml
    /// </summary>
    public partial class DaySelector : UserControl
    {
        public static DependencyProperty SelectedDatesProperty;
        IEnumerable<DateTime> _oldValue = null; //
        static DaySelector()
        {
            SelectedDatesProperty = DependencyProperty.Register(
                "SelectedDates", 
                typeof(IEnumerable<DateTime>), 
                typeof(DaySelector),
                new FrameworkPropertyMetadata(null, OnSelectionChanged));
        }
        public DaySelector()
        {
            InitializeComponent();
        }
        static void OnSelectionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var daySelector = dependencyObject as DaySelector;
            var dates = eventArgs.NewValue as IEnumerable<DateTime>;
            if (dates != null) {
                daySelector.SetSelectionFromEnumerable(dates.First(), dates.Last());
                daySelector._oldValue = (eventArgs.NewValue as IEnumerable<DateTime>).ToArray();
            }
        }

        private void SetSelectionFromEnumerable( DateTime first, DateTime last)
        {
            calendar.SelectedDates.Clear();
            calendar.SelectedDates.AddRange(first, last);
            calendar.DisplayDate = first;            
        }

        public IEnumerable<DateTime> SelectedDates
        {
            get { return (IEnumerable<DateTime>)GetValue(SelectedDatesProperty); }
            set { SetValue(SelectedDatesProperty, value); }
        }

        private void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.Capture(null); //для того, чтобы Calendar потерял фокус, иначе нужен двойной клик по кнопке
        }

        private void PopupClose_Click(object sender, RoutedEventArgs e)
        {
            SelectedDates = calendar.SelectedDates.ToArray();
            var popup = Parent as Popup;
            if( popup != null) {
                popup.IsOpen = false;
            }
        }

        private void CancelAndClose_Click(object sender, RoutedEventArgs e)
        {
            SetSelectionFromEnumerable(_oldValue.First(), _oldValue.Last());
            var popup = Parent as Popup;
            if (popup != null) {
                popup.IsOpen = false;
            }
        }
    }
}
