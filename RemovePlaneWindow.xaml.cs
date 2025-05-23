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
using System.Windows.Shapes;

namespace BPR
{
    /// <summary>
    /// Логика взаимодействия для RemovePlaneWindow.xaml
    /// </summary>
    public partial class RemovePlaneWindow : Window
    {//В БУДУЩЕМ ПРИ УЛУЧШЕНИИ ЭТОГО ВСЕГО ЛУЧШЕ ЮЗАТЬ LINQ
        ApplicationContext db;
        List<Plane> planes;
        SolidColorBrush brushDefault;
        string patternRegnum = @"^[A-Za-z0-9]+(?:-[A-Za-z0-9]+)?$";
        //^ -> начало строки; $ -> конец строки; + -> один и более; * -> ноль и более; (?:) -> возможны повторения; \s -> символ пробела; \d -> любая цифра
        public RemovePlaneWindow()
        {
            InitializeComponent();
            db = MainWindow.db;
            brushDefault = textBoxPlaneRegnum.BorderBrush as SolidColorBrush; //для приведения в изначальное положение textBoxы
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            planes = db.Planes.ToList(); //вытянуть все кортежи с таблицы БД Users и суем их в список
            string planeRegnum = textBoxPlaneRegnum.Text;
            bool isCorrect = true;
            bool isFound = false;
            Plane planeToRemove = null;
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
            else
            {
                textBoxPlaneRegnum.ToolTip = null;
                textBoxPlaneRegnum.BorderBrush = brushDefault;
                foreach (Plane plane in planes)
                {
                    if (plane.regnum == planeRegnum)
                    {
                        planeToRemove = plane;
                        isFound = true;
                        break;
                    }
                }
            }
            if (!isFound && isCorrect)
            {
                textBoxPlaneRegnum.ToolTip = "Данное поле введено некорректно! Самолет с этим регистрационным номером не найден"; //ToolTip -> подсказка при наведении на курсор
                textBoxPlaneRegnum.BorderBrush = Brushes.DarkRed; //цвет фона задается цветом Brushes.DarkRed
            }
            if (isFound && isCorrect)
            {
                if (db.Bills.Any(b => b.planeId == planeToRemove.id && b.isRentNow))
                {
                    MessageBox.Show("К сожалению, данный самолет недоступен для удаления, так как он в данный момент арендуется!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить данный самолет без возможности восстановления?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    db.Planes.Remove(planeToRemove);
                    db.SaveChanges();
                    MessageBox.Show("Самолет успешно удален", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
        }
    }
}