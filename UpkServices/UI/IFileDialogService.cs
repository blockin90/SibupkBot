using System;
using System.Collections.Generic;
using System.Text;

namespace UpkServices.UI
{
    /// <summary>
    /// Предоставляет доступ к диалоговым окнам открытия и закрытия файла
    /// </summary>
    public interface IFileDialogService
    {
        string FileName { get; set; }
        string Filter { get; set; }
        bool ShowSaveFileDialog();
        bool ShowOpenFileDialog();
    }
}
