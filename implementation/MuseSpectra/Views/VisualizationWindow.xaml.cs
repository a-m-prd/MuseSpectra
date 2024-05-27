using MuseSpectra.Data; // Import the MuseSpectra.Data namespace for database context
using NAudio.Wave; // Import the NAudio.Wave namespace for audio capturing
using System;
using System.Drawing; // Import the System.Drawing namespace for drawing (used by NotifyIcon)
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms; // Import the System.Windows.Forms namespace for the NotifyIcon
using System.Windows.Input; // Import the System.Windows.Input namespace for input handling
using System.Windows.Media; // Import the System.Windows.Media namespace for media handling
using System.Windows.Shapes; // Import the System.Windows.Shapes namespace for shape drawing
using System.Windows.Threading; // Import the System.Windows.Threading namespace for the dispatcher timer

namespace MuseSpectra.Views {
    public partial class VisualizationWindow : Window {
        // NotifyIcon for system tray interactions
        private NotifyIcon _notifyIcon;
        private System.Windows.Forms.ContextMenu _contextMenu;

        // Variables for fullscreen mode management
        private bool _isFullscreen = false;
        private WindowState _previousWindowState;
        private WindowStyle _previousWindowStyle;
        private ResizeMode _previousResizeMode;
        private Rect _previousWindowRect;

        // Timer for periodic updates
        private DispatcherTimer _timer;
        private Random _random;
        private WasapiLoopbackCapture _waveIn;
        private const int _bufferSize = 1024;
        private float[] _buffer;
        private int _bytesPerSample = sizeof(float); // Size of each audio sample

        // Customizable parameters
        private System.Windows.Media.Color _selectedColor;
        private string _selectedShape;
        private string _selectedEffect;
        private double _sizeRange;
        private float _threshold = 0.01f; // Threshold to determine if sound is present

        public VisualizationWindow() {
            InitializeComponent();
            LoadUserSettings();
            InitializeNotifyIcon();
            _random = new Random(); // Initialize the random number generator
            _buffer = new float[_bufferSize]; // Initialize the audio buffer
            InitializeVisualization();
            InitializeAudio();
        }

        private void InitializeNotifyIcon() {
            // Set the icon for the NotifyIcon
            _notifyIcon = new NotifyIcon {
                Icon = new Icon("icon.ico"),
                Visible = true,
                Text = "MuseSpectra"
            };

            _notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            // Create and configure the context menu for the NotifyIcon
            _contextMenu = new System.Windows.Forms.ContextMenu();
            _contextMenu.MenuItems.Add("Open", Open_Click);
            _contextMenu.MenuItems.Add("Show Parameters", ToggleParameterBar_Click);
            _contextMenu.MenuItems.Add("Settings", Settings_Click);
            _contextMenu.MenuItems.Add("Exit", Exit_Click);

            _notifyIcon.ContextMenu = _contextMenu;
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e) {
            // Open visualization window
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        private void Open_Click(object sender, EventArgs e) {
            // Open visualization window
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        private void ToggleParameterBar_Click(object sender, EventArgs e) {
            // Toggle the visibility of the parameter bar
            if (ParameterBar.Visibility == Visibility.Visible) {
                ParameterBar.Visibility = Visibility.Collapsed;
                ((System.Windows.Forms.MenuItem)sender).Text = "Show Parameters";
            }
            else {
                ParameterBar.Visibility = Visibility.Visible;
                ((System.Windows.Forms.MenuItem)sender).Text = "Hide Parameters";
            }
        }

        private void Settings_Click(object sender, EventArgs e) {
            // Open settings window
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        private void Exit_Click(object sender, EventArgs e) {
            // Shutdown the application
            _notifyIcon.Visible = false;
            System.Windows.Application.Current.Shutdown();
        }

        protected override void OnStateChanged(EventArgs e) {
            // Hide the window when minimized
            base.OnStateChanged(e);
            if (WindowState == WindowState.Minimized) {
                this.Hide();
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            // Toggle fullscreen mode when F11 is pressed
            if (e.Key == Key.F11) {
                ToggleFullscreen();
            }
        }

        private void ToggleFullscreen() {
            if (_isFullscreen) {
                // Exit fullscreen
                WindowStyle = _previousWindowStyle;
                WindowState = _previousWindowState;
                ResizeMode = _previousResizeMode;
                Left = _previousWindowRect.Left;
                Top = _previousWindowRect.Top;
                Width = _previousWindowRect.Width;
                Height = _previousWindowRect.Height;

                _isFullscreen = false; // Update the fullscreen flag
            }
            else {
                // Enter fullscreen
                _previousWindowState = WindowState;
                _previousWindowStyle = WindowStyle;
                _previousResizeMode = ResizeMode;
                _previousWindowRect = new Rect(Left, Top, Width, Height);

                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Normal;
                ResizeMode = ResizeMode.NoResize;
                Left = 0;
                Top = 0;
                Width = SystemParameters.PrimaryScreenWidth;
                Height = SystemParameters.PrimaryScreenHeight;

                _isFullscreen = true; // Update the fullscreen flag
            }
        }

        private void LoadUserSettings() {
            if (LoginWindow.LoggedInUser != null) {
                // Get user from login window
                var user = LoginWindow.LoggedInUser;
                _selectedColor = ColorFromString(user.SelectedColor);
                _selectedShape = user.SelectedShape;
                _sizeRange = user.SizeRange;
                _selectedEffect = user.SelectedEffect;

                // Set UI elements
                ColorComboBox.SelectedValue = _selectedColor;
                ShapeComboBox.SelectedValue = _selectedShape;
                SizeSlider.Value = _sizeRange;
                EffectComboBox.SelectedValue = _selectedEffect;
            }
            else {
                // Get user from registration window
                var user = RegisterWindow.RegisteredUser;
                _selectedColor = ColorFromString(user.SelectedColor);
                _selectedShape = user.SelectedShape;
                _sizeRange = user.SizeRange;
                _selectedEffect = "None";

                // Set UI elements
                ColorComboBox.SelectedValue = _selectedColor;
                ShapeComboBox.SelectedValue = _selectedShape;
                SizeSlider.Value = _sizeRange;
                EffectComboBox.SelectedValue = _selectedEffect;

                // Display first time message
                System.Windows.MessageBox.Show($"" +
                    $"Hello {user.Username}!\n" +
                    $"Since this is your first time logging in, here a few tips to get started:\n" +
                    $"- sound is automatically visualized by playing anything\n" +
                    $"- if the parameter bar is gone you can bring it back in the notifications bar\n" +
                    $"- press F11 to enter/exit fullscreen mode\n" +
                    $"I hope you have a great experience!");
            }
        }

        // Method to convert a string representation of a color to a System.Windows.Media.Color
        private System.Windows.Media.Color ColorFromString(string color) {
            switch (color) {
                case "Red":
                    return Colors.Red;
                case "Green":
                    return Colors.Green;
                case "Blue":
                    return Colors.Blue;
                case "Dynamic":
                    return Colors.Transparent;
                default:
                    return Colors.Transparent;
            }
        }

        private void InitializeVisualization() {
            _timer = new DispatcherTimer(); // Create a new DispatcherTimer instance
            _timer.Interval = TimeSpan.FromMilliseconds(50); // Set the timer interval to 50 milliseconds
            _timer.Tick += OnTimerTick; // Subscribe to the Tick event
            _timer.Start(); // Start the timer
        }

        private void InitializeAudio() {
            try {
                _waveIn = new WasapiLoopbackCapture(); // Create a new WasapiLoopbackCapture instance for capturing audio
                _waveIn.DataAvailable += OnDataAvailable; // Subscribe to the DataAvailable event
                _waveIn.RecordingStopped += OnRecordingStopped; // Subscribe to the RecordingStopped event
                _waveIn.StartRecording(); // Start audio recording
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show($"Error initializing audio input: {ex.Message}");
            }
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e) {
            int samplesRecorded = e.BytesRecorded / _bytesPerSample; // Calculate the number of samples recorded
            int bufferSize = Math.Min(samplesRecorded, _buffer.Length); // Determine the buffer size to process

            for (int i = 0; i < bufferSize; i++) {
                _buffer[i] = BitConverter.ToSingle(e.Buffer, i * _bytesPerSample); // Convert byte data to float and store in buffer
            }
        }

        private void OnTimerTick(object sender, EventArgs e) {
            // Clear the canvas
            VisualizationCanvas.Children.Clear();

            // Check if any audio level exceeds the threshold
            bool soundDetected = false;
            for (int i = 0; i < _buffer.Length; i++) {
                if (Math.Abs(_buffer[i]) > _threshold) {
                    soundDetected = true;
                    break;
                }
            }

            // Generate visual effect based on audio data only if sound is detected
            if (soundDetected) {
                for (int i = 0; i < _bufferSize; i += 32) {
                    float magnitude = (float)(Math.Abs(_buffer[i]) * _sizeRange);
                    if (magnitude < 1) magnitude = 1; // Avoid zero size shapes

                    Shape shape = CreateShape(magnitude); // Create shape based on magnitude

                    // Set shape fill color based on user settings
                    if (_selectedColor == Colors.Transparent) {
                        shape.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(
                            (byte)_random.Next(256),
                            (byte)_random.Next(256),
                            (byte)_random.Next(256)));
                    }
                    else {
                        shape.Fill = new SolidColorBrush(_selectedColor);
                    }

                    // Set shape position randomly on the canvas
                    Canvas.SetLeft(shape, _random.Next((int)VisualizationCanvas.ActualWidth));
                    Canvas.SetTop(shape, _random.Next((int)VisualizationCanvas.ActualHeight));
                    VisualizationCanvas.Children.Add(shape);
                }
            }
        }

        private Shape CreateShape(double size) {
            // Generate shapes based on user settings
            Shape shape;
            switch (_selectedShape) {
                case "Rectangle":
                    // Create shape rectangle
                    shape = new System.Windows.Shapes.Rectangle {
                        Width = size,
                        Height = size
                    };
                    break;
                case "Triangle":
                    shape = CreateTriangle(size);
                    break;
                case "Polygon":
                    shape = CreatePolygon(size);
                    break;
                default:
                    shape = new Ellipse {
                        Width = size,
                        Height = size
                    };
                    break;
            }
            return shape;
        }

        private Shape CreateTriangle(double size) {
            // Create shape triangle
            PointCollection points = new PointCollection {
                new System.Windows.Point(size / 2, 0),
                new System.Windows.Point(0, size),
                new System.Windows.Point(size, size)
            };
            return new Polygon { Points = points, Width = size, Height = size };
        }

        private Shape CreatePolygon(double size) {
            // Create shape polygon
            PointCollection points = new PointCollection();
            int numSides = _random.Next(5, 10); // Random number of sides between 5 and 10
            for (int i = 0; i < numSides; i++) {
                double angle = i * 2 * Math.PI / numSides;
                points.Add(new System.Windows.Point(size / 2 * (1 + Math.Cos(angle)), size / 2 * (1 - Math.Sin(angle))));
            }
            return new Polygon { Points = points, Width = size, Height = size };
        }

        private void ApplyEffect(Shape shape, double size) {
            // Generate effects based on user settings
            switch (_selectedEffect) {
                // Pulsate animation
                case "Pulsate":
                    shape.RenderTransform = new ScaleTransform(1, 1);
                    var scaleAnimation = new System.Windows.Media.Animation.DoubleAnimation {
                        From = 1,
                        To = 2,
                        AutoReverse = true,
                        Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                        RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever
                    };
                    shape.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
                    shape.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
                    break;
                // Move animation
                case "Move":
                    var moveAnimation = new System.Windows.Media.Animation.DoubleAnimation {
                        From = Canvas.GetLeft(shape),
                        To = Canvas.GetLeft(shape) + _random.NextDouble() * 100 - 50,
                        Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                        AutoReverse = true,
                        RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever
                    };
                    shape.BeginAnimation(Canvas.LeftProperty, moveAnimation);
                    break;
                // Gradient animation
                case "Gradient":
                    shape.Fill = new LinearGradientBrush(
                        System.Windows.Media.Color.FromRgb((byte)_random.Next(256), (byte)_random.Next(256), (byte)_random.Next(256)),
                        System.Windows.Media.Color.FromRgb((byte)_random.Next(256), (byte)_random.Next(256), (byte)_random.Next(256)),
                        45);
                    break;
            }
        }

        private void OnRecordingStopped(object sender, StoppedEventArgs e) {
            _waveIn.Dispose(); // Dispose of the audio capture device
        }

        protected override void OnClosed(EventArgs e) {
            _waveIn.StopRecording();
            _waveIn.Dispose();
            base.OnClosed(e);
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e) {
            // Update selected color
            var selectedColorItem = ColorComboBox.SelectedItem as ComboBoxItem;
            if (selectedColorItem == null) {
                System.Windows.MessageBox.Show("Please select a color.");
                return;
            }
            _selectedColor = ColorFromString(selectedColorItem.Content.ToString());

            // Update selected shape
            var selectedShapeItem = ShapeComboBox.SelectedItem as ComboBoxItem;
            if (selectedShapeItem == null) {
                System.Windows.MessageBox.Show("Please select a shape.");
                return;
            }
            _selectedShape = selectedShapeItem.Content.ToString();

            // Update selected effect
            var selectedEffectItem = EffectComboBox.SelectedItem as ComboBoxItem;
            if (selectedEffectItem == null) {
                System.Windows.MessageBox.Show("Please select an effect.");
                return;
            }
            _selectedEffect = selectedEffectItem.Content.ToString();

            _sizeRange = SizeSlider.Value;

            // Save settings to user
            if (LoginWindow.LoggedInUser != null) {
                var user = LoginWindow.LoggedInUser;
                user.SelectedColor = selectedColorItem.Content.ToString();
                user.SelectedShape = _selectedShape;
                user.SizeRange = _sizeRange;

                using (var context = new AppDbContext()) {
                    context.Users.Attach(user);
                    context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            else {
                var user = RegisterWindow.RegisteredUser;
                user.SelectedColor = selectedColorItem.Content.ToString();
                user.SelectedShape = _selectedShape;
                user.SizeRange = _sizeRange;

                using (var context = new AppDbContext()) {
                    context.Users.Attach(user);
                    context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }

        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e) {
            // Open settings window
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void ToggleParameterBarButton_Checked(object sender, RoutedEventArgs e) {
            // Show parameter bar
            ParameterBar.Visibility = Visibility.Visible;
            ToggleParameterBarButton.Content = "Hide Parameters";
        }

        private void ToggleParameterBarButton_Unchecked(object sender, RoutedEventArgs e) {
            // Hide parameter bar
            ParameterBar.Visibility = Visibility.Collapsed;
            ToggleParameterBarButton.Content = "Show Parameters";
        }
    }
}