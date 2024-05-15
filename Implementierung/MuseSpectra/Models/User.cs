using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseSpectra.Models {
    public class User {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string SelectedColor { get; set; }
        public string SelectedShape { get; set; }
        public double SizeRange { get; set; }
    }
}
