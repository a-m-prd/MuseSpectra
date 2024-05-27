using MuseSpectra.Data; // Import the MuseSpectra.Data namespace for database context
using MuseSpectra.Models; // Import the MuseSpectra.Models namespace for user model
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MuseSpectra.Views {
    public partial class RegisterWindow : Window {
        // DbContext for interacting with the database
        private AppDbContext _context;

        // Static property to hold the registered user
        public static User RegisteredUser;
        public RegisterWindow() {
            InitializeComponent();
            _context = new AppDbContext();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e) {
            // Retrieving registration data from input fields
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string email = EmailTextBox.Text.Trim();

            // Validating data from input fields
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email)) {
                MessageBox.Show("Please fill all the required fields.");
                return;
            }

            // Checking if the username already exists
            var userExists = _context.Users.Any(user => user.Username == username);
            if (userExists) {
                MessageBox.Show("Username already exists. Please choose a different one or proceed to login.");
                return;
            }

            // Checking if the email already exists
            var emailExists = _context.Users.Any(user => user.Email == email);
            if (emailExists) {
                MessageBox.Show("Email already exists. Please choose a different one or proceed to login.");
                return;
            }

            // Hash the password with BCrypt
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            // Creating a new user
            var newUser = new User {
                Username = username,
                PasswordHash = password,
                Email = email,
            };
            RegisteredUser = newUser;

            // Adding the new user to the database and saving changes
            _context.Users.Add(newUser);
            _context.SaveChanges();

            // Open visualization window
            MessageBox.Show($"Congratulations {username}! Your registration was successfull.");
            var visualizationWindow = new VisualizationWindow();
            visualizationWindow.Show();
            this.Close();
        }

        // Option to go back to login window
        private void RegisterBackButton_Click(object sender, RoutedEventArgs e) {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}