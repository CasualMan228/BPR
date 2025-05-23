using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для InputToRentPlaneWindow.xaml
    /// </summary>
    public partial class InputToRentPlaneWindow : Window 
    {//ПЕЧАТЬ ВОРД ТИП ЧЕК
        ApplicationContext db;
        List<Plane> planes;
        List<Bill> bills;
        List<User> users;
        SolidColorBrush brushDefault;
        string patternDigit = @"^\d+$";
        bool isAdmin = false;
        public string currentUserName;
        public Plane currentPlane;
        public bool isClosedNormal;
        public InputToRentPlaneWindow(Plane plane, string userRole, string userName)
        {
            InitializeComponent();
            isClosedNormal = false;
            db = MainWindow.db;
            currentPlane = plane;
            currentUserName = userName;
            if (userRole == "admin")
            {
                isAdmin = true;
            }
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            planes = db.Planes.ToList(); //вытянуть с бд
            users = db.Users.ToList();
            string days = textBoxDays.Text;
            bool isCorrect = true;
            if (days == "")
            {
                textBoxDays.ToolTip = "Данное поле пустое! Разрешены только цифры (1-4 символов)";
                textBoxDays.BorderBrush = Brushes.DarkRed;
                isCorrect = false;
            }
            else if (days.Length < 1 || days.Length > 4 || !Regex.IsMatch(days, patternDigit) || int.Parse(days) < 1)
            {
                textBoxDays.ToolTip = "Данное поле введено некорректно! Разрешены только цифры (1-4 символов)"; //ToolTip -> подсказка при наведении на курсор
                textBoxDays.BorderBrush = Brushes.DarkRed; //цвет фона задается цветом Brushes.DarkRed
                isCorrect = false;
            }
            else
            {
                textBoxDays.ToolTip = null;
                textBoxDays.BorderBrush = brushDefault;
            }
            if (isCorrect)
            {
                textBoxDays.ToolTip = null;
                textBoxDays.BorderBrush = Brushes.Transparent; //Transparent = прозрачный
                if (isAdmin)
                {
                    MessageBoxResult result = MessageBox.Show("Вы являетесь администратором. Вы действительно хотите арендовать этот самолет?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        int totalPrice = int.Parse(days) * currentPlane.price;
                        string formattedTotalPrice = totalPrice.ToString("N0");
                        MessageBoxResult result2 = MessageBox.Show($"Итого: ${formattedTotalPrice} за аренду этого самолета. Продолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (result2 == MessageBoxResult.Yes)
                        {
                            var currentUser = users.FirstOrDefault(user => user.name == currentUserName);
                            Bill bill = new Bill(currentUser.id, currentPlane.id, int.Parse(days), totalPrice, DateTime.Now, true);
                            bills = db.Bills.ToList();
                            if (bills.Any(b => b.planeId == currentPlane.id && b.isRentNow))
                            {
                                MessageBox.Show("К сожалению, данный самолет недоступен для аренды!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            db.Bills.Add(bill);
                            db.SaveChanges();
                            var wordApp = new Word.Application();
                            wordApp.Visible = true; //запустить Word
                            Word.Document document = wordApp.Documents.Add(); //добавить документ
                            Word.Paragraph text = document.Content.Paragraphs.Add(); //добавить абзац
                            text.Range.Text = $"ЗАЯВКА НА АРЕНДУ № {bill.id}";
                            text.Range.Font.Name = "Calibri";
                            text.Range.Font.Size = 18;
                            text.Range.Font.Bold = 1; //жирный
                            text.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1); //сдвиг текста по горизонтальной линейке
                            text.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter; //выравнивание по центру
                            text.Range.InsertParagraphAfter(); //вставить абзац (последующий)
                            //
                            Word.Paragraph text2 = document.Content.Paragraphs.Add(); //добавить абзац
                            string fixedTotalPrice = bill.totalPrice.ToString("N0");
                            text2.Range.Text = $"Сообщаем об успешном оформлении заявки на аренду самолета {currentPlane.name} на {bill.days} дн. – итоговая стоимость {fixedTotalPrice} USD (курс на {bill.date}).";
                            text2.Range.Font.Name = "Calibri";
                            text2.Range.Font.Size = 12;
                            text2.Range.Font.Bold = 0;
                            text2.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
                            text2.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            text2.Range.InsertParagraphAfter();
                            //
                            Word.Paragraph text3 = document.Content.Paragraphs.Add(); //добавить абзац
                            text3.Range.Text = "Подробности аренды";
                            text3.Range.Font.Name = "Calibri";
                            text3.Range.Font.Size = 16;
                            text3.Range.Font.Bold = 1; //жирный
                            text3.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
                            text3.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            text3.Range.InsertParagraphAfter();
                            //
                            Word.Table table = document.Tables.Add(document.Bookmarks["\\endofdoc"].Range, 11, 2); //добавить таблицу + document.Bookmarks[...].Range -> куда вставить + строки + столбцы
                            table.Range.Font.Name = "Calibri";
                            table.Range.Font.Size = 9;
                            table.Range.Font.Bold = 0;
                            table.Borders.Enable = 1; //есть рамки
                            table.Cell(1, 1).Range.Text = "Лицо, совершившее аренду";
                            table.Cell(1, 2).Range.Text = currentUser.name;
                            table.Cell(2, 1).Range.Text = "Роль лица в BPR";
                            table.Cell(2, 2).Range.Text = currentUser.role;
                            table.Cell(3, 1).Range.Text = "Название самолета";
                            table.Cell(3, 2).Range.Text = currentPlane.name;
                            table.Cell(4, 1).Range.Text = "Год выпуска";
                            table.Cell(4, 2).Range.Text = currentPlane.year.ToString();
                            table.Cell(5, 1).Range.Text = "Производитель";
                            table.Cell(5, 2).Range.Text = currentPlane.maker;
                            table.Cell(6, 1).Range.Text = "Регистрационный номер";
                            table.Cell(6, 2).Range.Text = currentPlane.regnum;
                            table.Cell(7, 1).Range.Text = "Страна регистрации";
                            table.Cell(7, 2).Range.Text = currentPlane.country;
                            table.Cell(8, 1).Range.Text = "Тип";
                            table.Cell(8, 2).Range.Text = currentPlane.type;
                            table.Cell(9, 1).Range.Text = "Категория";
                            table.Cell(9, 2).Range.Text = currentPlane.category;
                            table.Cell(10, 1).Range.Text = "Общий налет (км)";
                            string fixedTotalFly = currentPlane.totalFly.ToString("N0");
                            table.Cell(10, 2).Range.Text = fixedTotalFly;
                            table.Cell(11, 1).Range.Text = "Описание";
                            table.Cell(11, 2).Range.Text = currentPlane.description;
                            //
                            Word.Paragraph emptyParagraph = document.Content.Paragraphs.Add();
                            emptyParagraph.Range.InsertParagraphAfter();
                            emptyParagraph.Range.InsertBreak(Word.WdBreakType.wdPageBreak); //сделать разрыв страницы
                            //
                            Word.Paragraph text4 = document.Content.Paragraphs.Add(); //добавить абзац
                            text4.Range.Text = "Фотографии самолета";
                            text4.Range.Font.Name = "Calibri";
                            text4.Range.Font.Size = 16;
                            text4.Range.Font.Bold = 1; //жирный
                            text4.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
                            text4.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            text4.Range.InsertParagraphAfter();
                            //
                            byte[] photoNeedBytes = Convert.FromBase64String(currentPlane.photoNeed);
                            string tempPhotoNeed = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid() + ".png"); //создать путь к TEMP папке ПК и с уникальным именем (NewGuid()), а Combine объединяет части пути
                            File.WriteAllBytes(tempPhotoNeed, photoNeedBytes);
                            Word.Paragraph image = document.Content.Paragraphs.Add();
                            image.Range.InlineShapes.AddPicture(tempPhotoNeed); //добавить картинку
                            image.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            image.Range.InsertParagraphAfter();
                            File.Delete(tempPhotoNeed); //удалить временную картинку с ПК
                            //
                            if (currentPlane.photo1 != null)
                            {
                                byte[] photo1Bytes = Convert.FromBase64String(currentPlane.photo1);
                                string tempPhoto1 = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid() + ".png"); //создать путь к TEMP папке ПК и с уникальным именем (NewGuid()), а Combine объединяет части пути
                                File.WriteAllBytes(tempPhoto1, photo1Bytes);
                                Word.Paragraph image2 = document.Content.Paragraphs.Add();
                                image2.Range.InlineShapes.AddPicture(tempPhoto1); //добавить картинку
                                image2.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                image2.Range.InsertParagraphAfter();
                                File.Delete(tempPhoto1); //удалить временную картинку с ПК
                            }
                            //
                            if (currentPlane.photo2 != null)
                            {
                                byte[] photo2Bytes = Convert.FromBase64String(currentPlane.photo2);
                                string tempPhoto2 = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid() + ".png"); //создать путь к TEMP папке ПК и с уникальным именем (NewGuid()), а Combine объединяет части пути
                                File.WriteAllBytes(tempPhoto2, photo2Bytes);
                                Word.Paragraph image3 = document.Content.Paragraphs.Add();
                                image3.Range.InlineShapes.AddPicture(tempPhoto2); //добавить картинку
                                image3.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                image3.Range.InsertParagraphAfter();
                                File.Delete(tempPhoto2); //удалить временную картинку с ПК
                            }
                            //
                            Word.Paragraph text5 = document.Content.Paragraphs.Add(); //добавить абзац
                            text5.Range.Text = "Просим вас прибыть по адресу ул. Аэровокзальная, 148, г. Минск для подтверждения бронирования, осмотра самолета и оплаты.";
                            text5.Range.Font.Name = "Calibri";
                            text5.Range.Font.Size = 12;
                            text5.Range.Font.Bold = 0;
                            text5.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
                            text5.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            text5.Range.InsertParagraphAfter();
                            //
                            Word.Paragraph text6 = document.Content.Paragraphs.Add(); //добавить абзац
                            text6.Range.Text = "При утере этого документа свяжитесь с нами по электронной почте: belarusplanerentinfo@gmail.com.";
                            text6.Range.Font.Name = "Calibri";
                            text6.Range.Font.Size = 12;
                            text6.Range.Font.Bold = 0;
                            text6.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
                            text6.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            text6.Range.InsertParagraphAfter();
                            //
                            Word.Paragraph text7 = document.Content.Paragraphs.Add(); //добавить абзац
                            text7.Range.Text = "С уважением, Belarus Plane Rent ✈";
                            text7.Range.Font.Name = "Calibri";
                            text7.Range.Font.Size = 12;
                            text7.Range.Font.Bold = 0;
                            text7.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
                            text7.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                            text7.Range.InsertParagraphAfter();
                            isClosedNormal = true;
                            this.Close();
                        }
                        else
                        {
                            this.Close();
                        }
                    }
                    else
                    {
                        this.Close();
                    }
                }
                else
                {
                    int totalPrice = int.Parse(days) * currentPlane.price;
                    string formattedTotalPrice = totalPrice.ToString("N0");
                    MessageBoxResult result2 = MessageBox.Show($"Итого: ${formattedTotalPrice} за аренду этого самолета. Продолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result2 == MessageBoxResult.Yes)
                    {
                        var currentUser = users.FirstOrDefault(user => user.name == currentUserName);
                        Bill bill = new Bill(currentUser.id, currentPlane.id, int.Parse(days), totalPrice, DateTime.Now, true);
                        bills = db.Bills.ToList();
                        if (bills.Any(b => b.planeId == currentPlane.id && b.isRentNow))
                        {
                            MessageBox.Show("К сожалению, данный самолет недоступен для аренды!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        db.Bills.Add(bill);
                        db.SaveChanges();
                        var wordApp = new Word.Application();
                        wordApp.Visible = true; //запустить Word
                        Word.Document document = wordApp.Documents.Add(); //добавить документ
                        Word.Paragraph text = document.Content.Paragraphs.Add(); //добавить абзац
                        text.Range.Text = $"ЗАЯВКА НА АРЕНДУ № {bill.id}";
                        text.Range.Font.Name = "Calibri";
                        text.Range.Font.Size = 18;
                        text.Range.Font.Bold = 1; //жирный
                        text.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1); //сдвиг текста по горизонтальной линейке
                        text.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter; //выравнивание по центру
                        text.Range.InsertParagraphAfter(); //вставить абзац (последующий)
                        //
                        Word.Paragraph text2 = document.Content.Paragraphs.Add(); //добавить абзац
                        string fixedTotalPrice = bill.totalPrice.ToString("N0");
                        text2.Range.Text = $"Сообщаем об успешном оформлении заявки на аренду самолета {currentPlane.name} на {bill.days} дн. – итоговая стоимость {fixedTotalPrice} USD (курс на {bill.date}).";
                        text2.Range.Font.Name = "Calibri";
                        text2.Range.Font.Size = 12;
                        text2.Range.Font.Bold = 0;
                        text2.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
                        text2.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                        text2.Range.InsertParagraphAfter();
                        //
                        Word.Paragraph text3 = document.Content.Paragraphs.Add(); //добавить абзац
                        text3.Range.Text = "Подробности аренды";
                        text3.Range.Font.Name = "Calibri";
                        text3.Range.Font.Size = 16;
                        text3.Range.Font.Bold = 1; //жирный
                        text3.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
                        text3.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        text3.Range.InsertParagraphAfter();
                        //
                        Word.Table table = document.Tables.Add(document.Bookmarks["\\endofdoc"].Range, 11, 2); //добавить таблицу + document.Bookmarks[...].Range -> куда вставить + строки + столбцы
                        table.Range.Font.Name = "Calibri";
                        table.Range.Font.Size = 9;
                        table.Range.Font.Bold = 0;
                        table.Borders.Enable = 1; //есть рамки
                        table.Cell(1, 1).Range.Text = "Лицо, совершившее аренду";
                        table.Cell(1, 2).Range.Text = currentUser.name;
                        table.Cell(2, 1).Range.Text = "Роль лица в BPR";
                        table.Cell(2, 2).Range.Text = currentUser.role;
                        table.Cell(3, 1).Range.Text = "Название самолета";
                        table.Cell(3, 2).Range.Text = currentPlane.name;
                        table.Cell(4, 1).Range.Text = "Год выпуска";
                        table.Cell(4, 2).Range.Text = currentPlane.year.ToString();
                        table.Cell(5, 1).Range.Text = "Производитель";
                        table.Cell(5, 2).Range.Text = currentPlane.maker;
                        table.Cell(6, 1).Range.Text = "Регистрационный номер";
                        table.Cell(6, 2).Range.Text = currentPlane.regnum;
                        table.Cell(7, 1).Range.Text = "Страна регистрации";
                        table.Cell(7, 2).Range.Text = currentPlane.country;
                        table.Cell(8, 1).Range.Text = "Тип";
                        table.Cell(8, 2).Range.Text = currentPlane.type;
                        table.Cell(9, 1).Range.Text = "Категория";
                        table.Cell(9, 2).Range.Text = currentPlane.category;
                        table.Cell(10, 1).Range.Text = "Общий налет (км)";
                        string fixedTotalFly = currentPlane.totalFly.ToString("N0");
                        table.Cell(10, 2).Range.Text = fixedTotalFly;
                        table.Cell(11, 1).Range.Text = "Описание";
                        table.Cell(11, 2).Range.Text = currentPlane.description;
                        //
                        Word.Paragraph emptyParagraph = document.Content.Paragraphs.Add();
                        emptyParagraph.Range.InsertParagraphAfter();
                        emptyParagraph.Range.InsertBreak(Word.WdBreakType.wdPageBreak); //сделать разрыв страницы
                        //
                        Word.Paragraph text4 = document.Content.Paragraphs.Add(); //добавить абзац
                        text4.Range.Text = "Фотографии самолета";
                        text4.Range.Font.Name = "Calibri";
                        text4.Range.Font.Size = 16;
                        text4.Range.Font.Bold = 1; //жирный
                        text4.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
                        text4.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        text4.Range.InsertParagraphAfter();
                        //
                        byte[] photoNeedBytes = Convert.FromBase64String(currentPlane.photoNeed);
                        string tempPhotoNeed = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid() + ".png"); //создать путь к TEMP папке ПК и с уникальным именем (NewGuid()), а Combine объединяет части пути
                        File.WriteAllBytes(tempPhotoNeed, photoNeedBytes);
                        Word.Paragraph image = document.Content.Paragraphs.Add();
                        image.Range.InlineShapes.AddPicture(tempPhotoNeed); //добавить картинку
                        image.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        image.Range.InsertParagraphAfter();
                        File.Delete(tempPhotoNeed); //удалить временную картинку с ПК
                        //
                        if (currentPlane.photo1 != null)
                        {
                            byte[] photo1Bytes = Convert.FromBase64String(currentPlane.photo1);
                            string tempPhoto1 = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid() + ".png"); //создать путь к TEMP папке ПК и с уникальным именем (NewGuid()), а Combine объединяет части пути
                            File.WriteAllBytes(tempPhoto1, photo1Bytes);
                            Word.Paragraph image2 = document.Content.Paragraphs.Add();
                            image2.Range.InlineShapes.AddPicture(tempPhoto1); //добавить картинку
                            image2.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            image2.Range.InsertParagraphAfter();
                            File.Delete(tempPhoto1); //удалить временную картинку с ПК
                        }
                        //
                        if (currentPlane.photo2 != null)
                        {
                            byte[] photo2Bytes = Convert.FromBase64String(currentPlane.photo2);
                            string tempPhoto2 = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid() + ".png"); //создать путь к TEMP папке ПК и с уникальным именем (NewGuid()), а Combine объединяет части пути
                            File.WriteAllBytes(tempPhoto2, photo2Bytes);
                            Word.Paragraph image3 = document.Content.Paragraphs.Add();
                            image3.Range.InlineShapes.AddPicture(tempPhoto2); //добавить картинку
                            image3.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            image3.Range.InsertParagraphAfter();
                            File.Delete(tempPhoto2); //удалить временную картинку с ПК
                        }
                        //
                        Word.Paragraph text5 = document.Content.Paragraphs.Add(); //добавить абзац
                        text5.Range.Text = "Просим вас прибыть по адресу ул. Аэровокзальная, 148, г. Минск для подтверждения бронирования, осмотра самолета и оплаты.";
                        text5.Range.Font.Name = "Calibri";
                        text5.Range.Font.Size = 12;
                        text5.Range.Font.Bold = 0;
                        text5.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
                        text5.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                        text5.Range.InsertParagraphAfter();
                        //
                        Word.Paragraph text6 = document.Content.Paragraphs.Add(); //добавить абзац
                        text6.Range.Text = "При утере этого документа свяжитесь с нами по электронной почте: belarusplanerentinfo@gmail.com.";
                        text6.Range.Font.Name = "Calibri";
                        text6.Range.Font.Size = 12;
                        text6.Range.Font.Bold = 0;
                        text6.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
                        text6.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                        text6.Range.InsertParagraphAfter();
                        //
                        Word.Paragraph text7 = document.Content.Paragraphs.Add(); //добавить абзац
                        text7.Range.Text = "С уважением, Belarus Plane Rent ✈";
                        text7.Range.Font.Name = "Calibri";
                        text7.Range.Font.Size = 12;
                        text7.Range.Font.Bold = 0;
                        text7.Format.FirstLineIndent = wordApp.CentimetersToPoints(-1);
                        text7.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                        text7.Range.InsertParagraphAfter();
                        isClosedNormal = true;
                        this.Close();
                    }
                    else
                    {
                        this.Close();
                    }
                }
            }
        }
    }
}