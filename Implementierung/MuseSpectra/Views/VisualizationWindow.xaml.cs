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
using System.Windows.Threading;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using MuseSpectra.Data;
using MuseSpectra.Models;

namespace MuseSpectra.Views {
    public partial class VisualizationWindow : Window {
        private DispatcherTimer _timer;
        private Random _random;
        private WasapiLoopbackCapture _waveIn;
        private const int _bufferSize = 1024;
        private float[] _buffer;
        private int _bytesPerSample = sizeof(float); // Adjust for the size of each sample

        // Customizable parameters
        private Color _selectedColor = Colors.Transparent;
        private string _selectedShape = "Ellipse";
        private string _selectedEffect = "None";
        private double _sizeRange = 50;
        private float _threshold = 0.01f; // Threshold to determine if sound is present

        public VisualizationWindow() {
            InitializeComponent();
            _random = new Random();
            _buffer = new float[_bufferSize];
            LoadUserSettings();
            InitializeVisualization();
            InitializeAudio();
        }

        private void LoadUserSettings() {
            var user = LoginWindow.LoggedInUser;
            _selectedColor = ColorFromString(user.SelectedColor);
            _selectedShape = user.SelectedShape;
            _sizeRange = user.SizeRange;
            _selectedEffect = "None";

            // Set UI elements
            ColorComboBox.SelectedValue = user.SelectedColor;
            ShapeComboBox.SelectedValue = user.SelectedShape;
            SizeSlider.Value = user.SizeRange;
            EffectComboBox.SelectedValue = "None";
        }

        private Color ColorFromString(string color) {
            switch (color) {
                case "Red":
                    return Colors.Red;
                case "Green":
                    return Colors.Green;
                case "Blue":
                    return Colors.Blue;
                case "Dynamic":
                    return Colors.Transparent; // Dynamic color changes
                default:
                    return Colors.Transparent; // Random
            }
        }

        private void InitializeVisualization() {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(50); // Update every 50 milliseconds
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private void InitializeAudio() {
            try {
                _waveIn = new WasapiLoopbackCapture();
                _waveIn.DataAvailable += OnDataAvailable;
                _waveIn.RecordingStopped += OnRecordingStopped;
                _waveIn.StartRecording();
            }
            catch (Exception ex) {
                MessageBox.Show($"Error initializing audio input: {ex.Message}");
            }
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e) {
            int samplesRecorded = e.BytesRecorded / _bytesPerSample;
            int bufferSize = Math.Min(samplesRecorded, _buffer.Length);

            for (int i = 0; i < bufferSize; i++) {
                _buffer[i] = BitConverter.ToSingle(e.Buffer, i * _bytesPerSample);
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

                    Shape shape = CreateShape(magnitude);

                    if (_selectedColor == Colors.Transparent) {
                        shape.Fill = new SolidColorBrush(Color.FromRgb(
                            (byte)_random.Next(256),
                            (byte)_random.Next(256),
                            (byte)_random.Next(256)));
                    }
                    else if (_selectedColor == Colors.Transparent) {
                        shape.Fill = new SolidColorBrush(Color.FromRgb(
                            (byte)(_random.Next(256) * Math.Abs(_buffer[i])),
                            (byte)(_random.Next(256) * Math.Abs(_buffer[i])),
                            (byte)(_random.Next(256) * Math.Abs(_buffer[i]))));
                    }
                    else {
                        shape.Fill = new SolidColorBrush(_selectedColor);
                    }

                    Canvas.SetLeft(shape, _random.Next((int)VisualizationCanvas.ActualWidth));
                    Canvas.SetTop(shape, _random.Next((int)VisualizationCanvas.ActualHeight));
                    VisualizationCanvas.Children.Add(shape);
                }
            }
        }

        private Shape CreateShape(double size) {
            Shape shape;
            switch (_selectedShape) {
                case "Rectangle":
                    shape = new Rectangle {
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
            PointCollection points = new PointCollection
            {
                new Point(size / 2, 0),
                new Point(0, size),
                new Point(size, size)
            };
            return new Polygon { Points = points, Width = size, Height = size };
        }

        private Shape CreatePolygon(double size) {
            PointCollection points = new PointCollection();
            int numSides = _random.Next(5, 10); // Random number of sides between 5 and 10
            for (int i = 0; i < numSides; i++) {
                double angle = i * 2 * Math.PI / numSides;
                points.Add(new Point(size / 2 * (1 + Math.Cos(angle)), size / 2 * (1 - Math.Sin(angle))));
            }
            return new Polygon { Points = points, Width = size, Height = size };
        }

        private void ApplyEffect(Shape shape, double size) {
            switch (_selectedEffect) {
                case "Pulsate":
                    shape.RenderTransform = new ScaleTransform(1, 1);
                    var scaleAnimation = new System.Windows.Media.Animation.DoubleAnimation {
                        From = 1,
                        To = 1.5,
                        AutoReverse = true,
                        Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                        RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever
                    };
                    shape.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
                    shape.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
                    break;
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
                case "Gradient":
                    shape.Fill = new LinearGradientBrush(
                        Color.FromRgb((byte)_random.Next(256), (byte)_random.Next(256), (byte)_random.Next(256)),
                        Color.FromRgb((byte)_random.Next(256), (byte)_random.Next(256), (byte)_random.Next(256)),
                        45);
                    break;
            }
        }

        private void OnRecordingStopped(object sender, StoppedEventArgs e) {
            _waveIn.Dispose();
        }

        protected override void OnClosed(EventArgs e) {
            _waveIn.StopRecording();
            _waveIn.Dispose();
            base.OnClosed(e);
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e) {
            // Update selected color
            var selectedColorItem = ColorComboBox.SelectedItem as ComboBoxItem;
            _selectedColor = ColorFromString(selectedColorItem.Content.ToString());
            _selectedShape = (ShapeComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            _selectedEffect = (EffectComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            _sizeRange = SizeSlider.Value;

            // Save settings to user
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

        private void ToggleParameterBarButton_Checked(object sender, RoutedEventArgs e) {
            ParameterBar.Visibility = Visibility.Visible;
            ToggleParameterBarButton.Content = "Hide Parameters";
        }

        private void ToggleParameterBarButton_Unchecked(object sender, RoutedEventArgs e) {
            ParameterBar.Visibility = Visibility.Collapsed;
            ToggleParameterBarButton.Content = "Show Parameters";
        }
    }
}
