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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UPK.Controls
{
    /// <summary>
    /// Логика взаимодействия для TabItem.xaml
    /// </summary>
    public partial class TabItem : UserControl
    {
        public static DependencyProperty HeaderProperty;
        public static DependencyProperty CommandProperty;
        public static DependencyProperty CommandParameterProperty;
        public static DependencyProperty IsSelectedProperty;
        static TabItem()
        {
            HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(TabItem), 
                new FrameworkPropertyMetadata("Empty Tab", FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));
            CommandProperty = DependencyProperty.Register("Command",typeof(ICommand),typeof(TabItem));
            CommandParameterProperty = DependencyProperty.Register("CommandParameter",typeof(object),typeof(TabItem));
            IsSelectedProperty = DependencyProperty.Register("IsSelected",typeof(bool),typeof(TabItem));
        }

        public TabItem()
        {
            InitializeComponent();
            rBtn.Checked += RadioButton_Checked;
            rBtn.Unchecked += RadioButton_Unchecked;
        }
        public string GroupName
        {
            get => rBtn.GroupName; 
            set => rBtn.GroupName = value; 
        }
        public string Header
        {
            get => (string)GetValue(HeaderProperty); 
            set => SetValue(HeaderProperty, value); 
        }
        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue( CommandProperty, value);
        }
        public object CommandParameter
        {
            get => (object) GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
        private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {

            IsSelected = false;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            IsSelected = true;
        }

        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }
    }
}
