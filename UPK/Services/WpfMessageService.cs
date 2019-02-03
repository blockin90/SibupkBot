using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UpkServices;
using UpkServices.UI;

namespace UPK.Services
{
    class WpfMessageService : IMessageService
    {
        MessageBoxButton ConvertButtons( Buttons button)
        {
            switch (button) {
                case Buttons.OkCancel:
                    return MessageBoxButton.OKCancel;
                case Buttons.YesNo:
                    return MessageBoxButton.YesNo;
                case Buttons.YesNoCancel:
                    return MessageBoxButton.YesNoCancel;
                default:
                    return MessageBoxButton.OK;
            }
        }
        public bool? ShowDialog(string message, string caption = "", Buttons button = Buttons.Ok)
        {
            MessageBoxResult result = MessageBox.Show(message, caption, ConvertButtons(button));
            if( result == MessageBoxResult.OK || result == MessageBoxResult.Yes) {
                return true;
            }else if( result == MessageBoxResult.No) {
                return false;
            }
            return null;
        }
    }
}