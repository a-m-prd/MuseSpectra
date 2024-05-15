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
    public partial class RegisterWindow : Window {
        public RegisterWindow() {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e) {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string email = EmailTextBox.Text;

            using (var context = new AppDbContext()) {
                var user = new User {
                    Username = username,
                    Password = password,
                    Email = email,
                    SelectedColor = "Random",
                    SelectedShape = "Ellipse",
                    SizeRange = 50
                };

                context.Users.Add(user);
                context.SaveChanges();
            }

            MessageBox.Show($"Congratulations {username}! Your registration was successfull and you can now proceed to login.");
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
