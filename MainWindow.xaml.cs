using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BPR 
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ApplicationContext db; //декларация для работы уже с БД
        string pattern = @"^[a-zA-Z0-9]+$"; //^начало строки +один и более символов $конец строки
        List<User> users;
        SolidColorBrush brushDefault;
        public MainWindow()
        {
            InitializeComponent();
            db = new ApplicationContext(); //инициализация для работы уже с БД (создание и использование сессии с БД)
            brushDefault = textBoxName.BorderBrush as SolidColorBrush; //для приведения в изначальное положение textBoxы
        }

        private void AboutUs_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Компания: BPR (Belarus Plane Rent)\nАдрес: ул. Аэровокзальная, 148, Минск, Беларусь\nЭлектронная почта: belarusplanerentinfo@gmail.com\nМы предоставляем услуги аренды самолетов по всей Беларуси ✈", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow secondWindow = new LoginWindow();
            secondWindow.Show();
            this.Close();
        }

        private void RegButtonClick(object sender, RoutedEventArgs e)
        {
            users = db.Users.ToList(); //вытянуть все кортежи с таблицы БД Users и суем их в список
            string name = textBoxName.Text;
            string pass = passBox.Password;
            string pass2 = passBox2.Password;
            string passAdmin = passBoxAdmin.Password;
            bool isCorrect = true;
            string role = "user";
            if (name == "")
            {
                textBoxName.ToolTip = "Данное поле пустое! Разрешены только латиница и цифры (4-30 символов)"; //ToolTip -> подсказка при наведении на курсор
                textBoxName.BorderBrush = Brushes.DarkRed; //цвет фона задается цветом Brushes.DarkRed
                isCorrect = false;
            }
            else if (name.Length < 4 || name.Length > 30 || !Regex.IsMatch(name, pattern))
            {
                textBoxName.ToolTip = "Данное поле введено некорректно! Разрешены только латиница и цифры (4-30 символов)"; //ToolTip -> подсказка при наведении на курсор
                textBoxName.BorderBrush = Brushes.DarkRed; //цвет фона задается цветом Brushes.DarkRed
                isCorrect = false;
            }
            else
            {
                textBoxName.ToolTip = null;
                textBoxName.BorderBrush = brushDefault;
            }
            foreach (User user in users)
            {
                if (user.name == name)
                {
                    textBoxName.ToolTip = "Данное поле введено некорректно! Данный пользователь уже существует"; //ToolTip -> подсказка при наведении на курсор
                    textBoxName.BorderBrush = Brushes.DarkRed; //цвет фона задается цветом Brushes.DarkRed
                    isCorrect = false;
                    break;
                }
            }
            if (pass == "")
            {
                passBox.ToolTip = "Данное поле пустое! Разрешены только латиница и цифры (7-30 символов)"; //ToolTip -> подсказка при наведении на курсор
                passBox.BorderBrush = Brushes.DarkRed; //цвет фона задается цветом Brushes.DarkRed
                isCorrect = false;
            }
            else if (pass.Length < 7 || pass.Length > 30 || !Regex.IsMatch(pass, pattern))
            {
                passBox.ToolTip = "Данное поле введено некорректно! Разрешены только латиница и цифры (7-30 символов)"; //ToolTip -> подсказка при наведении на курсор
                passBox.BorderBrush = Brushes.DarkRed; //цвет фона задается цветом Brushes.DarkRed
                isCorrect = false;
            }
            else
            {
                passBox.ToolTip = null;
                passBox.BorderBrush = brushDefault;
            }
            if (pass != pass2)
            {
                passBox2.ToolTip = "Пароли не совпадают!";
                passBox2.BorderBrush = Brushes.DarkRed;
                isCorrect = false;
            }
            else
            {
                passBox2.ToolTip = null;
                passBox2.BorderBrush = brushDefault;
            }
            if (passAdmin != "" && passAdmin != "boeing737belavia")
            {
                passBoxAdmin.ToolTip = "Неверный пароль! Оставьте поле пустым";
                passBoxAdmin.BorderBrush = Brushes.DarkRed;
                isCorrect = false;
            }
            else if (passAdmin == "boeing737belavia")
            {
                role = "admin";
                passBoxAdmin.ToolTip = null;
                passBoxAdmin.BorderBrush = brushDefault;
            }
            else
            {
                passBoxAdmin.ToolTip = null;
                passBoxAdmin.BorderBrush = brushDefault;
            }
            if (isCorrect)
            {
                textBoxName.ToolTip = null;
                textBoxName.BorderBrush = Brushes.Transparent; //Transparent = прозрачный
                passBox.ToolTip = null;
                passBox.BorderBrush = Brushes.Transparent;
                passBox2.ToolTip = null;
                passBox2.BorderBrush = Brushes.Transparent;
                passBoxAdmin.ToolTip = null;
                passBoxAdmin.BorderBrush = Brushes.Transparent;
                User user = new User(name, role, pass); //создаем объект модели, который потом станет кортежом в бд
                db.Users.Add(user); //добавляем данный объект модели (класс-модель USER, который там уже юзает класс ApplicationContext и коннектится к БД)
                db.SaveChanges(); //сохраняем изменения с бд
                MessageBox.Show("Успешная регистрация", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                WorkWindow workWindow = new WorkWindow(name, role);
                workWindow.Show();
                this.Close();
            }
        }
    }
}