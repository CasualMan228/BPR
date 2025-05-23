using Microsoft.Win32;
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

namespace BPR
{
    /// <summary>
    /// Логика взаимодействия для ChangePlaneWindow.xaml
    /// </summary>
    public partial class ChangePlaneWindow : Window
    {
        ApplicationContext db;
        List<Plane> planes;
        List<Bill> bills;
        public Bill currentBill;
        SolidColorBrush brushDefault;
        string patternENGWithoutSpaces = @"^[a-zA-Z0-9]+$";
        string patternENGWithSpaces = @"^[a-zA-Z0-9]+(?:\s+[a-zA-Z0-9]+)*$";
        string patternRUSWithoutSpaces = @"^[а-яА-ЯёЁa-zA-Z0-9]+$";
        string patternRUSWithSpaces = @"^[а-яА-ЯёЁa-zA-Z0-9]+(?:\s+[а-яА-ЯёЁa-zA-Z0-9]+)*$";
        string patternDigit = @"^\d+$";
        string patternRegnum = @"^[A-Za-z0-9]+(?:-[A-Za-z0-9]+)?$";
        string planePhotoNeed;
        string planePhoto1;
        string planePhoto2;
        string planeType;
        string planeCategory;
        string fixedRegnum; //зафиксированный регистрационный номер самолета, который мы и передали сюда (необходимо для изменения именно этого конкретного самолета, т.к. регистрационный номер может поменяться)
        //^ -> начало строки; $ -> конец строки; + -> один и более; * -> ноль и более; (?:) -> возможны повторения; \s -> символ пробела; \d -> любая цифра
        public ChangePlaneWindow(Plane plane)
        {
            InitializeComponent();
            db = MainWindow.db;
            brushDefault = textBoxPlaneName.BorderBrush as SolidColorBrush; //для приведения в изначальное положение textBoxы
            fixedRegnum = plane.regnum;
            bills = db.Bills.ToList(); //вытянуть все кортежи с таблицы БД Users и суем их в список
            if (!bills.Any(bill => bill.planeId == plane.id && bill.isRentNow))
            {
                cancelRentPanel.IsEnabled = false;
                cancelRentPanel.Visibility = Visibility.Hidden;
            }
            else
            {
                currentBill = bills.First(bill => bill.planeId == plane.id && bill.isRentNow);
                cancelRentPanel.Tag = currentBill;
            }
            textBoxPlaneName.Text = plane.name;
            textBoxPlaneYear.Text = plane.year.ToString();
            textBoxPlaneMaker.Text = plane.maker;
            textBoxPlaneRegnum.Text = plane.regnum;
            textBoxPlaneCountry.Text = plane.country;
            if (plane.type == "Jet")
            {
                typeComboBox.SelectedIndex = 0;
            }
            else if (plane.type == "Turboprop")
            {
                typeComboBox.SelectedIndex = 1;
            }
            else if (plane.type == "Glider")
            {
                typeComboBox.SelectedIndex = 2;
            }
            if (plane.category == "Passenger")
            {
                categoryComboBox.SelectedIndex = 0;
            }
            else if (plane.category == "Cargo")
            {
                categoryComboBox.SelectedIndex = 1;
            }
            else if (plane.category == "Study")
            {
                categoryComboBox.SelectedIndex = 2;
            }
            else if (plane.category == "Military")
            {
                categoryComboBox.SelectedIndex = 3;
            }
            else if (plane.category == "Sport")
            {
                categoryComboBox.SelectedIndex = 4;
            }
            else if (plane.category == "Business")
            {
                categoryComboBox.SelectedIndex = 5;
            }
            textBoxPlaneDescription.Text = plane.description;
            textBoxPlaneTotalFly.Text = plane.totalFly.ToString();
            textBoxPlanePrice.Text = plane.price.ToString();
            planePhotoNeed = plane.photoNeed;
            if (plane.photo2 != null)
            {
                photo1ButtonCancel.IsEnabled = true;
                photo2Button.IsEnabled = true;
                photo2ButtonCancel.IsEnabled = true;
                planePhoto2 = plane.photo2;
                planePhoto1 = plane.photo1;
            }
            else if (plane.photo1 != null)
            {
                photo1ButtonCancel.IsEnabled = true;
                photo2Button.IsEnabled = true;
                planePhoto1 = plane.photo1;
            }
        }

        private void PhotoNeedClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Title = "Выбор изображения",
                    Filter = "Image Files|*.jpg;*.jpeg;*.png",
                    Multiselect = false
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                    planePhotoNeed = Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Ошибка! {exception}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Photo1Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Title = "Выбор изображения",
                    Filter = "Image Files|*.jpg;*.jpeg;*.png",
                    Multiselect = false
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                    planePhoto1 = Convert.ToBase64String(imageBytes);
                    photo2Button.IsEnabled = true;
                    photo1ButtonCancel.IsEnabled = true;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Ошибка! {exception}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Photo1ClickCancel(object sender, RoutedEventArgs e)
        {
            planePhoto1 = null;
            planePhoto2 = null;
            photo2Button.IsEnabled = false;
            photo1ButtonCancel.IsEnabled = false;
            photo2ButtonCancel.IsEnabled = false;
        }

        private void Photo2Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Title = "Выбор изображения",
                    Filter = "Image Files|*.jpg;*.jpeg;*.png",
                    Multiselect = false
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                    planePhoto2 = Convert.ToBase64String(imageBytes);
                    photo2ButtonCancel.IsEnabled = true;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Ошибка! {exception}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Photo2ClickCancel(object sender, RoutedEventArgs e)
        {
            planePhoto2 = null;
            photo2ButtonCancel.IsEnabled = false;
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            planes = db.Planes.ToList(); //вытянуть все кортежи с таблицы БД Users и суем их в список
            bills = db.Bills.ToList();
            string planeName = textBoxPlaneName.Text;
            string planeYear = textBoxPlaneYear.Text;
            string planeMaker = textBoxPlaneMaker.Text;
            string planeRegnum = textBoxPlaneRegnum.Text;
            string planeCountry = textBoxPlaneCountry.Text;
            string planeDescription = textBoxPlaneDescription.Text;
            string planeTotalFly = textBoxPlaneTotalFly.Text;
            string planePrice = textBoxPlanePrice.Text;
            if (typeComboBox.SelectedIndex == 0)
            {
                planeType = "Jet";
            }
            else if (typeComboBox.SelectedIndex == 1)
            {
                planeType = "Turboprop";
            }
            else if (typeComboBox.SelectedIndex == 2)
            {
                planeType = "Glider";
            }
            if (categoryComboBox.SelectedIndex == 0)
            {
                planeCategory = "Passenger";
            }
            else if (categoryComboBox.SelectedIndex == 1)
            {
                planeCategory = "Cargo";
            }
            else if (categoryComboBox.SelectedIndex == 2)
            {
                planeCategory = "Study";
            }
            else if (categoryComboBox.SelectedIndex == 3)
            {
                planeCategory = "Military";
            }
            else if (categoryComboBox.SelectedIndex == 4)
            {
                planeCategory = "Sport";
            }
            else if (categoryComboBox.SelectedIndex == 5)
            {
                planeCategory = "Business";
            }
            bool isCorrect = true;
            //
            if (planeName == "")
            {
                textBoxPlaneName.ToolTip = "Данное поле пустое! (4-50 символов)";
                textBoxPlaneName.BorderBrush = Brushes.DarkRed;
                isCorrect = false;
            }
            else if (planeName.Length < 4 || planeName.Length > 50)
            {
                textBoxPlaneName.ToolTip = "Данное поле введено некорректно! (4-50 символов)"; //ToolTip -> подсказка при наведении на курсор
                textBoxPlaneName.BorderBrush = Brushes.DarkRed; //цвет фона задается цветом Brushes.DarkRed
                isCorrect = false;
            }
            else
            {
                textBoxPlaneName.ToolTip = null;
                textBoxPlaneName.BorderBrush = brushDefault;
            }
            if (planeYear == "")
            {
                textBoxPlaneYear.ToolTip = "Данное поле пустое! Разрешены только цифры (4 символа)";
                textBoxPlaneYear.BorderBrush = Brushes.DarkRed;
                isCorrect = false;
            }
            else if (planeYear.Length != 4 || !Regex.IsMatch(planeYear, patternDigit))
            {
                textBoxPlaneYear.ToolTip = "Данное поле введено некорректно! Разрешены только цифры (4 символа)"; //ToolTip -> подсказка при наведении на курсор
                textBoxPlaneYear.BorderBrush = Brushes.DarkRed; //цвет фона задается цветом Brushes.DarkRed
                isCorrect = false;
            }
            else
            {
                textBoxPlaneYear.ToolTip = null;
                textBoxPlaneYear.BorderBrush = brushDefault;
            }
            if (planeMaker == "")
            {
                textBoxPlaneMaker.ToolTip = "Данное поле пустое! (4-75 символов)";
                textBoxPlaneMaker.BorderBrush = Brushes.DarkRed;
                isCorrect = false;
            }
            else if (planeMaker.Length < 4 || planeMaker.Length > 75)
            {
                textBoxPlaneMaker.ToolTip = "Данное поле введено некорректно! (4-75 символов)"; //ToolTip -> подсказка при наведении на курсор
                textBoxPlaneMaker.BorderBrush = Brushes.DarkRed; //цвет фона задается цветом Brushes.DarkRed
                isCorrect = false;
            }
            else
            {
                textBoxPlaneMaker.ToolTip = null;
                textBoxPlaneMaker.BorderBrush = brushDefault;
            }
            if (planeRegnum == "")
            {
                textBoxPlaneRegnum.ToolTip = "Данное поле пустое! Разрешены только латиница и цифры и тире (4-50 символов)";
                textBoxPlaneRegnum.BorderBrush = Brushes.DarkRed;
                isCorrect = false;
            }
            else if (planeRegnum.Length < 4 || planeRegnum.Length > 50 || !Regex.IsMatch(planeRegnum, patternRegnum))
            {
                textBoxPlaneRegnum.ToolTip = "Данное поле введено некорректно! Разрешены только латиница и цифры и тире (4-50 символов)"; //ToolTip -> подсказка при наведении на курсор
                textBoxPlaneRegnum.BorderBrush = Brushes.DarkRed; //цвет фона задается цветом Brushes.DarkRed
                isCorrect = false;
            }
            else if (db.Planes.Any(p => p.regnum == planeRegnum && p.regnum != fixedRegnum))
            {
                textBoxPlaneRegnum.ToolTip = "Данное поле введено некорректно! Самолет с таким регистрационным номером существует";
                textBoxPlaneRegnum.BorderBrush = Brushes.DarkRed;
                isCorrect = false;
            }
            else 
            {
                textBoxPlaneRegnum.ToolTip = null;
                textBoxPlaneRegnum.BorderBrush = brushDefault;
            }
            if (planeCountry == "")
            {
                textBoxPlaneCountry.ToolTip = "Данное поле пустое! Разрешены только латиница и цифры и пробелы между ними (3-50 символов)";
                textBoxPlaneCountry.BorderBrush = Brushes.DarkRed;
                isCorrect = false;
            }
            else if (planeCountry.Length < 3 || planeCountry.Length > 50 || !Regex.IsMatch(planeCountry, patternENGWithSpaces))
            {
                textBoxPlaneCountry.ToolTip = "Данное поле введено некорректно! Разрешены только латиница и цифры и пробелы между ними (3-50 символов)"; //ToolTip -> подсказка при наведении на курсор
                textBoxPlaneCountry.BorderBrush = Brushes.DarkRed; //цвет фона задается цветом Brushes.DarkRed
                isCorrect = false;
            }
            else
            {
                textBoxPlaneCountry.ToolTip = null;
                textBoxPlaneCountry.BorderBrush = brushDefault;
            }
            if (planeDescription == "")
            {
                textBoxPlaneDescription.ToolTip = "Данное поле пустое! (4-1000 символов)";
                textBoxPlaneDescription.BorderBrush = Brushes.DarkRed;
                isCorrect = false;
            }
            else if (planeDescription.Length < 4 || planeDescription.Length > 1000)
            {
                textBoxPlaneDescription.ToolTip = "Данное поле введено некорректно! (4-1000 символов)"; //ToolTip -> подсказка при наведении на курсор
                textBoxPlaneDescription.BorderBrush = Brushes.DarkRed; //цвет фона задается цветом Brushes.DarkRed
                isCorrect = false;
            }
            else
            {
                textBoxPlaneDescription.ToolTip = null;
                textBoxPlaneDescription.BorderBrush = brushDefault;
            }
            if (planeTotalFly == "")
            {
                textBoxPlaneTotalFly.ToolTip = "Данное поле пустое! Разрешены только цифры (1-15 символов)";
                textBoxPlaneTotalFly.BorderBrush = Brushes.DarkRed;
                isCorrect = false;
            }
            else if (planeTotalFly.Length < 1 || planeTotalFly.Length > 15 || !Regex.IsMatch(planeTotalFly, patternDigit))
            {
                textBoxPlaneTotalFly.ToolTip = "Данное поле введено некорректно! Разрешены только цифры (1-15 символов)"; //ToolTip -> подсказка при наведении на курсор
                textBoxPlaneTotalFly.BorderBrush = Brushes.DarkRed; //цвет фона задается цветом Brushes.DarkRed
                isCorrect = false;
            }
            else
            {
                textBoxPlaneTotalFly.ToolTip = null;
                textBoxPlaneTotalFly.BorderBrush = brushDefault;
            }
            if (planePrice == "")
            {
                textBoxPlanePrice.ToolTip = "Данное поле пустое! Разрешены только цифры (1-15 символов)";
                textBoxPlanePrice.BorderBrush = Brushes.DarkRed;
                isCorrect = false;
            }
            else if (planePrice.Length < 1 || planePrice.Length > 15 || !Regex.IsMatch(planePrice, patternDigit))
            {
                textBoxPlanePrice.ToolTip = "Данное поле введено некорректно! Разрешены только цифры (1-15 символов)"; //ToolTip -> подсказка при наведении на курсор
                textBoxPlanePrice.BorderBrush = Brushes.DarkRed; //цвет фона задается цветом Brushes.DarkRed
                isCorrect = false;
            }
            else
            {
                textBoxPlanePrice.ToolTip = null;
                textBoxPlanePrice.BorderBrush = brushDefault;
            }
            if (isCorrect)
            {
                textBoxPlaneName.ToolTip = null;
                textBoxPlaneName.BorderBrush = Brushes.Transparent; //Transparent = прозрачный
                textBoxPlaneYear.ToolTip = null;
                textBoxPlaneYear.BorderBrush = Brushes.Transparent;
                textBoxPlaneMaker.ToolTip = null;
                textBoxPlaneMaker.BorderBrush = Brushes.Transparent;
                textBoxPlaneRegnum.ToolTip = null;
                textBoxPlaneRegnum.BorderBrush = Brushes.Transparent;
                textBoxPlaneCountry.ToolTip = null;
                textBoxPlaneCountry.BorderBrush = Brushes.Transparent;
                textBoxPlaneDescription.ToolTip = null;
                textBoxPlaneDescription.BorderBrush = Brushes.Transparent;
                textBoxPlaneTotalFly.ToolTip = null;
                textBoxPlaneTotalFly.BorderBrush = Brushes.Transparent;
                textBoxPlanePrice.ToolTip = null;
                textBoxPlanePrice.BorderBrush = Brushes.Transparent;
                var foundPlaneDB = db.Planes.FirstOrDefault(plane => plane.regnum == fixedRegnum);
                if (bills.Any(bill => bill.planeId == foundPlaneDB.id && bill.isRentNow))
                {
                    MessageBox.Show("К сожалению, данный самолет недоступен для изменения, так как он в данный момент арендуется!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                foundPlaneDB.name = planeName;
                foundPlaneDB.year = int.Parse(planeYear);
                foundPlaneDB.maker = planeMaker;
                foundPlaneDB.regnum = planeRegnum;
                foundPlaneDB.country = planeCountry;
                foundPlaneDB.type = planeType;
                foundPlaneDB.category = planeCategory;
                foundPlaneDB.description = planeDescription;
                foundPlaneDB.totalFly = int.Parse(planeTotalFly);
                foundPlaneDB.price = int.Parse(planePrice);
                foundPlaneDB.photoNeed = planePhotoNeed;
                foundPlaneDB.photo1 = planePhoto1;
                foundPlaneDB.photo2 = planePhoto2;
                db.SaveChanges(); //сохраняем изменения с бд
                MessageBox.Show("Самолет успешно изменен", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }

        private void CancelRent_Click(object sender, RoutedEventArgs e)
        {
            bills = db.Bills.ToList();
            MessageBoxResult result = MessageBox.Show("Этот самолет уже забронирован. Вы уверены, что хотите отменить предварительное бронирование?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                var currentBill = cancelRentPanel.Tag as Bill;
                currentBill.isRentNow = false;
                db.SaveChanges();
                bills = db.Bills.ToList();
                cancelRentPanel.IsEnabled = false;
                cancelRentPanel.Visibility = Visibility.Hidden;
                MessageBox.Show("Бронирование успешно отменено", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                this.Close();
            }
        }
    }
}