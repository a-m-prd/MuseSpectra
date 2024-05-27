using MuseSpectra.Data; // Import the MuseSpectra.Data namespace for database context
using MuseSpectra.Models; // Import the MuseSpectra.Models namespace for user model
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MuseSpectra.Views {
    public partial class LoginWindow : Window {
        // Static property to hold the logged-in user
        public static User LoggedInUser;

        public LoginWindow() {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) {
            // Retrieve the username and password entered by the user
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            // Use a database context to interact with the database
            using (var context = new AppDbContext()) {
                // Query the Users table to find a user with matching username and password hash
                var user = context.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);

                if (user != null) {
                    // Set the logged-in user
                    LoggedInUser = user;
                    MessageBox.Show($"Welcome {user.Username}!");
                    // Open the visualization window and close login window
                    var visualizationWindow = new VisualizationWindow();
                    visualizationWindow.Show();
                    this.Close();
                }
                else if (username == null || password == null) {
                    // If one of the fields is empty, display an error message
                    MessageBox.Show("Please fill all the required fields.");
                }
                else {
                    // If no matching user is found, display an error message
                    MessageBox.Show("Invalid username or password.");
                }
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e) {
            // Open the registration window and close login window
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }
    }
}