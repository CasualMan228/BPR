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
using Word = Microsoft.Office.Interop.Word;

namespace BPR
{
    /// <summary>
    /// Логика взаимодействия для SelectToStatisticsWindow.xaml
    /// </summary>
    public partial class SelectToStatisticsWindow : Window
    {
        ApplicationContext db;
        List<Bill> bills;
        List<Plane> planes;
        List<User> users;
        public SelectToStatisticsWindow()
        {
            InitializeComponent();
            db = MainWindow.db;
        }

        private void WordButtonClick(object sender, RoutedEventArgs e)
        {
            var wordApp = new Word.Application();
            wordApp.Visible = true; //запустить Word
            Word.Document document = wordApp.Documents.Add(); //добавить документ
            Word.Paragraph text4 = document.Content.Paragraphs.Add(); //добавить абзац
            text4.Range.Text = "Общая статистика";
            text4.Range.Font.Name = "Calibri";
            text4.Range.Font.Size = 16;
            text4.Range.Font.Bold = 1; //жирный
            text4.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1); //сдвиг текста по горизонтальной линейке
            text4.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter; //выравнивание по центру
            text4.Range.InsertParagraphAfter(); //вставить абзац (последующий)
            //1
            string textq = db.Bills.Count().ToString();
            Word.Paragraph textw = document.Content.Paragraphs.Add(); //добавить абзац
            textw.Range.Text = $"Общее количество аренд самолетов: {textq}";
            textw.Range.Font.Name = "Calibri";
            textw.Range.Font.Size = 12;
            textw.Range.Font.Bold = 0;
            textw.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
            textw.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            textw.Range.InsertParagraphAfter();
            //2
            string textq2 = db.Planes.Count().ToString();
            Word.Paragraph textw2 = document.Content.Paragraphs.Add(); //добавить абзац
            textw2.Range.Text = $"Общее количество самолетов: {textq2}";
            textw2.Range.Font.Name = "Calibri";
            textw2.Range.Font.Size = 12;
            textw2.Range.Font.Bold = 0;
            textw2.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
            textw2.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            textw2.Range.InsertParagraphAfter();
            //3
            string textq3 = db.Bills.Count(b => b.isRentNow).ToString();
            Word.Paragraph textw3 = document.Content.Paragraphs.Add(); //добавить абзац
            textw3.Range.Text = $"Из них, находящихся в аренде: {textq3}";
            textw3.Range.Font.Name = "Calibri";
            textw3.Range.Font.Size = 12;
            textw3.Range.Font.Bold = 0;
            textw3.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
            textw3.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            textw3.Range.InsertParagraphAfter();
            //4
            string textq4 = db.Users.Count().ToString();
            Word.Paragraph textw4 = document.Content.Paragraphs.Add(); //добавить абзац
            textw4.Range.Text = $"Общее количество пользователей: {textq4}";
            textw4.Range.Font.Name = "Calibri";
            textw4.Range.Font.Size = 12;
            textw4.Range.Font.Bold = 0;
            textw4.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
            textw4.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            textw4.Range.InsertParagraphAfter();
            //5
            string textq5 = db.Users.Count(u => u.role == "admin").ToString();
            Word.Paragraph textw5 = document.Content.Paragraphs.Add(); //добавить абзац
            textw5.Range.Text = $"Из них - количество администраторов: {textq5}";
            textw5.Range.Font.Name = "Calibri";
            textw5.Range.Font.Size = 12;
            textw5.Range.Font.Bold = 0;
            textw5.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
            textw5.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            textw5.Range.InsertParagraphAfter();
            //6
            string textq6 = db.Bills.Sum(b => b.totalPrice).ToString("N0");
            Word.Paragraph textw6 = document.Content.Paragraphs.Add(); //добавить абзац
            textw6.Range.Text = $"Общая выручка (BYN): {textq6}";
            textw6.Range.Font.Name = "Calibri";
            textw6.Range.Font.Size = 12;
            textw6.Range.Font.Bold = 0;
            textw6.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
            textw6.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            textw6.Range.InsertParagraphAfter();
            //7
            var mostPopularPlaneId = db.Bills.GroupBy(b => b.planeId).OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault();
            Plane textq7 = db.Planes.FirstOrDefault(p => p.id == mostPopularPlaneId);
            Word.Paragraph textw7 = document.Content.Paragraphs.Add(); //добавить абзац
            if (textq7 != null)
                textw7.Range.Text = $"Самый популярный самолет для аренды: {textq7.name}";
            else
                textw7.Range.Text = "Самый популярный самолет для аренды: данных нет";
            textw7.Range.Font.Name = "Calibri";
            textw7.Range.Font.Size = 12;
            textw7.Range.Font.Bold = 0;
            textw7.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
            textw7.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            textw7.Range.InsertParagraphAfter();
            //8
            string textq8 = db.Bills.Join(db.Planes, b => b.planeId, p => p.id, (b, p) => p.maker)
                .GroupBy(m => m)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();
            Word.Paragraph textw8 = document.Content.Paragraphs.Add(); //добавить абзац
            if (textq8 != null)
                textw8.Range.Text = $"Самый популярный производитель самолетов: {textq8}";
            else
                textw8.Range.Text = "Самый популярный производитель самолетов: данных нет";
            textw8.Range.Font.Name = "Calibri";
            textw8.Range.Font.Size = 12;
            textw8.Range.Font.Bold = 0;
            textw8.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
            textw8.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            textw8.Range.InsertParagraphAfter();
            //9
            string textq9 = db.Bills.Join(db.Planes,
                    b => b.planeId,
                    p => p.id,
                    (b, p) => p.country)
             .GroupBy(c => c)
             .OrderByDescending(g => g.Count())
             .Select(g => g.Key)
             .FirstOrDefault();
            Word.Paragraph textw9 = document.Content.Paragraphs.Add(); //добавить абзац
            if (textq9 != null)
                textw9.Range.Text = $"Самая популярная страна регистрации самолетов: {textq9}";
            else
                textw9.Range.Text = "Самая популярная страна регистрации самолетов: данных нет";
            textw9.Range.Font.Name = "Calibri";
            textw9.Range.Font.Size = 12;
            textw9.Range.Font.Bold = 0;
            textw9.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
            textw9.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            textw9.Range.InsertParagraphAfter();
            //10
            string textq10 = db.Bills
           .Join(db.Planes,
                 b => b.planeId,
                 p => p.id,
                 (b, p) => p.type)
           .GroupBy(t => t)
           .OrderByDescending(g => g.Count())
           .Select(g => g.Key)
           .FirstOrDefault();
            Word.Paragraph textw10 = document.Content.Paragraphs.Add(); //добавить абзац
            if (textq10 != null)
                textw10.Range.Text = $"Самый популярный тип самолетов: {textq10}";
            else
                textw10.Range.Text = "Самый популярный тип самолетов: данных нет";
            textw10.Range.Font.Name = "Calibri";
            textw10.Range.Font.Size = 12;
            textw10.Range.Font.Bold = 0;
            textw10.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
            textw10.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            textw10.Range.InsertParagraphAfter();
            //11
            string textq11 = db.Bills
            .Join(db.Planes,
                b => b.planeId,
                p => p.id,
                (b, p) => p.category)
            .GroupBy(cat => cat)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();
            Word.Paragraph textw11 = document.Content.Paragraphs.Add(); //добавить абзац
            if (textq11 != null)
                textw11.Range.Text = $"Самая популярная категория самолетов: {textq11}";
            else
                textw11.Range.Text = "Самая популярная категория самолетов: данных нет";
            textw11.Range.Font.Name = "Calibri";
            textw11.Range.Font.Size = 12;
            textw11.Range.Font.Bold = 0;
            textw11.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
            textw11.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            textw11.Range.InsertParagraphAfter();
            //
            Word.Paragraph emptyParagraph3 = document.Content.Paragraphs.Add();
            emptyParagraph3.Range.InsertParagraphAfter();
            emptyParagraph3.Range.InsertBreak(Word.WdBreakType.wdPageBreak); //сделать разрыв страницы
            //
            Word.Paragraph text = document.Content.Paragraphs.Add(); //добавить абзац
            text.Range.Text = "Таблица выставленных счетов (BILLS)";
            text.Range.Font.Name = "Calibri";
            text.Range.Font.Size = 16;
            text.Range.Font.Bold = 1; //жирный
            text.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1); //сдвиг текста по горизонтальной линейке
            text.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter; //выравнивание по центру
            text.Range.InsertParagraphAfter(); //вставить абзац (последующий)
            //
            bills = db.Bills.ToList();
            Word.Table table = document.Tables.Add(document.Bookmarks["\\endofdoc"].Range, bills.Count + 1, 7); //добавить таблицу + document.Bookmarks[...].Range -> куда вставить + строки + столбцы
            table.Range.Font.Name = "Calibri";
            table.Range.Font.Size = 9;
            table.Borders.Enable = 1; //есть рамки
            table.Cell(1, 1).Range.Text = "Id"; //первая строка + первый столбец
            table.Cell(1, 2).Range.Text = "UserId";
            table.Cell(1, 3).Range.Text = "PlaneId";
            table.Cell(1, 4).Range.Text = "Days";
            table.Cell(1, 5).Range.Text = "TotalPrice (BYN)";
            table.Cell(1, 6).Range.Text = "Date";
            table.Cell(1, 7).Range.Text = "IsRentNow";
            table.Rows[1].Range.Font.Bold = 1; //сделать первую строку жирной
            table.Rows[1].Shading.BackgroundPatternColor = Word.WdColor.wdColorLightTurquoise;
            for (int i = 0; i < bills.Count; i++)
            {
                var bill = bills[i];
                int row = i + 2;
                table.Cell(row, 1).Range.Text = bill.id.ToString();
                table.Cell(row, 2).Range.Text = bill.userId.ToString();
                table.Cell(row, 3).Range.Text = bill.planeId.ToString();
                table.Cell(row, 4).Range.Text = bill.days.ToString();
                table.Cell(row, 5).Range.Text = bill.totalPrice.ToString();
                table.Cell(row, 6).Range.Text = bill.date.ToString();
                table.Cell(row, 7).Range.Text = bill.isRentNow.ToString();
                table.Rows[row].Range.Font.Bold = 0;
            }
            //
            Word.Paragraph emptyParagraph = document.Content.Paragraphs.Add();
            emptyParagraph.Range.InsertParagraphAfter();
            emptyParagraph.Range.InsertBreak(Word.WdBreakType.wdPageBreak); //сделать разрыв страницы
            //
            Word.Paragraph text2 = document.Content.Paragraphs.Add(); //добавить абзац
            text2.Range.Text = "Таблица самолетов (PLANES)";
            text2.Range.Font.Name = "Calibri";
            text2.Range.Font.Size = 16;
            text2.Range.Font.Bold = 1; //жирный
            text2.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1); //сдвиг текста по горизонтальной линейке
            text2.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter; //выравнивание по центру
            text2.Range.InsertParagraphAfter(); //вставить абзац (последующий)
            //
            planes = db.Planes.ToList();
            Word.Table table2 = document.Tables.Add(document.Bookmarks["\\endofdoc"].Range, planes.Count + 1, 11); //добавить таблицу + document.Bookmarks[...].Range -> куда вставить + строки + столбцы
            table2.Range.Font.Name = "Calibri";
            table2.Range.Font.Size = 9;
            table2.Borders.Enable = 1; //есть рамки
            table2.Cell(1, 1).Range.Text = "Id"; //первая строка + первый столбец
            table2.Cell(1, 2).Range.Text = "Name";
            table2.Cell(1, 3).Range.Text = "Year";
            table2.Cell(1, 4).Range.Text = "Maker";
            table2.Cell(1, 5).Range.Text = "Regnum";
            table2.Cell(1, 6).Range.Text = "Country";
            table2.Cell(1, 7).Range.Text = "Type";
            table2.Cell(1, 8).Range.Text = "Category";
            table2.Cell(1, 9).Range.Text = "TotalFly (km)";
            table2.Cell(1, 10).Range.Text = "Price (BYN/per day)";
            table2.Cell(1, 11).Range.Text = "Description";
            table2.Rows[1].Range.Font.Bold = 1; //сделать первую строку жирной
            table2.Rows[1].Shading.BackgroundPatternColor = Word.WdColor.wdColorLightTurquoise;
            for (int i = 0; i < planes.Count; i++)
            {
                var plane = planes[i];
                int row = i + 2;
                table2.Cell(row, 1).Range.Text = plane.id.ToString();
                table2.Cell(row, 2).Range.Text = plane.name;
                table2.Cell(row, 3).Range.Text = plane.year.ToString();
                table2.Cell(row, 4).Range.Text = plane.maker;
                table2.Cell(row, 5).Range.Text = plane.regnum;
                table2.Cell(row, 6).Range.Text = plane.country;
                table2.Cell(row, 7).Range.Text = plane.type;
                table2.Cell(row, 8).Range.Text = plane.category;
                table2.Cell(row, 9).Range.Text = plane.totalFly.ToString();
                table2.Cell(row, 10).Range.Text = plane.price.ToString();
                table2.Cell(row, 11).Range.Text = plane.description;
                table2.Rows[row].Range.Font.Bold = 0;
            }
            //
            Word.Paragraph emptyParagraph2 = document.Content.Paragraphs.Add();
            emptyParagraph2.Range.InsertParagraphAfter();
            emptyParagraph2.Range.InsertBreak(Word.WdBreakType.wdPageBreak); //сделать разрыв страницы
            //
            Word.Paragraph text3 = document.Content.Paragraphs.Add(); //добавить абзац
            text3.Range.Text = "Таблица пользователей (USERS)";
            text3.Range.Font.Name = "Calibri";
            text3.Range.Font.Size = 16;
            text3.Range.Font.Bold = 1; //жирный
            text3.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1); //сдвиг текста по горизонтальной линейке
            text3.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter; //выравнивание по центру
            text3.Range.InsertParagraphAfter(); //вставить абзац (последующий)
            //
            users = db.Users.ToList();
            Word.Table table3 = document.Tables.Add(document.Bookmarks["\\endofdoc"].Range, users.Count + 1, 4); //добавить таблицу + document.Bookmarks[...].Range -> куда вставить + строки + столбцы
            table3.Range.Font.Name = "Calibri";
            table3.Range.Font.Size = 9;
            table3.Borders.Enable = 1; //есть рамки
            table3.Cell(1, 1).Range.Text = "Id"; //первая строка + первый столбец
            table3.Cell(1, 2).Range.Text = "Name";
            table3.Cell(1, 3).Range.Text = "Role";
            table3.Cell(1, 4).Range.Text = "Pass";
            table3.Rows[1].Range.Font.Bold = 1; //сделать первую строку жирной
            table3.Rows[1].Shading.BackgroundPatternColor = Word.WdColor.wdColorLightTurquoise;
            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                int row = i + 2;
                table3.Cell(row, 1).Range.Text = user.id.ToString();
                table3.Cell(row, 2).Range.Text = user.name;
                table3.Cell(row, 3).Range.Text = user.role;
                table3.Cell(row, 4).Range.Text = user.pass;
                table3.Rows[row].Range.Font.Bold = 0;
            }
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StatisticsButtonClick(object sender, RoutedEventArgs e)
        {
            //просмотр статистики в окне
        }
    }
}