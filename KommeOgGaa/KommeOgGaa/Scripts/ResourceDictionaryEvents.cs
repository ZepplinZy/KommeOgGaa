using System.Windows;
using System.Windows.Controls;

namespace KommeOgGaa.Scripts
{
    public partial class ResourceDictionaryEvents : ResourceDictionary
    {



        public ResourceDictionaryEvents()
        {
            InitializeComponent();
        }

        void OnPasswordChanged(object sender, RoutedEventArgs e)
        {

            if ((sender as PasswordBox).Password == "")
                (sender as PasswordBox).Tag = "Show";
            else
                (sender as PasswordBox).Tag = "Hidden";
        }


    }
}