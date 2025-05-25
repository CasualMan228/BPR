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
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;

namespace BPR
{
    /// <summary>
    /// Логика взаимодействия для SelectToBillsWindow.xaml
    /// </summary>
    public partial class SelectToBillsWindow : Window
    {
        ApplicationContext db;
        List<Bill> bills;
        List<Plane> planes;
        List <User> users;
        public SelectToBillsWindow()
        {
            InitializeComponent();
            db = MainWindow.db;
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WordButtonClick(object sender, RoutedEventArgs e)
        {
            var wordApp = new Word.Application();
            wordApp.Visible = true; //запустить Word
            Word.Document document = wordApp.Documents.Add(); //добавить документ
            Word.Paragraph text = document.Content.Paragraphs.Add(); //добавить абзац
            text.Range.Text = "Таблица учета бронирований (выставленных счетов)";
            text.Range.Font.Name = "Calibri";
            text.Range.Font.Size = 16;
            text.Range.Font.Bold = 1; //жирный
            text.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1); //сдвиг текста по горизонтальной линейке
            text.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter; //выравнивание по центру
            text.Range.InsertParagraphAfter(); //вставить абзац (последующий)
            bills = db.Bills.ToList();
            Word.Table table = document.Tables.Add(document.Bookmarks["\\endofdoc"].Range, bills.Count + 1, 8);
            //добавить таблицу + document.Bookmarks[...].Range -> куда вставить + строки + столбцы
            table.Range.Font.Name = "Calibri";
            table.Range.Font.Size = 9;
            table.Borders.Enable = 1; //есть рамки
            table.Cell(1, 1).Range.Text = "Id"; //первая строка + первый столбец
            table.Cell(1, 2).Range.Text = "UserName";
            table.Cell(1, 3).Range.Text = "UserRole";
            table.Cell(1, 4).Range.Text = "Regnum";
            table.Cell(1, 5).Range.Text = "Days";
            table.Cell(1, 6).Range.Text = "TotalPrice (BYN)";
            table.Cell(1, 7).Range.Text = "Date";
            table.Cell(1, 8).Range.Text = "IsRentNow";
            table.Rows[1].Range.Font.Bold = 1; //сделать первую строку жирной
            table.Rows[1].Shading.BackgroundPatternColor = Word.WdColor.wdColorLightTurquoise;
            planes = db.Planes.ToList();
            users = db.Users.ToList();
            for (int i = 0; i < bills.Count; i++)
            {
                var bill = bills[i];
                int row = i + 2;
                table.Cell(row, 1).Range.Text = bill.id.ToString();
                var foundUser = users.FirstOrDefault(user => user.id == bill.userId);
                table.Cell(row, 2).Range.Text = foundUser == null ? "USER NOT FOUND" : foundUser.name;
                table.Cell(row, 3).Range.Text = foundUser == null ? "USER NOT FOUND" : foundUser.role;
                var foundPlane = planes.FirstOrDefault(plane => plane.id == bill.planeId);
                table.Cell(row, 4).Range.Text = foundPlane == null ? "PLANE NOT FOUND" : foundPlane.regnum.ToString();
                table.Cell(row, 5).Range.Text = bill.days.ToString();
                table.Cell(row, 6).Range.Text = bill.totalPrice.ToString();
                table.Cell(row, 7).Range.Text = bill.date.ToString();
                table.Cell(row, 8).Range.Text = bill.isRentNow.ToString();
                table.Rows[row].Range.Font.Bold = 0;
            }
        }

        private void BillsButtonClick(object sender, RoutedEventArgs e)
        {
            //прописать в окне
        }
    }
}