using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpkServices;
using UpkServices.UI;

namespace UPK.Services
{
    class WpfFileDialogService : IFileDialogService
    {
        public string FileName { get; set; }
        public string Filter { get; set; }

        public bool ShowOpenFileDialog()
        {
            return InitAndShowDialog( new OpenFileDialog());
        }
        public bool ShowSaveFileDialog()
        {
            return InitAndShowDialog(new SaveFileDialog());
        }
        private bool InitAndShowDialog( FileDialog fileDialog)
        {
            fileDialog.FileName = FileName;
            fileDialog.Filter = Filter;
            if (fileDialog.ShowDialog() == true) {
                FileName = fileDialog.FileName;
                return true;
            }
            return false;
        }
    }
}