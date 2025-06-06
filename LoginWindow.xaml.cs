using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        ApplicationContext db; 
        List<User> users;
        SolidColorBrush brushDefault;
        public LoginWindow()
        {
            InitializeComponent();
            db = MainWindow.db;
            brushDefault = textBoxName.BorderBrush as SolidColorBrush; //для приведения в изначальное положение textBoxы
        }
        private void AboutUs_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Компания: BPR (Belarus Plane Rent)\nАдрес: ул. Аэровокзальная, 148, Минск, Беларусь\nЭлектронная почта: belarusplanerentinfo@gmail.com\nМы предоставляем услуги аренды самолетов по всей Беларуси ✈", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow secondWindow = new MainWindow();
            secondWindow.Show();
            this.Close();
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            users = db.Users.ToList(); //вытянуть все кортежи с таблицы БД Users и суем их в список
            string name = textBoxName.Text;
            string pass = passBox.Password;
            string passFromUser = null;
            bool isCorrect = true;
            bool isUserFound = false;
            /*foreach (User user in users)
            {
                if (user.name == name)
                {
                    isUserFound = true;
                    passFromUser = user.pass;
                    break;
                }
            }*/
            var foundUser = users.FirstOrDefault(user => user.name == name);
            if (foundUser != null)
            {
                isUserFound = true;
                passFromUser = foundUser.pass;
            }
            if (!isUserFound)
            {
                textBoxName.ToolTip = "Данного пользователя не существует!"; //ToolTip -> подсказка при наведении на курсор
                textBoxName.BorderBrush = Brushes.DarkRed; //цвет фона задается цветом Brushes.DarkRed
                isCorrect = false;
            }
            else
            {
                textBoxName.ToolTip = null;
                textBoxName.BorderBrush = brushDefault;
            }
            if (passFromUser == null)
            {
                passBox.ToolTip = null;
                passBox.BorderBrush = Brushes.DarkRed; //цвет фона задается цветом Brushes.DarkRed
                isCorrect = false;
            }
            else if (pass != passFromUser)
            {
                passBox.ToolTip = "Неверный пароль!"; //ToolTip -> подсказка при наведении на курсор
                passBox.BorderBrush = Brushes.DarkRed; //цвет фона задается цветом Brushes.DarkRed
                isCorrect = false;
            }
            else
            {
                passBox.ToolTip = null;
                passBox.BorderBrush = brushDefault;
            }
            if (isCorrect)
            {
                textBoxName.ToolTip = null;
                textBoxName.BorderBrush = Brushes.Transparent; //Transparent = прозрачный
                passBox.ToolTip = null;
                passBox.BorderBrush = Brushes.Transparent;
                MessageBox.Show("Успешный вход", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                WorkWindow workWindow = new WorkWindow(name);
                workWindow.Show();
                this.Close();
            }
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("help.chm");
        }
    }
}