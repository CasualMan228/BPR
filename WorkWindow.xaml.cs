using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace BPR
{
    /// <summary>
    /// Логика взаимодействия для WorkWindow.xaml
    /// </summary>
    public partial class WorkWindow : Window
    {//НА БУДУЩЕЕ - ОБРАЩАЙСЯ К БД НАПРЯМУЮ БЕЗ ПОДТЯГИВАНИЙ КОРТЕЖЕЙ СПИСКОМ, А ТАК ПРОВЕРКИ ЧЕРЕЗ LINQ (db.Planes.Any(условие);) => СДЕЛАТЬ ОПТИМИЗАЦИЮ
        ApplicationContext db;
        List<User> users;
        List<Plane> planes;
        List<Bill> bills;
        double scrollPosition;
        string selectedType = "Все";
        string selectedCategory = "Все";
        string selectedSort = "умолчанию";
        public string currentUserName { get; set; }
        public string currentUserRole { get; set; }
        public Button rentButton;
        public WorkWindow(string name) //ПОСЛЕ ОКНА ВХОДА
        {
            InitializeComponent();
            db = MainWindow.db;
            users = db.Users.ToList(); //вытянуть все кортежи с таблицы БД Users и суем их в список
            planes = db.Planes.ToList();
            currentUserName = name;
            var foundUser = users.FirstOrDefault(user => user.name == name);
            currentUserRole = foundUser.role;
            if (currentUserRole != "admin")
            {
                adminPanel.IsEnabled = false;
                adminPanel.Visibility = Visibility.Hidden;
            }
            contentPanel.Children.Clear();
            SetPlanes();
        }
        public WorkWindow(string name, string role) //ПОСЛЕ ОКНА РЕГИСТРАЦИИ
        {
            InitializeComponent();
            db = MainWindow.db;
            planes = db.Planes.ToList();
            currentUserName = name;
            currentUserRole = role;
            if (currentUserRole != "admin")
            {
                adminPanel.IsEnabled = false;
                adminPanel.Visibility = Visibility.Hidden;
            }
            contentPanel.Children.Clear();
            SetPlanes();
        }
        void SetPlanes()
        {
            planes = db.Planes.ToList();
            bills = db.Bills.ToList();
            contentPanel.Children.Clear();
            List<Plane> searchedPlanes = new List<Plane>();
            List<Plane> filteredPlanes = new List<Plane>();
            List<Plane> sortedPlanes = new List<Plane>();
            if (textBoxPlaneName.Text != "")
            {
                foreach (var plane in planes)
                {
                    if (plane.name.IndexOf(textBoxPlaneName.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        searchedPlanes.Add(plane); //здесь может быть пусто
                    }
                }
            }
            else if (textBoxPlaneName.Text == "" || textBoxPlaneName.Text == null)
            {
                foreach (var plane in planes)
                {
                    searchedPlanes.Add(plane);
                }
            }
            //поискнутые самолеты зафиксированы
            if (selectedType == "Все" && selectedCategory == "Все" && searchedPlanes.Any())
            {
                foreach (var plane in searchedPlanes)
                {
                    filteredPlanes.Add(plane);
                }
            }
            else if (selectedType != "Все" && selectedCategory != "Все" && searchedPlanes.Any())
            {
                foreach (var plane in searchedPlanes)
                {
                    if (plane.type == selectedType && plane.category == selectedCategory)
                    {
                        filteredPlanes.Add(plane);
                    }
                }
            }
            else if (selectedType != "Все" && selectedCategory == "Все" && searchedPlanes.Any())
            {
                foreach (var plane in searchedPlanes)
                {
                    if (plane.type == selectedType)
                    {
                        filteredPlanes.Add(plane);
                    }
                }
            }
            else if (selectedType == "Все" && selectedCategory != "Все" && searchedPlanes.Any())
            {
                foreach (var plane in searchedPlanes)
                {
                    if (plane.category == selectedCategory)
                    {
                        filteredPlanes.Add(plane);
                    }
                }
            }
            //отфильтрованные самолеты зафиксированы
            if (selectedSort == "умолчанию" && filteredPlanes.Any())
            {
                foreach (var plane in filteredPlanes)
                {
                    sortedPlanes.Add(plane);
                }
            }
            else if (selectedSort == "цене (возрастание)" && filteredPlanes.Any())
            {
                sortedPlanes = filteredPlanes.OrderBy(plane => plane.price).ToList();
            }
            else if (selectedSort == "цене (убывание)" && filteredPlanes.Any())
            {
                sortedPlanes = filteredPlanes.OrderByDescending(plane => plane.price).ToList();
            }
            else if (selectedSort == "году выпуска (возрастание)" && filteredPlanes.Any())
            {
                sortedPlanes = filteredPlanes.OrderBy(plane => plane.year).ToList();
            }
            else if (selectedSort == "году выпуска (убывание)" && filteredPlanes.Any())
            {
                sortedPlanes = filteredPlanes.OrderByDescending(plane => plane.year).ToList();
            }
            else if (selectedSort == "общему налету (возрастание)" && filteredPlanes.Any())
            {
                sortedPlanes = filteredPlanes.OrderBy(plane => plane.totalFly).ToList();
            }
            else if (selectedSort == "общему налету (убывание)" && filteredPlanes.Any())
            {
                sortedPlanes = filteredPlanes.OrderByDescending(plane => plane.totalFly).ToList();
            }
            //отсортированные самолеты зафиксированы
            var availablePlanes = sortedPlanes.Where(plane => !bills.Any(bill => bill.planeId == plane.id && bill.isRentNow)).ToList();
            //доступные самолеты зафиксированы
            if (availablePlanes.Any())
            {
                foreach (var plane in availablePlanes)
                {
                    PrintBorder(plane);
                }
            }
            searchedPlanes.Clear();
            filteredPlanes.Clear();
            sortedPlanes.Clear();
            availablePlanes.Clear();
        }
        void PrintBorder(Plane plane)
        {
            contentPanel.ItemWidth = 350;
            Border border = new Border
            {
                Background = Brushes.LightGray,
                Margin = new Thickness(10),
                Padding = new Thickness(15),
                CornerRadius = new CornerRadius(5),
                Height = 350,
                Cursor = Cursors.Hand,
                Tag = plane
            };
            border.MouseLeftButtonDown += Border_Click;
            StackPanel stackPanel = new StackPanel();
            border.Child = stackPanel;
            TextBlock planeName = new TextBlock
            {
                Text = plane.name,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.Medium
            };
            stackPanel.Children.Add(planeName);
            TextBlock planeType = new TextBlock()
            {
                Text = plane.type,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.Medium
            };
            stackPanel.Children.Add(planeType);
            byte[] binaryPhotoNeed = Convert.FromBase64String(plane.photoNeed);
            var bitmap = new BitmapImage(); //пустая картинка
            using (var memoryStream = new MemoryStream(binaryPhotoNeed))
            {
                bitmap.BeginInit(); //пока картинку не показываем
                bitmap.CacheOption = BitmapCacheOption.OnLoad; //прочитай все байты из потока
                bitmap.StreamSource = memoryStream; //типа вот этот вот поток
                bitmap.EndInit(); //все, показывай картинку
            } //вообщем сложная ерунда, которая берет картинку из БД(BASE64) и обрабатывает ее в ОЗУ и кидает нам результат
            Image planeImage = new Image()
            {
                Source = bitmap,
                Width = 400,
                Height = 200,
                Margin = new Thickness(0, 10, 0, 10)
            };
            RenderOptions.SetBitmapScalingMode(planeImage, BitmapScalingMode.HighQuality);
            stackPanel.Children.Add(planeImage);
            TextBlock planeTotalFly = new TextBlock()
            {
                Text = "Общий налет: " + plane.totalFly.ToString("N0") + " км",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.Medium
            };
            stackPanel.Children.Add(planeTotalFly);
            TextBlock planePrice = new TextBlock()
            {
                Text = plane.price.ToString("N0") + " BYN сутки", //N -> форматированное число, а 0 -> кол-во знаков после запятой
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0),
                FontFamily = new FontFamily("Arial")
            };
            stackPanel.Children.Add(planePrice);
            contentPanel.Children.Add(border);
        }
        void PrintChoiseBorder(Plane currentPlane)
        {
            scrollViewer.ScrollToVerticalOffset(0);
            contentPanel.ItemWidth = 1100;
            Button back = new Button
            {
                Content = "Назад",
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontWeight = FontWeights.Bold
            };
            back.Style = (Style)FindResource("MaterialDesignFlatButton");
            back.Click += Back_Click;
            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(back);
            TextBlock planeName = new TextBlock
            {
                Text = currentPlane.name,
                FontSize = 22,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            };
            stackPanel.Children.Add(planeName);
            TextBlock planeRegnum = new TextBlock()
            {
                Text = currentPlane.regnum,
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.Medium,
                Margin = new Thickness(0, 0, 0, 30)
            };
            stackPanel.Children.Add(planeRegnum);
            TextBlock photo = new TextBlock
            {
                Text = "Фотографии:",
                FontSize = 22,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20)
            };
            stackPanel.Children.Add(photo);
            if (currentPlane.photo1 == null)
            {
                #region та сложная ерунда
                byte[] binaryPhotoNeed = Convert.FromBase64String(currentPlane.photoNeed);
                var bitmap = new BitmapImage(); //пустая картинка
                using (var memoryStream = new MemoryStream(binaryPhotoNeed))
                {
                    bitmap.BeginInit(); //пока картинку не показываем
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; //прочитай все байты из потока
                    bitmap.StreamSource = memoryStream; //типа вот этот вот поток
                    bitmap.EndInit(); //все, показывай картинку
                } //вообщем сложная ерунда, которая берет картинку из БД(BASE64) и обрабатывает ее в ОЗУ и кидает нам результат
                #endregion
                Image planeImage = new Image()
                {
                    Source = bitmap,
                    Width = 800,
                    Height = 500,
                    Margin = new Thickness(0, 0, 0, 30),
                    Stretch = Stretch.Fill
                };
                RenderOptions.SetBitmapScalingMode(planeImage, BitmapScalingMode.HighQuality);
                stackPanel.Children.Add(planeImage);
            }
            else if (currentPlane.photo1 != null && currentPlane.photo2 == null)
            {
                #region та сложная ерунда
                byte[] binaryPhotoNeed = Convert.FromBase64String(currentPlane.photoNeed);
                var bitmap = new BitmapImage(); //пустая картинка
                using (var memoryStream = new MemoryStream(binaryPhotoNeed))
                {
                    bitmap.BeginInit(); //пока картинку не показываем
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; //прочитай все байты из потока
                    bitmap.StreamSource = memoryStream; //типа вот этот вот поток
                    bitmap.EndInit(); //все, показывай картинку
                } //вообщем сложная ерунда, которая берет картинку из БД(BASE64) и обрабатывает ее в ОЗУ и кидает нам результат
                #endregion
                Image planeImage = new Image()
                {
                    Source = bitmap,
                    Width = 800,
                    Height = 500,
                    Margin = new Thickness(0, 0, 0, 10),
                    Stretch = Stretch.Fill
                };
                RenderOptions.SetBitmapScalingMode(planeImage, BitmapScalingMode.HighQuality);
                stackPanel.Children.Add(planeImage);
                #region та сложная ерунда
                byte[] binaryPhoto1 = Convert.FromBase64String(currentPlane.photo1);
                var bitmap1 = new BitmapImage(); //пустая картинка
                using (var memoryStream = new MemoryStream(binaryPhoto1))
                {
                    bitmap1.BeginInit(); //пока картинку не показываем
                    bitmap1.CacheOption = BitmapCacheOption.OnLoad; //прочитай все байты из потока
                    bitmap1.StreamSource = memoryStream; //типа вот этот вот поток
                    bitmap1.EndInit(); //все, показывай картинку
                } //вообщем сложная ерунда, которая берет картинку из БД(BASE64) и обрабатывает ее в ОЗУ и кидает нам результат
                #endregion
                Image planeImage1 = new Image()
                {
                    Source = bitmap1,
                    Width = 800,
                    Height = 500,
                    Margin = new Thickness(0, 0, 0, 30),
                    Stretch = Stretch.Fill
                };
                RenderOptions.SetBitmapScalingMode(planeImage1, BitmapScalingMode.HighQuality);
                stackPanel.Children.Add(planeImage1);
            }
            else
            {
                #region та сложная ерунда
                byte[] binaryPhotoNeed = Convert.FromBase64String(currentPlane.photoNeed);
                var bitmap = new BitmapImage(); //пустая картинка
                using (var memoryStream = new MemoryStream(binaryPhotoNeed))
                {
                    bitmap.BeginInit(); //пока картинку не показываем
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; //прочитай все байты из потока
                    bitmap.StreamSource = memoryStream; //типа вот этот вот поток
                    bitmap.EndInit(); //все, показывай картинку
                } //вообщем сложная ерунда, которая берет картинку из БД(BASE64) и обрабатывает ее в ОЗУ и кидает нам результат
                #endregion
                Image planeImage = new Image()
                {
                    Source = bitmap,
                    Width = 800,
                    Height = 500,
                    Margin = new Thickness(0, 0, 0, 10),
                    Stretch = Stretch.Fill
                };
                RenderOptions.SetBitmapScalingMode(planeImage, BitmapScalingMode.HighQuality);
                stackPanel.Children.Add(planeImage);
                #region та сложная ерунда
                byte[] binaryPhoto1 = Convert.FromBase64String(currentPlane.photo1);
                var bitmap1 = new BitmapImage(); //пустая картинка
                using (var memoryStream = new MemoryStream(binaryPhoto1))
                {
                    bitmap1.BeginInit(); //пока картинку не показываем
                    bitmap1.CacheOption = BitmapCacheOption.OnLoad; //прочитай все байты из потока
                    bitmap1.StreamSource = memoryStream; //типа вот этот вот поток
                    bitmap1.EndInit(); //все, показывай картинку
                } //вообщем сложная ерунда, которая берет картинку из БД(BASE64) и обрабатывает ее в ОЗУ и кидает нам результат
                #endregion
                Image planeImage1 = new Image()
                {
                    Source = bitmap1,
                    Width = 800,
                    Height = 500,
                    Margin = new Thickness(0, 0, 0, 10),
                    Stretch = Stretch.Fill
                };
                RenderOptions.SetBitmapScalingMode(planeImage1, BitmapScalingMode.HighQuality);
                stackPanel.Children.Add(planeImage1);
                #region та сложная ерунда
                byte[] binaryPhoto2 = Convert.FromBase64String(currentPlane.photo2);
                var bitmap2 = new BitmapImage(); //пустая картинка
                using (var memoryStream = new MemoryStream(binaryPhoto2))
                {
                    bitmap2.BeginInit(); //пока картинку не показываем
                    bitmap2.CacheOption = BitmapCacheOption.OnLoad; //прочитай все байты из потока
                    bitmap2.StreamSource = memoryStream; //типа вот этот вот поток
                    bitmap2.EndInit(); //все, показывай картинку
                } //вообщем сложная ерунда, которая берет картинку из БД(BASE64) и обрабатывает ее в ОЗУ и кидает нам результат
                #endregion
                Image planeImage2 = new Image()
                {
                    Source = bitmap2,
                    Width = 800,
                    Height = 500,
                    Margin = new Thickness(0, 0, 0, 30),
                    Stretch = Stretch.Fill
                };
                RenderOptions.SetBitmapScalingMode(planeImage2, BitmapScalingMode.HighQuality);
                stackPanel.Children.Add(planeImage2);
            }
            TextBlock description = new TextBlock
            {
                Text = "Описание:",
                FontSize = 22,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20)
            };
            stackPanel.Children.Add(description);
            StackPanel stackPanelDescription = new StackPanel()
            {
                MaxWidth = 1050
            };
            TextBlock planeDescription = new TextBlock()
            {
                Text = currentPlane.description,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 10),
                TextWrapping = TextWrapping.Wrap
            };
            stackPanelDescription.Children.Add(planeDescription);
            stackPanel.Children.Add(stackPanelDescription);
            StackPanel stackPanelType = new StackPanel()
            {
                MaxWidth = 1050
            };
            TextBlock planeType = new TextBlock()
            {
                Text = "Тип: " + currentPlane.type,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 10),
            };
            stackPanelType.Children.Add(planeType);
            stackPanel.Children.Add(stackPanelType);
            StackPanel stackPanelCategory = new StackPanel()
            {
                MaxWidth = 1050
            };
            TextBlock planeCategory = new TextBlock()
            {
                Text = "Категория: " + currentPlane.category,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 10),
            };
            stackPanelCategory.Children.Add(planeCategory);
            stackPanel.Children.Add(stackPanelCategory);
            StackPanel stackPanelYear = new StackPanel()
            {
                MaxWidth = 1050
            };
            TextBlock planeYear = new TextBlock()
            {
                Text = "Год выпуска: " + currentPlane.year,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 10),
            };
            stackPanelYear.Children.Add(planeYear);
            stackPanel.Children.Add(stackPanelYear);
            StackPanel stackPanelMaker = new StackPanel()
            {
                MaxWidth = 1050
            };
            TextBlock planeMaker = new TextBlock()
            {
                Text = "Производитель: " + currentPlane.maker,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 10),
            };
            stackPanelMaker.Children.Add(planeMaker);
            stackPanel.Children.Add(stackPanelMaker);
            StackPanel stackPanelCountry = new StackPanel()
            {
                MaxWidth = 1050
            };
            TextBlock planeCountry = new TextBlock()
            {
                Text = "Страна регистрации: " + currentPlane.country,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 10),
            };
            stackPanelCountry.Children.Add(planeCountry);
            stackPanel.Children.Add(stackPanelCountry);
            StackPanel stackPanelAnotherRegnum = new StackPanel()
            {
                MaxWidth = 1050
            };
            TextBlock planeAnotherRegnum = new TextBlock()
            {
                Text = "Регистрационный номер: " + currentPlane.regnum,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 10),
            };
            stackPanelAnotherRegnum.Children.Add(planeAnotherRegnum);
            stackPanel.Children.Add(stackPanelAnotherRegnum);
            StackPanel stackPanelTotalFly = new StackPanel()
            {
                MaxWidth = 1050
            };
            TextBlock planeTotalFly = new TextBlock()
            {
                Text = "Общий налет: " + currentPlane.totalFly.ToString("N0") + " км",
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 0),
            };
            stackPanelTotalFly.Children.Add(planeTotalFly);
            stackPanel.Children.Add(stackPanelTotalFly);
            rentButton = new Button()
            {
                Content = "Предварительно забронировать (MS Word должен быть установлен!)",
                Margin = new Thickness(0, 20, 0, 10),
                Height = 35,
                Width = 850,
                Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#4682B4"),
                BorderBrush = (SolidColorBrush) new BrushConverter().ConvertFrom("#4682B4")
            };
            rentButton.Click += RentButtonClick;
            rentButton.Tag = currentPlane;
            stackPanel.Children.Add(rentButton);
            contentPanel.Children.Add(stackPanel);
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            scrollViewer.ScrollToVerticalOffset(scrollPosition);
            contentPanel.Children.Clear();
            textBoxPlaneName.IsEnabled = true;
            textBoxPlaneName.Visibility = Visibility.Visible;
            sort.IsEnabled = true;
            sort.Visibility = Visibility.Visible;
            filter.IsEnabled = true;
            filter.Visibility = Visibility.Visible;
            search.Visibility = Visibility.Visible;
            adminPanel.IsEnabled = true;
            SetPlanes();
        }
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("help.chm");
        }
        private void AboutUs_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Компания: BPR (Belarus Plane Rent)\nАдрес: ул. Аэровокзальная, 148, Минск, Беларусь\nЭлектронная почта: belarusplanerentinfo@gmail.com\nМы предоставляем услуги аренды самолетов по всей Беларуси ✈", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddPlane_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("При добавлении самолета текущие настройки фильтрации, сортировки и поиска будут сброшены до значений по умолчанию. Продолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                selectedType = "Все";
                selectedCategory = "Все";
                selectedSort = "умолчанию";
                textBoxPlaneName.Text = "";
                AddPlaneWindow addPlaneWindow = new AddPlaneWindow();
                addPlaneWindow.ShowDialog();
                SetPlanes();
            }
        }

        private void RemovePlane_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("При удалении самолета текущие настройки фильтрации, сортировки и поиска будут сброшены до значений по умолчанию. Продолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                selectedType = "Все";
                selectedCategory = "Все";
                selectedSort = "умолчанию";
                textBoxPlaneName.Text = "";
                RemovePlaneWindow removePlaneWindow = new RemovePlaneWindow();
                removePlaneWindow.ShowDialog();
                SetPlanes();
            }
        }

        private void EditPlane_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("При изменении самолета текущие настройки фильтрации, сортировки и поиска будут сброшены до значений по умолчанию. Продолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                selectedType = "Все";
                selectedCategory = "Все";
                selectedSort = "умолчанию";
                textBoxPlaneName.Text = "";
                SelectToChangePlaneWindow selectToChangePlaneWindow = new SelectToChangePlaneWindow();
                selectToChangePlaneWindow.ShowDialog();
                SetPlanes();
            }
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("При просмотре общей статистики текущие настройки фильтрации, сортировки и поиска будут сброшены до значений по умолчанию. Продолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                selectedType = "Все";
                selectedCategory = "Все";
                selectedSort = "умолчанию";
                textBoxPlaneName.Text = "";
                SelectToStatisticsWindow selectToStatisticsWindow = new SelectToStatisticsWindow();
                selectToStatisticsWindow.ShowDialog();
                SetPlanes();
            }
        }

        private void TextBoxPlaneName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetPlanes();
            }
        }

        private void Filter_Click(object sender, MouseButtonEventArgs e)
        {
            FilterWindow filterWindow = new FilterWindow(selectedType, selectedCategory);
            filterWindow.ShowDialog();
            bool isClosedNormal = filterWindow.isClosedNormal;
            if (isClosedNormal == true)
            {
                selectedType = filterWindow.selectedType;
                selectedCategory = filterWindow.selectedCategory;
            }
            SetPlanes();
        }
        private void Sort_Click(object sender, MouseButtonEventArgs e)
        {
            SortWindow sortWindow = new SortWindow(selectedSort);
            sortWindow.ShowDialog();
            bool isClosedNormal = sortWindow.isClosedNormal;
            if (isClosedNormal == true)
            {
                selectedSort = sortWindow.selectedSort;
            }
            SetPlanes();
        }
        private void Border_Click(object sender, MouseButtonEventArgs e)
        {
            scrollPosition = scrollViewer.VerticalOffset;
            contentPanel.Children.Clear();
            textBoxPlaneName.IsEnabled = false;
            textBoxPlaneName.Visibility = Visibility.Hidden;
            sort.IsEnabled = false;
            sort.Visibility = Visibility.Hidden;
            filter.IsEnabled = false;
            filter.Visibility = Visibility.Hidden;
            search.Visibility = Visibility.Hidden;
            adminPanel.IsEnabled = false;
            var choiseBorder = (Border)sender;
            var choisePlane = choiseBorder.Tag as Plane;
            /*if (choisePlane == null)
            {
                MessageBox.Show("Данный самолет не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }*/
            PrintChoiseBorder(choisePlane);
        }
        private void Bills_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("При просмотре учета бронирований текущие настройки фильтрации, сортировки и поиска будут сброшены до значений по умолчанию. Продолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                selectedType = "Все";
                selectedCategory = "Все";
                selectedSort = "умолчанию";
                textBoxPlaneName.Text = "";
                SelectToBillsWindow selectToBillsWindow = new SelectToBillsWindow();
                selectToBillsWindow.ShowDialog();
                SetPlanes();
            }
        }
        private void RentButtonClick(object sender, RoutedEventArgs e)
        {
            bool isPlaneAvailable = false;
            planes = db.Planes.ToList(); //вытянуть все кортежи с таблицы БД Users и суем их в список
            bills = db.Bills.ToList();
            var currentPlane = rentButton.Tag as Plane;
            foreach (var plane in planes)
            {
                if (plane.regnum == currentPlane.regnum && !bills.Any(bill => bill.planeId == plane.id && bill.isRentNow))
                {
                    isPlaneAvailable = true;
                }
            }
            if (!isPlaneAvailable)
            {
                MessageBox.Show("К сожалению, данный самолет недоступен для аренды!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                InputToRentPlaneWindow inputToRentPlaneWindow = new InputToRentPlaneWindow(currentPlane, currentUserRole, currentUserName);
                inputToRentPlaneWindow.ShowDialog();
                bool isClosedNormal = inputToRentPlaneWindow.isClosedNormal;
                if (isClosedNormal == true)
                {
                    scrollViewer.ScrollToVerticalOffset(scrollPosition);
                    contentPanel.Children.Clear();
                    textBoxPlaneName.IsEnabled = true;
                    textBoxPlaneName.Visibility = Visibility.Visible;
                    sort.IsEnabled = true;
                    sort.Visibility = Visibility.Visible;
                    filter.IsEnabled = true;
                    filter.Visibility = Visibility.Visible;
                    search.Visibility = Visibility.Visible;
                    adminPanel.IsEnabled = true;
                    SetPlanes();
                }
            }
        }
    }
}
/* ВАРИАНТ НА РАССМОТРЕНИЕ (ОПТИМИЗАЦИЯ ПРИ ПОМОЩИ LINQ)
void SetPlanes()
{
    contentPanel.Children.Clear();

    var query = planes.AsEnumerable();

    // Поиск
    if (!string.IsNullOrEmpty(textBoxPlaneName.Text))
    {
        query = query.Where(p => p.name.IndexOf(textBoxPlaneName.Text, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    // Фильтрация
    if (selectedType != "Все")
    {
        query = query.Where(p => p.type == selectedType);
    }
    if (selectedCategory != "Все")
    {
        query = query.Where(p => p.category == selectedCategory);
    }

    // Сортировка
    switch (selectedSort)
    {
        case "цене (возрастание)":
            query = query.OrderBy(p => p.price);
            break;
        case "цене (убывание)":
            query = query.OrderByDescending(p => p.price);
            break;
        case "году выпуска (возрастание)":
            query = query.OrderBy(p => p.year);
            break;
        case "году выпуска (убывание)":
            query = query.OrderByDescending(p => p.year);
            break;
        case "общему налету (возрастание)":
            query = query.OrderBy(p => p.totalFly);
            break;
        case "общему налету (убывание)":
            query = query.OrderByDescending(p => p.totalFly);
            break;
    }

    // Отрисовка
    foreach (var plane in query)
    {
        PrintBorder(plane);
    }
} рассмотреть вариантик после фулл работы
*/