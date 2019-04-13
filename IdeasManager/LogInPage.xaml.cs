using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;
namespace IdeasManager
{
    /// <summary>
    /// Interaction logic for LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Window
    {
        public LogInPage()
        {
            string connectionString = (@"Data Source=COLIN\SQLMAIN;Initial Catalog=TestLoginCredentials;Persist Security Info=True;User ID=sa;Password=Thr33four");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            InitializeComponent();
            PasswordConfirm.Visibility = Visibility.Collapsed;
            ConfirmPassword.Visibility = Visibility.Collapsed;
            // ConfirmPassword.IsVisible = false;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            LoginButton.Content = "Register";
            PasswordInput.Background = Brushes.LightPink;
            PasswordConfirm.Visibility = Visibility.Visible;
            ConfirmPassword.Visibility = Visibility.Visible;
            LoginButton.Background = Brushes.LightPink;
            UsernameHeading.Content = "New Username";
            PasswordHeading.Content = "New Password";
            LoginButton.FontWeight = FontWeights.Bold;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            LoginButton.Content = "Log In";
            PasswordInput.Background = Brushes.GhostWhite;
            PasswordConfirm.Visibility = Visibility.Collapsed;
            ConfirmPassword.Visibility = Visibility.Collapsed;
            LoginButton.Background = Brushes.GhostWhite;
            UsernameHeading.Content = "Username";
            PasswordHeading.Content = "Password";
            LoginButton.FontWeight = FontWeights.Normal;
        }

        private void Window_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (System.Windows.Forms.Control.IsKeyLocked(Keys.CapsLock))
            {
                CapsCheck.Visibility = Visibility.Visible;
            }
            else
            {
                // System.Windows.MessageBox.Show("The Caps Lock key is OFF.");
                CapsCheck.Visibility = Visibility.Collapsed;
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (System.Windows.Forms.Control.IsKeyLocked(Keys.CapsLock))
            {
                CapsCheck.Visibility = Visibility.Visible;
            }
            else
            {
                // System.Windows.MessageBox.Show("The Caps Lock key is OFF.");
                CapsCheck.Visibility = Visibility.Collapsed;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = (@"Data Source=COLIN\SQLMAIN;Initial Catalog=TestLoginCredentials;Persist Security Info=True;User ID=sa;Password=Thr33four");
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            if (LoginButton.Content.ToString() == "Log In")
            {
                //System.Windows.Forms.MessageBox.Show("Login");
                // new MainWindow().Show();
                try
                {
                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        sqlConnection.Open();
                        //    System.Windows.Forms.MessageBox.Show("Connection opened");
                    }
                    String query = "SELECT COUNT(1) FROM tblUser WHERE Username=@UserName AND Password=@Password";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlConnection);
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.Parameters.AddWithValue("@Username", UsernameInput.Text);
                    sqlCmd.Parameters.AddWithValue("@Password", PasswordInput.Password);
                    int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
                    //System.Windows.Forms.MessageBox.Show("Database communication successful.");
                    if (count == 1)
                    {
                        // System.Windows.Forms.MessageBox.Show("Username or password accepted.");
                        //Properties.Settings.Default["LoggedInUserName"] = UsernameInput.Text;
                        //Properties.Settings.Default.Save(); // Saves settings in application configuration file
                        MainWindow dashboard = new MainWindow();
                        dashboard.Show();
                        this.Close();
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Username or password was incorrect.");
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }
                finally
                {
                    sqlConnection.Close();
                    // this.Close();
                }
            }
            else if (LoginButton.Content.ToString() == "Register")
            {
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }
                try
                {
                    // If Author already exists, get ID, add AuthorID + BookID to BookAuthor table. Lol
                    // Otherwise add new AuthorID & Book ID to table.
                    string authorQuery = "insert into tblUser values (@UserName, @Password)";
                    SqlCommand sqlCommand = new SqlCommand(authorQuery, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@UserName", UsernameInput.Text);
                    sqlCommand.Parameters.AddWithValue("@Password", PasswordInput.Password);
                    sqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something went wrong..." + ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                    MessageBox.Show("Thanks for registering. You may now log in using the details you've provided.");
                    chkRegister.Visibility = Visibility.Hidden;
                    PasswordConfirm.Visibility = Visibility.Hidden;
                    ConfirmPassword.Visibility = Visibility.Hidden;
                    LoginButton.Content = "Log In";
                }
            }
        }
    }
}
