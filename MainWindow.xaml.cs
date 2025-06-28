using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FirstApp
{
    public partial class MainWindow : Window
    {
        private Sketch sketch = new Sketch();

        private string currentlyLoadedFile = null;


        public MainWindow()
        {
            InitializeComponent();


            Task.Run(() => new FirstApp.ServerUpload().StartAsync());
            Task.Run(() => new FirstApp.ServerDownload().StartAsync());
        }

        private void AddLine_Click(object sender, RoutedEventArgs e)
        {
            var line = new Line(10, 10, 150, 150);
            sketch.AddShape(line);
            //sketch.DrawAll(MyCanvas);
            sketch.DrawLast(MyCanvas);
        }

        private void AddRectangle_Click(object sender, RoutedEventArgs e)
        {
            var rect = new Rectangle(100, 60) { X = 50, Y = 50 };
            sketch.AddShape(rect);
            //sketch.DrawAll(MyCanvas);
            sketch.DrawLast(MyCanvas);
        }

        private void AddCircle_Click(object sender, RoutedEventArgs e)
        {
            var circle = new Circle(40) { X = 200, Y = 100 };
            sketch.AddShape(circle);
            //sketch.DrawAll(MyCanvas);
            sketch.DrawLast(MyCanvas);
        }

        private async void Clear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentlyLoadedFile))
                {
                    await ReleaseFileAsync(currentlyLoadedFile);
                    currentlyLoadedFile = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error releasing file:\n" + ex.Message);
            }

            sketch.Clear();
            MyCanvas.Children.Clear();
        }


        private async void Upload_Click(object sender, RoutedEventArgs e)
        {
            if (ServerWindow.TokenSource.IsCancellationRequested)
            {
                MessageBox.Show("The server is suspended. Upload is currently disabled.");
                return;
            }

            if (!string.IsNullOrEmpty(currentlyLoadedFile))
            {
                await ReleaseFileAsync(currentlyLoadedFile);
                currentlyLoadedFile = null;
            }


            string fileName = InputDialog.Show("Enter file name:");
            if (string.IsNullOrWhiteSpace(fileName))
            {
                MessageBox.Show("No file name provided. Upload canceled.");
                return;
            }

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "Sketches");
            string filePath = Path.Combine(folder, fileName + ".json");

            if (File.Exists(filePath))
            {
                MessageBox.Show("Try another name, that name is taken");
                return;
            }


            var shapes = sketch.GetShapes();
            string json = JsonConvert.SerializeObject(shapes, Formatting.Indented);

            string message = $"SAVE_AS:{fileName}\n{json}";

            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync("127.0.0.1", 5000);
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(message);
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                }
            }

            MessageBox.Show($"Sketch saved as {fileName}.json");
        }


        private async void Load_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> fileList;
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync("127.0.0.1", 5001);
                    using (NetworkStream stream = client.GetStream())
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        await writer.WriteLineAsync("GET_FILE_LIST");
                        string jsonList = await reader.ReadToEndAsync();
                        fileList = JsonConvert.DeserializeObject<List<string>>(jsonList);
                    }
                }

                string fileName = ShowSelectionDialog(fileList);

                if (!string.IsNullOrEmpty(currentlyLoadedFile))
                {
                    await ReleaseFileAsync(currentlyLoadedFile);
                    currentlyLoadedFile = null;
                }

                if (string.IsNullOrWhiteSpace(fileName))
                    return;

                currentlyLoadedFile = fileName;

                string fileContent;
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync("127.0.0.1", 5001);
                    using (NetworkStream stream = client.GetStream())
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        await writer.WriteLineAsync("GET_FILE:" + fileName);
                        fileContent = await reader.ReadToEndAsync();
                    }
                }






                var shapes = JsonConvert.DeserializeObject<List<Shape>>(fileContent, new ShapeConverter());
                sketch.Clear();
                sketch.AddShapes(shapes);
                sketch.DrawAll(MyCanvas);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Failed to load sketch another user is watching it now.\n");
            }
        }


        public static string ShowSelectionDialog(List<string> options)
        {
            Window window = new Window
            {
                Title = "Choose a sketch",
                Width = 300,
                Height = 250,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };



            var listBox = new ListBox { ItemsSource = options };
            var okButton = new Button
            {

                Content = "Load",
                Width = 60,
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            string selected = null;
            okButton.Click += (s, e) =>
            {
                selected = listBox.SelectedItem as string;
                window.Close();
            };

            var stack = new StackPanel();
            stack.Children.Add(listBox);
            stack.Children.Add(okButton);



            window.Content = stack;
            window.ShowDialog();
            return selected;
        }

        private async Task ReleaseFileAsync(string fileName)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync("127.0.0.1", 5001);
                    using (NetworkStream stream = client.GetStream())
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        await writer.WriteLineAsync("RELEASE_FILE:" + fileName);
                        string response = await reader.ReadLineAsync();

                        if (!response.StartsWith("OK"))
                        {
                            MessageBox.Show("Release failed:\n" + response);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Release error:\n" + ex.Message);
            }
        }


    }
}
