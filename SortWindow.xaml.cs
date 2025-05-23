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

namespace BPR
{
    /// <summary>
    /// Логика взаимодействия для SortWindow.xaml
    /// </summary>
    public partial class SortWindow : Window
    {
        public string selectedSort;
        public bool isClosedNormal;

        public SortWindow(string selectedSort)
        {
            InitializeComponent();
            isClosedNormal = false;
            if (selectedSort == "умолчанию")
            {
                sortComboBox.SelectedIndex = 0;
            }
            else if (selectedSort == "цене (возрастание)")
            {
                sortComboBox.SelectedIndex = 1;
            }
            else if (selectedSort == "цене (убывание)")
            {
                sortComboBox.SelectedIndex = 2;
            }
            else if (selectedSort == "году создания (возрастание)")
            {
                sortComboBox.SelectedIndex = 3;
            }
            else if (selectedSort == "году создания (убывание)")
            {
                sortComboBox.SelectedIndex = 4;
            }
            else if (selectedSort == "общему налету (возрастание)")
            {
                sortComboBox.SelectedIndex = 5;
            }
            else if (selectedSort == "общему налету (убывание)")
            {
                sortComboBox.SelectedIndex = 6;
            }
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = sortComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                selectedSort = selectedItem.Content.ToString();
            }
            isClosedNormal = true;
            this.Close();
        }
    }
}