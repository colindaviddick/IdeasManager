using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
            UsernameInput.Focus();
            UsernameInput.Text = Properties.Settings.Default.CurrentUserName;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            LoginButton.Content = "users Register";
            PasswordInput.Background = Brushes.LightPink;
            PasswordConfirm.Visibility = Visibility.Visible;
            ConfirmPassword.Visibility = Visibility.Visible;
            LoginButton.Background = Brushes.LightPink;
            UsernameHeading.Content = "User";
            PasswordHeading.Content = "unlock";
            LoginButton.FontWeight = FontWeights.Bold;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            LoginButton.Content = "users Log In";
            PasswordInput.Background = Brushes.GhostWhite;
            PasswordConfirm.Visibility = Visibility.Collapsed;
            ConfirmPassword.Visibility = Visibility.Collapsed;
            LoginButton.Background = Brushes.GhostWhite;
            UsernameHeading.Content = "User";
            PasswordHeading.Content = "unlock";
            LoginButton.FontWeight = FontWeights.Normal;
        }

        private void Window_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (System.Windows.Forms.Control.IsKeyLocked(Keys.CapsLock))
            {
                CapsCheck.Visibility = Visibility.Visible;
                Jim.Background = Brushes.Red;
                Bob.Background = Brushes.Green;
            }
            else
            {
                // System.Windows.MessageBox.Show("The Caps Lock key is OFF.");
                CapsCheck.Visibility = Visibility.Collapsed;
                Jim.Background = Brushes.DarkTurquoise;
                Bob.Background = Brushes.PaleVioletRed;
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                if (UsernameInput.IsFocused)
                {
                    PasswordInput.Focus();
                }
                else if (PasswordInput.IsFocused)
                {
                    LoginButton.Focus();
                    SendKeys.SendWait("{Enter}");
                }
            }

            else
            {
                if (System.Windows.Forms.Control.IsKeyLocked(Keys.CapsLock))
                {
                    CapsCheck.Visibility = Visibility.Visible;
                    Jim.Background = Brushes.Red;
                    Bob.Background = Brushes.Green;
                }
                else
                {
                    // System.Windows.MessageBox.Show("The Caps Lock key is OFF.");
                    CapsCheck.Visibility = Visibility.Collapsed;
                    Jim.Background = Brushes.DarkTurquoise;
                    Bob.Background = Brushes.PaleVioletRed;
                }
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = (@"Data Source=COLIN\SQLMAIN;Initial Catalog=TestLoginCredentials;Persist Security Info=True;User ID=sa;Password=Thr33four");
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            if (LoginButton.Content.ToString() == "users Log In")
            {
                try
                {
                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        sqlConnection.Open();
                    }

                    String query = "SELECT COUNT(1) FROM tblUser WHERE Username=@UserName AND Password=@Password";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlConnection);
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.Parameters.AddWithValue("@Username", UsernameInput.Text);
                    sqlCmd.Parameters.AddWithValue("@Password", PasswordInput.Password);
                    int count = Convert.ToInt32(sqlCmd.ExecuteScalar());

                    if (count == 1)
                    {
                        Properties.Settings.Default.CurrentUserName = UsernameInput.Text;
                        Properties.Settings.Default.Save();
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
                }
            }
            else if (LoginButton.Content.ToString() == "users Register")
            {
                if (PasswordInput.Password != PasswordConfirm.Password)
                {
                    MessageBox.Show("Passwords do not match.");
                }

                else
                {
                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        sqlConnection.Open();
                    }

                    SqlCommand check_User_Name = new SqlCommand("SELECT COUNT(*) FROM tblUser WHERE ([UserName] = @UserName)", sqlConnection);
                    check_User_Name.Parameters.AddWithValue("@UserName", UsernameInput.Text);
                    int UserExist = (int)check_User_Name.ExecuteScalar();

                    if (UserExist > 0)
                    {
                        MessageBox.Show("This username already exists.\nPlease use a different username.");
                    }
                    else
                    {
                        if (sqlConnection.State == ConnectionState.Closed)
                        {
                            sqlConnection.Open();
                        }
                        try
                        {

                            string createNewUserQuery = "insert into tblUser values (@UserName, @Password); insert into NoteData values (@UserName, @NoteTitle, @Note)";
                            //string createNewUserTableQuery = "insert into NoteData values (@UserName, @NoteTitle, @Note)";
                            SqlCommand sqlCommand = new SqlCommand(createNewUserQuery, sqlConnection);
                            //SqlCommand sqlCommand2 = new SqlCommand(createNewUserTableQuery, sqlConnection);
                            sqlCommand.Parameters.AddWithValue("@UserName", UsernameInput.Text);
                            sqlCommand.Parameters.AddWithValue("@Password", PasswordInput.Password);
                            //sqlCommand.Parameters.AddWithValue("@UserName", UsernameInput.Text);
                            sqlCommand.Parameters.AddWithValue("@NoteTitle", "Example Note");
                            sqlCommand.Parameters.AddWithValue("@Note", "This is an example note, write all your ideas in here, then use the save button below. Your notes will appear in the menu to the left.");
                            sqlCommand.ExecuteScalar();
                            //sqlCommand2.ExecuteScalar();
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
                            LoginButton.Content = "users Log In";
                        }
                    }
                }
            }
        }

        //private void UsernameInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    if (e.Key == Key.Return || e.Key == Key.Enter)
        //    {
        //        PasswordInput.Focus();
        //    }
        //}

        //private void PasswordConfirm_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    if (e.Key == Key.Return || e.Key == Key.Enter)
        //    {

        //        // Doesn't work... Problem due to duplication of sender & e?
        //        MessageBox.Show("lajflasj");
        //        //LoginButton_Click();
        //    }
        //}
    }
}