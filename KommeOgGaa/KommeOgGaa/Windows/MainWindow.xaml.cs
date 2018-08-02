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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Printing;
using System.Runtime.InteropServices;

namespace KommeOgGaa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Dictionary<string, object> Settings = new Dictionary<string, object>();
        //List<BitmapImage> imageList = new List<BitmapImage>();
        ObservableCollection<Person> todayList = new ObservableCollection<Person>();
        ObservableCollection<Person> galleryList = new ObservableCollection<Person>();
        Person checkInToCheckOut;



        public bool IsAdmin
        {
            get { return (bool)GetValue(IsAdminProperty); }
            set { SetValue(IsAdminProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAdmin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAdminProperty =
            DependencyProperty.Register("IsAdmin", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));



        public MainWindow()
        {
            InitializeComponent();
            PictureList.ItemsSource = todayList;
            Gallery.ItemsSource = galleryList;
            

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Gallery.ItemsSource);
            view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Date", System.ComponentModel.ListSortDirection.Ascending));
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Date");
            view.GroupDescriptions.Add(groupDescription);

            
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConnectToDatabase();
            //CreateDatabase(); 
            LoadSettings();
            Person.DeleteOldRecords(Convert.ToInt32(Settings["DeleteData"]));
            todayList.Clear();

            //Person p = new Person()
            //{
            //    PicturesLocation = "",
            //    IsCheckIn = true,
            //    IsLate = false,
            //    Category = "A",
            //    Ticks = new DateTime(2018, 6, 18, 8, 0, 0).Ticks,

            //};
            //p.Insert();
            //p.Insert();
            //p.Insert();

            foreach (var item in Person.GetAllCheckInToday())
            {
                todayList.Add(item);
            }

            UpdateTodayList();
        }

        private static void LoadSettings()
        {
           var rows = SQLite_DB_LIB.Database.GetRows<int>("Settings", new string[] { "Key", "Value" });
            foreach (var row in rows)
            {
                Settings.Add(row[0].ToString(), row[1]);
            }
        }

        private static void ConnectToDatabase()
        {
            string folder = Directory.GetCurrentDirectory() + "\\Settings";

            if (!Directory.Exists(folder))
            {
                DirectoryInfo di = Directory.CreateDirectory(folder);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            SQLite_DB_LIB.Database.Connect("Data Source="+folder + "\\database.sqlite;Version=3;");
            
        }

        private static void CreateDatabase()
        {
            SQLite_DB_LIB.Database.Create("Persons",
                new SQLite_DB_LIB.Column() { name = "Index", type = SQLite_DB_LIB.Column.TYPE_INT, isAutoIncrement = true, isPrimaryKey = true, isNotNull = true},
                new SQLite_DB_LIB.Column() { name = "RelationID", type = SQLite_DB_LIB.Column.TYPE_INT},
                new SQLite_DB_LIB.Column() { name = "PicturesLocation", type = SQLite_DB_LIB.Column.TYPE_STRING, isNotNull = true },
                new SQLite_DB_LIB.Column() { name = "Ticks", type = SQLite_DB_LIB.Column.TYPE_INT, isNotNull = true },
                new SQLite_DB_LIB.Column() { name = "Category", type = SQLite_DB_LIB.Column.TYPE_STRING, isNotNull = true },
                new SQLite_DB_LIB.Column() { name = "IsCheckIn", type = SQLite_DB_LIB.Column.TYPE_INT, isNotNull = true },
                new SQLite_DB_LIB.Column() { name = "IsLate", type = SQLite_DB_LIB.Column.TYPE_INT, isNotNull = true }
            );

            SQLite_DB_LIB.Database.Create("Settings",
                new SQLite_DB_LIB.Column() { name = "Key", type = SQLite_DB_LIB.Column.TYPE_STRING, isPrimaryKey = true, isNotNull = true },
                new SQLite_DB_LIB.Column() { name = "Value", type = SQLite_DB_LIB.Column.TYPE_STRING }
            );

            SQLite_DB_LIB.Database.Insert("Settings", new string[] { "Key", "Value" }, new object[] { "CheckInHour", "7" });
            SQLite_DB_LIB.Database.Insert("Settings", new string[] { "Key", "Value" }, new object[] { "CheckInMinute", "30" });
            SQLite_DB_LIB.Database.Insert("Settings", new string[] { "Key", "Value" }, new object[] { "CheckOutHour", "15" });
            SQLite_DB_LIB.Database.Insert("Settings", new string[] { "Key", "Value" }, new object[] { "CheckOutMinute", "0" });

            SQLite_DB_LIB.Database.Insert("Settings", new string[] { "Key", "Value" }, new object[] { "DeleteData", "7" });

            SQLite_DB_LIB.Database.Create("Admins",
                new SQLite_DB_LIB.Column() { name = "Username", type = SQLite_DB_LIB.Column.TYPE_STRING, isPrimaryKey = true, isNotNull = true },
                new SQLite_DB_LIB.Column() { name = "Password", type = SQLite_DB_LIB.Column.TYPE_STRING }
            );

            //SQLite_DB_LIB.Database.Insert("Admins", new string[] { "Username", "Password" }, new object[] { "", "" });

        }

        private void UpdateTodayList()
        {
            if (PictureList == null)
            {
                return;
            }

            string selectedCat = "";

            if (rBtnCategoryA.IsChecked ?? false) selectedCat = rBtnCategoryA.Content.ToString();
            else if (rBtnCategoryB.IsChecked ?? false) selectedCat = rBtnCategoryB.Content.ToString();
            else if (rBtnCategoryC.IsChecked ?? false) selectedCat = rBtnCategoryC.Content.ToString();

            PictureList.ItemsSource = todayList.Where(m => m.Category == selectedCat);
        }

        private void PictureList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Button!");
        }

        private void Button_CheckOut_Click(object sender, RoutedEventArgs e) //Tjek ud kanp
        {
            checkInToCheckOut = (Person)PictureList.SelectedItem;
            CameraView.IsCheckIn = false;
            CameraBtn.IsChecked = true;
        }
        
        private void Button_BackToGallery_Click(object sender, RoutedEventArgs e) //Tilbage knap
        {
            imageGrid.Visibility = Visibility.Collapsed;
            PictureList.Visibility = Visibility.Visible;
            PictureList.SelectedIndex = -1;
            UpdateTodayList();
        }

        private void PictureList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            
            if (((ListView)sender).SelectedIndex >= 0 && ((ListView)sender).SelectedIndex < ((ListView)sender).Items.Count)
            {
                imageGrid.Visibility = Visibility.Visible;
                PictureList.Visibility = Visibility.Collapsed;
                imageGrid.DataContext = ((ListView)sender).SelectedItem;
            }
        }

        private void Button_CloseCamera_Click(object sender, RoutedEventArgs e)
        {
            CameraView.IsCheckIn = true;
            CameraBtn.IsChecked = false;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //this.Focus();
            //Keyboard.ClearFocus();
            
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsLoaded && (sender as UIElement).Visibility == Visibility.Visible)
            {
                CameraView.Start();
            }
            else
            {
                CameraView.Stop();
            }
        }

        private void CameraView_OnPictureCompleted(object sender, bool arg2, string picturesLocation)
        {
          
            int h = CameraView.IsCheckIn ? Convert.ToInt32(Settings["CheckInHour"]) : Convert.ToInt32(Settings["CheckOutHour"]);
            int m = CameraView.IsCheckIn ? Convert.ToInt32(Settings["CheckInMinute"]) : Convert.ToInt32(Settings["CheckOutMinute"]);

            DateTime time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, h, m, 0);

            if (CameraView.IsCheckIn && DateTime.Now.Ticks > time.Ticks)
            {
                System.Windows.Forms.MessageBox.Show("Du er forsent til at tjekke dig ind.\nKontakt din lærer for mere information.", "Ikke Tjekket Ind", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                File.Delete(Directory.GetCurrentDirectory()+ picturesLocation);
                return;
            }
            else if (!CameraView.IsCheckIn && DateTime.Now.Ticks < time.Ticks)
            {
                System.Windows.Forms.MessageBox.Show("Du er for tidligt til at tjekke dig ud.\nKontakt din lærer for mere information.", "Ikke Tjekket Ud", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                File.Delete(Directory.GetCurrentDirectory() + picturesLocation);
                return;
                
            }



            string category = "";
            foreach (var item in personCategories.Children)
            {
                if ((item as RadioButton).IsChecked ?? false)
                {
                    category = (item as RadioButton).Content.ToString();
                    break;
                }
            }


            var person = new Person();
            person.Ticks = DateTime.Now.Ticks;
            person.PicturesLocation = picturesLocation;
            person.Category = category;
            person.IsCheckIn = CameraView.IsCheckIn;

            person.Insert();

            if (CameraView.IsCheckIn)
            {
                todayList.Add(person);
            }
            else
            {
                checkInToCheckOut.RelationID = person.Index;
                person.RelationID = checkInToCheckOut.Index;

                checkInToCheckOut.Update();
                person.Update();

                todayList.Remove(checkInToCheckOut);
            }
            CameraView.IsCheckIn = true;
            CameraBtn.IsChecked = false;
            Button_BackToGallery_Click(null, null);
        }

        private void CameraView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (CameraView.Visibility != Visibility.Visible)
            {
                CameraView.IsCheckIn = true;
                checkInToCheckOut = null;
            }
        }

        private void Gallery_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded && Gallery.Visibility == Visibility.Visible)
            {
                galleryList.Clear();

                foreach (var item in Person.GetAllCheckInPersons())
                {
                    galleryList.Add(item);
                }
            }
        }

        private void Button_LogOutAdmin_Click(object sender, RoutedEventArgs e)
        {
            IsAdmin = false;
            MenuHomeBtn.IsChecked = true;
        }

        private void Button_LogInAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (SQLite_DB_LIB.Database.Exist("Admins", new string[] { "Username", "Password" }, new object[] { txtAdminUser.Text, txtAdminPass.Password }))
            {
                IsAdmin = true;
                txtAdminUser.Text = "";
                txtAdminPass.Password = "";
                System.Windows.Forms.MessageBox.Show("Du er nu logget ind som admin", "Admin Login", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Login er forkert", "Admin Login", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            MenuHomeBtn.IsChecked = IsAdmin;
        }

        private void Button_SaveSetting_Click(object sender, RoutedEventArgs e)
        {
            Settings["CheckInHour"] = SliderCheckInHours.Value;
            Settings["CheckInMinute"] = SliderCheckInMin.Value;
            Settings["CheckOutHour"] = SliderCheckOutHours.Value;
            Settings["CheckOutMinute"] = SliderCheckOutMin.Value;

            int deleteDay = Convert.ToInt32(Settings["DeleteData"]);
            int.TryParse(txtDeleteData.Text, out deleteDay);
            Settings["DeleteData"] = deleteDay.ToString();

            foreach (var item in Settings)
            {
                SQLite_DB_LIB.Database.Update("Settings", new Dictionary<string, object>() { { "Value", item.Value.ToString() } }, item.Key);
            }

            UpdateSettingInView();

           System.Windows.Forms.MessageBox.Show("Indstillingerne er nu gemt.", "Gemt", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }

        private void UpdateSettingInView()
        {
            SliderCheckInHours.Value    = Convert.ToInt32(Settings["CheckInHour"]);
            SliderCheckInMin.Value      = Convert.ToInt32(Settings["CheckInMinute"]);
            SliderCheckOutHours.Value   = Convert.ToInt32(Settings["CheckOutHour"]);
            SliderCheckOutMin.Value     = Convert.ToInt32(Settings["CheckOutMinute"]);
            txtDeleteData.Text          = Settings["DeleteData"].ToString();
        }

        private void Settings_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded && (sender as UIElement).Visibility == Visibility.Visible)
            {
                UpdateSettingInView();
            }
        }

        private void Button_PrintCatelog_Click(object sender, RoutedEventArgs e)
        {
           // Gallery.SetValue(VirtualizingStackPanel.VirtualizationModeProperty, VirtualizationMode.Standard);
            var fv = Scripts.PrintHelper.GetFixedDocument(Gallery, new PrintDialog());
            Scripts.PrintHelper.ShowPrintPreview(fv);
        }

        private void rBtnCategory_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).IsChecked ?? false)
            {
                UpdateTodayList();
            }
        }

        private void PictureList_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded)
            {
                UpdateTodayList();
            }
        }
    }
}
