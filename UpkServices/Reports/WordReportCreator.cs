using System;
using System.Collections.Generic;
using System.Text;
using UpkModel.Database;
using UpkModel.Database.Schedule;

namespace UpkServices.Reports
{
    public static class WordReportCreator
    {
        //todo: платформозависимый код, перенести в виндовую версию
        public static void MakeWordReport( string fileName, IEnumerable<Lesson> lessons )
        {
            // Создаём объект документа
            /*Word.Document doc = null;
            try {
                // Создаём объект приложения
                Word.Application app = new Word.Application();
                // Путь до шаблона документа
                string source = @"D:\\Test.docx";
                // Открываем
                doc = app.Documents.Open(source);
                doc.Activate();

                // Добавляем информацию
                // wBookmarks содержит все закладки
                Word.Bookmarks wBookmarks = doc.Bookmarks;
                Word.Range wRange;
                int i = 0;
                string[] data = new string[3] { "27", "Alex", "Gulynin" };
                foreach (Word.Bookmark mark in wBookmarks) {
                    
                    wRange = mark.Range;
                    wRange.Text = data[i];
                    i++;
                }

                // Закрываем документ
                doc.Close();
                doc = null;
            } catch (Exception ex) {
                // Если произошла ошибка, то
                // закрываем документ и выводим информацию
                doc.Close();
                doc = null;
                Console.WriteLine("Во время выполнения произошла ошибка!");
                Console.ReadLine();
            }*/
        }
    }
}
