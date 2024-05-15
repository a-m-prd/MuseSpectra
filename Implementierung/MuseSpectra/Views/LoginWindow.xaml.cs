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
using System.Windows.Shapes;
using MuseSpectra.Data;
using MuseSpectra.Models;

namespace MuseSpectra.Views {
    public partial class LoginWindow : Window {
        public static User LoggedInUser;

        public LoginWindow() {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            using (var context = new AppDbContext()) {
                var user = context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

                if (user != null) {
                    LoggedInUser = user;
                    MessageBox.Show($"Welcome {user.Username}!");
                    var visualizationWindow = new VisualizationWindow();
                    visualizationWindow.Show();
                    this.Close();
                }
                else {
                    MessageBox.Show("Invalid username or password.");
                }
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e) {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }
    }
}
