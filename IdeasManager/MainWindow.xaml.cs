using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IdeasManager
{
    // Things that need looked at:
    // The table needs to resize with the window
    // Colour scheme could do with being changable.
    // ADD OR UPDATE ENTRY, should be simple to do.

    //  Code for validating entry:

    // IF NOT EXISTS(SELECT 1 FROM Users WHERE FirstName = 'John' AND LastName = 'Smith')
    // BEGIN
    // INSERT INTO Users(FirstName, LastName) VALUES('John', 'Smith')
    // END

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool newNote = false;
        int currentNoteNumber = 0;

        Random r = new Random();

        readonly SqlConnection sqlConnection;

        public MainWindow()
        {
            InitializeComponent();
            Save_Button.IsEnabled = false;
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

            SqlCommand areThereAnyNotes = new SqlCommand("SELECT COUNT(*) FROM NoteData WHERE UserName = '" + Properties.Settings.Default.CurrentUserName + "'", sqlConnection);
            int numberOfNotes = (int)areThereAnyNotes.ExecuteScalar();

            if (numberOfNotes == 0)
            {
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("NoteListBG.png", UriKind.Relative));
                brush.TileMode = TileMode.None;
                brush.Stretch = Stretch.Uniform;
                NoteList.Background = brush;
                sqlConnection.Close();

            }

            // This may need work, 

            else
            {
                NoteList.Background = Brushes.PaleTurquoise;
            }

            try
            {
                string query = "SELECT * FROM NoteData where Username = '" + Properties.Settings.Default.CurrentUserName + "' ORDER BY NoteTitle";

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
            Save_Button.IsEnabled = true;
            newNote = false;

            if (sqlConnection.State == ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
            #region
            #endregion
            try
            {
                string query = "select Note from NoteData where NoteTitle = @NoteList; select NoteTitle from NoteData where NoteTitle = @NoteList";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            
                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@NoteList", NoteList.SelectedValue);
                    DataTable NotePad = new DataTable();
                    sqlDataAdapter.Fill(NotePad);
                    NoteName.Text = NotePad.Rows[0]["NoteTitle"].ToString();
                    DataTable NoteHeader = new DataTable();
                    sqlDataAdapter.Fill(NoteHeader);
                    NoteSpace.Text = NoteHeader.Rows[0]["Note"].ToString();
                }
            }
            //catch (Exception e2)
            //{
            //    // MessageBox.Show("Error: " + e2);
            //}
            finally
            {
                sqlConnection.Close();
            }
        }

        private void New_Button_Click(object sender, RoutedEventArgs e)
        {
            NoteName.Text = "New Title";
            NoteSpace.Text = "New Note";
            newNote = true;
            NoteSpace.Background = Brushes.LightGreen;
            Save_Button.Background = Brushes.Green;
            Save_Button.IsEnabled = true;
            Save_Button.Content = "Save";
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
                if (newNote)
                {
                    try
                    {
                        // Select from BLAH BLAH BLAH
                        // Do that
                        // If Result = > 0
                        // Check if you want to save
                        // Else create new... ???????????

                        string query = "SELECT * FROM NoteData WHERE NoteTitle = @NoteTitle; IF @@Rowcount = 0 INSERT INTO NoteData VALUES (@UserName, @NoteTitle, @Note)";
                        SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                        sqlConnection.Open();
                        sqlCommand.Parameters.AddWithValue("@UserName", Properties.Settings.Default.CurrentUserName);
                        sqlCommand.Parameters.AddWithValue("@NoteTitle", NoteName.Text);
                        sqlCommand.Parameters.AddWithValue("@Note", NoteSpace.Text);
                        sqlCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Note saving error: " + ex.ToString());
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
                    try
                    {
                        // This won't do anything but could come in useful... "Update NoteData Set NoteTitle = @NoteTitle, Note= @Note"

                        // I think I need to save the Note Id PK, then query & update that...(After checking with the user...)
                        string query = "UPDATE NoteData SET NoteTitle = @NoteTitle IF ";
                        SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                        sqlConnection.Open();
                        sqlCommand.Parameters.AddWithValue("@UserName", Properties.Settings.Default.CurrentUserName);
                        sqlCommand.Parameters.AddWithValue("@NoteTitle", NoteName.Text);
                        sqlCommand.Parameters.AddWithValue("@Note", NoteSpace.Text);
                        sqlCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Updating note error: " + ex.ToString());
                    }
                    finally
                    {
                        sqlConnection.Close();
                        NoteSpace.Background = Brushes.LightBlue;
                        Announcement.Text = "Idea saved! That sounds like a good one!";
                        ShowAllNotes();
                    }
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
                        ShowAllNotes();
                    }
                }
            }
            else
            {
                MessageBox.Show("Nothing selected.\nSelect an idea to delete.");
            }
        }

        private void NoteSpace_TextChanged(object sender, TextChangedEventArgs e)
        {
            string noteHeading = Properties.Settings.Default.CurrentUserName + "'s Notes";
            Notes.Content = noteHeading;
            //if (NoteName.Text.ToString() != "New Title" && NoteSpace.Text.ToString() != "New Note")
            //{
            //    Save_Button.IsEnabled = true;
            //}
            //else
            //{
            //    Save_Button.IsEnabled = false;
            //}
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
                //Save_Button.IsEnabled = false;
            }

            if (newNote)
            {
                Save_Button.Content = "Save";
            }
            else
            {
                Save_Button.Content = "Update Note";
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutPage aboutPage = new AboutPage();
            aboutPage.Show();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            ShowAllNotes();
        }
    }
}
