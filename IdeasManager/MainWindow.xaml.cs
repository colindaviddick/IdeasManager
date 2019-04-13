using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IdeasManager
{
    // Things that need looked at:
    // I need to be able to delete entries
    // The table needs to resize with the window
    // Colour scheme could do with being changable.
    // Auto select the Title when the New Note button is hit.
    // Find out how to create new lines in the textbox. DONE

    //  Code for validating entry:

    // IF NOT EXISTS(SELECT 1 FROM Users WHERE FirstName = 'John' AND LastName = 'Smith')
    // BEGIN
    // INSERT INTO Users(FirstName, LastName) VALUES('John', 'Smith')
    // END

        // Settings.Default["SomeProperty"] = "Some Value";
        // Settings.Default.Save(); // Saves settings in application configuration file

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random r = new Random();

        readonly SqlConnection sqlConnection;

        public MainWindow()
        {
            InitializeComponent();

            string connectionString = (@"Data Source=COLIN\SQLMAIN;Initial Catalog=TestLoginCredentials;Persist Security Info=True;User ID=sa;Password=Thr33four");
            sqlConnection = new SqlConnection(connectionString);
            ShowAllNotes();
        }

        private void ShowAllNotes()
        {
            if (sqlConnection.State == ConnectionState.Closed)
            {
                sqlConnection.Open();
            }

            try
            {
                string query = "SELECT * FROM NoteData ORDER BY NoteTitle";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@NoteTitle", NoteName.SelectedText);
                    var NoteData = new DataTable();
                    sqlDataAdapter.Fill(NoteData);

                    NoteList.DisplayMemberPath = "NoteTitle";
                    NoteList.SelectedValuePath = "NoteTitle";
                    NoteList.ItemsSource = NoteData.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private void NoteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NoteSpace.Background = Brushes.LightBlue;
            Announcement.Text = "Blue sky thinking... This idea has been locked away for later.";

            Brush brush = new SolidColorBrush(Color.FromRgb((byte)r.Next(180, 230), (byte)r.Next(180, 230), (byte)r.Next(180, 230)));
            NoteList.Background = brush;


            if (sqlConnection.State == ConnectionState.Closed)
            {
                sqlConnection.Open();
            }

            try
            {
                string query = "select Note from NoteData where NoteTitle = @NoteList";
                string query2 = "select NoteTitle from NoteData where NoteTitle = @NoteList";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlCommand sqlCommand2 = new SqlCommand(query2, sqlConnection);
                // the SqlDataAdapter can be imagined like an Interface to make Tables usable by C#-Objects
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                SqlDataAdapter sqlDataAdapter2 = new SqlDataAdapter(sqlCommand2);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@NoteList", NoteList.SelectedValue);

                    DataTable NotePad = new DataTable();

                    sqlDataAdapter.Fill(NotePad);

                    NoteSpace.Text = NotePad.Rows[0]["Note"].ToString();
                }
                using (sqlDataAdapter2)
                {
                    sqlCommand2.Parameters.AddWithValue("@NoteList", NoteList.SelectedValue);

                    DataTable NotePad = new DataTable();

                    sqlDataAdapter2.Fill(NotePad);

                    NoteName.Text = NotePad.Rows[0]["NoteTitle"].ToString();
                }
            }
            catch (Exception e2)
            {
                // MessageBox.Show("Error: " + e2);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private void New_Button_Click(object sender, RoutedEventArgs e)
        {
            NoteName.Text = "New Title";
            NoteSpace.Text = "New Note";
            NoteSpace.Background = Brushes.LightGreen;
            Save_Button.Background = Brushes.Green;
            Announcement.Text = "The background has turned green to denote a new Idea, remember to save it if you want it to be kept.";
            NoteName.Focus();
            NoteName.SelectAll();
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            if (NoteName.Text == "New Title" && NoteSpace.Text == "New Note")
            {
                MessageBox.Show("Worst. List. Ever. \nThat's seriously not worth saving.\n\nTry adding a title and some... Actual ideas to your list before trying to save.");
            }
            else if (NoteName.Text == "New Title")
            {
                MessageBox.Show("Boring list. Change the title.");
            }
            else if (NoteSpace.Text == "New Note")
            {
                MessageBox.Show("Give your idea some content.");
            }
            else if (NoteName.Text != "" && NoteSpace.Text != "")
            {
                try
                {
                    // Select from BLAH BLAH BLAH
                    // Do that
                    // If Result = > 0
                    // Check if you want to save
                    // Else create new... ???????????

                    string query = "SELECT FROM NoteData WHERE NoteTitle = @NoteTitle; IF @@Rowcount = 0 INSERT INTO NoteData VALUES (@UserName, @NoteTitle, @Note)";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("UserName", "Colin");
                    sqlCommand.Parameters.AddWithValue("@NoteTitle", NoteName.Text);
                    sqlCommand.Parameters.AddWithValue("@Note", NoteSpace.Text);
                    sqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("AddAnimal" + ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                    NoteSpace.Background = Brushes.LightBlue;
                    Announcement.Text = "Idea saved! That sounds like a good one!";
                    ShowAllNotes();
                }
            }
            else
            {
                MessageBox.Show("Check that both the Note Title and Note boxes have some content before you try to save your note!");
            }
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {

            if (NoteList.SelectedValue != null)
            {
                if (MessageBox.Show("Are you sure you want to delete this idea?\n", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {

                }
                else
                {
                    try
                    {
                        string query = "delete from NoteData where NoteTitle = @NoteList";
                        SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                        sqlConnection.Open();
                        sqlCommand.Parameters.AddWithValue("@NoteList", NoteList.SelectedValue);
                        sqlCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("Deleting note error:" + ex.ToString());
                    }
                    finally
                    {
                        sqlConnection.Close();
                        NoteList.SelectedIndex--;
                        //New_Button_Click(sender, e);
                        ShowAllNotes();
                    }
                }
            }
            else
            {
                MessageBox.Show("Nothing selected.\nSelect an idea to delete.");
            }
        }

        //Start of code to change window colour theme.
        private void ColourSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxItem.IsSelectedProperty.ToString().Equals("Aqua"))
            {
                IdeasWindow.Background = Brushes.Aqua;
                NoteList.Background = Brushes.Aquamarine;
                New.Background = Brushes.Crimson;
                Delete.Background = Brushes.Green;
            }
            else if (ComboBoxItem.IsSelectedProperty.Equals("Cool White"))
            {
                IdeasWindow.Background = Brushes.Fuchsia;
                NoteList.Background = Brushes.Orange;
                New.Background = Brushes.Crimson;
                Delete.Background = Brushes.Green;
            }
            else if (ComboBoxItem.IsSelectedProperty.Equals("Nightmode"))
            {
                IdeasWindow.Background = Brushes.Green;
                NoteList.Background = Brushes.Purple;
                New.Background = Brushes.Crimson;
                Delete.Background = Brushes.Green;
            }
            else if (ComboBoxItem.IsSelectedProperty.Equals("Default"))
            {
                IdeasWindow.Background = Brushes.Red;
                NoteList.Background = Brushes.DarkCyan;
                New.Background = Brushes.Crimson;
                Delete.Background = Brushes.Green;
            }
        }

        private void NoteSpace_TextChanged(object sender, TextChangedEventArgs e)
        {
            string user = Properties.Settings.Default.LoggedInUserName;
            Announcement.Text = user;

            //if(NoteName.Text.ToString() != "New Title" && NoteSpace.Text.ToString() != "New Note")
            //{
            //    Save_Button.IsEnabled = true;
            //}
            //else
            //{
            //    Save_Button.IsEnabled = false;
            //}
        }

        private void Save_Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void New_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Announcement.Text = "Click here when you've conjured up another award winning idea!";
        }

        private void Delete_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Announcement.Text = "Not all ideas are winners, click here if your idea has run its course.";
        }

        private void NoteName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NoteName.Text == "New Title" || NoteSpace.Text == "New Note")
            {
                Announcement.Text = "You need to change the title and content of your idea before you can save.";
                //  Save_Button.IsEnabled = false;
            }

            if (NoteName.Text != "New Title" && NoteSpace.Text != "New Note")
            {
                // Save_Button.IsEnabled = true;
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutPage aboutPage = new AboutPage();
            aboutPage.Show();
        }
    }
}
