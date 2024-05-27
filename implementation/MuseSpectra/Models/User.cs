namespace MuseSpectra.Models {
    // Define the User class, representing a user entity in the application
    public class User {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string SelectedColor { get; set; }
        public string SelectedShape { get; set; }
        public string SelectedEffect { get; set; }
        public double SizeRange { get; set; }
    }
}