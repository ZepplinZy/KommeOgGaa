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

namespace KommeOgGaa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //List<BitmapImage> imageList = new List<BitmapImage>();
        ObservableCollection<Person> imageList = new ObservableCollection<Person>();
        

        public MainWindow()
        {
            InitializeComponent();
            PictureList.ItemsSource = imageList;
            Gallery.ItemsSource = imageList;
        }

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] images = System.IO.Directory.GetFiles(@"C:\Users\kide\Pictures\Billeder til dansk");

            Random r = new Random();
            foreach (var path in images)
            {
                imageList.Add(new Person() { PicturesLocation = path, PictureTaken = "20/05/2018 12:03:" + r.Next(0,59) });
            }
        }


        private void PictureList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Button!");
        }

        private void Button_CheckOut_Click(object sender, RoutedEventArgs e) //Tjek ud kanp
        {
            Person selected = (Person)PictureList.SelectedItem;
            imageList.Remove(selected);

            Button_BackToGallery_Click(null, null);
        }
        
        private void Button_BackToGallery_Click(object sender, RoutedEventArgs e) //Tilbage knap
        {
            imageGrid.Visibility = Visibility.Collapsed;
            PictureList.Visibility = Visibility.Visible;
            PictureList.SelectedIndex = -1;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            


            

        }
        
    }
}
