using System.Windows;

namespace IdeasManager
{
    /// <summary>
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutPage : Window
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.flaticon.com/authors/smashicons");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.flaticon.com/");
        }
    }
}
