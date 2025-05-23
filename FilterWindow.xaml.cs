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
    /// Логика взаимодействия для FilterWindow.xaml
    /// </summary>
    public partial class FilterWindow : Window
    {
        public string selectedType;
        public string selectedCategory;
        public bool isClosedNormal;

        public FilterWindow(string selectedType, string selectedCategory)
        {
            InitializeComponent();
            isClosedNormal = false;
            if (selectedType == "Все")
            {
                typeComboBox.SelectedIndex = 0;
            }
            else if (selectedType == "Jet")
            {
                typeComboBox.SelectedIndex = 1;
            }
            else if (selectedType == "Turboprop")
            {
                typeComboBox.SelectedIndex = 2;
            }
            else if (selectedType == "Glider")
            {
                typeComboBox.SelectedIndex = 3;
            }
            if (selectedCategory == "Все")
            {
                categoryComboBox.SelectedIndex = 0;
            }
            else if (selectedCategory == "Passenger")
            {
                categoryComboBox.SelectedIndex = 1;
            }
            else if (selectedCategory == "Cargo")
            {
                categoryComboBox.SelectedIndex = 2;
            }
            else if (selectedCategory == "Study")
            {
                categoryComboBox.SelectedIndex = 3;
            }
            else if (selectedCategory == "Military")
            {
                categoryComboBox.SelectedIndex = 4;
            }
            else if (selectedCategory == "Sport")
            {
                categoryComboBox.SelectedIndex = 5;
            }
            else if (selectedCategory == "Business")
            {
                categoryComboBox.SelectedIndex = 6;
            }
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            var typeItem = typeComboBox.SelectedItem as ComboBoxItem;
            var categoryItem = categoryComboBox.SelectedItem as ComboBoxItem;
            if (typeItem != null && categoryItem != null)
            {
                selectedType = typeItem.Content.ToString();
                selectedCategory = categoryItem.Content.ToString();
            }
            isClosedNormal = true;
            this.Close();
        }
    }
}