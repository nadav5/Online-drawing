using SketchServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace FirstApp
{
    public partial class ServerWindow : Window
    {
        private DispatcherTimer refreshTimer;
        private bool isSuspended = false;
        public static CancellationTokenSource TokenSource = new CancellationTokenSource();
        private readonly string SketchesFolder = Path.Combine(Directory.GetCurrentDirectory(), "Sketches");

        public ServerWindow()
        {
            InitializeComponent();

            var uploadServer = new ServerUpload();
            uploadServer.SketchUploaded += name =>
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(this, $"Sketch '{name}' uploaded to server.", "Server Notification");
                    RefreshSketchList();
                });
            };
            Task.Run(() => uploadServer.StartAsync());


            refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            refreshTimer.Tick += (s, e) => RefreshSketchList();
            refreshTimer.Start();

            RefreshSketchList();
        }

        private void RefreshSketchList()
        {
            Dispatcher.Invoke(() =>
            {
                SketchListPanel.Children.Clear();

                if (!Directory.Exists(SketchesFolder))
                    Directory.CreateDirectory(SketchesFolder);

                var files = Directory.GetFiles(SketchesFolder, "*.json");
                foreach (var file in files)
                {
                    string fileName = Path.GetFileName(file);

                    StackPanel row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 2, 0, 2) };

                    Label nameLabel = new Label
                    {
                        Content = fileName,
                        Width = 180
                    };

                    Button deleteButton = new Button
                    {
                        Content = "X",
                        Background = Brushes.Red,
                        Foreground = Brushes.White,
                        Width = 25,
                        Height = 20,
                        Tag = file
                    };

                    deleteButton.Click += DeleteSketch_Click;

                    row.Children.Add(nameLabel);
                    row.Children.Add(deleteButton);
                    SketchListPanel.Children.Add(row);
                }
            });
        }

        private void DeleteSketch_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string path && File.Exists(path))
            {
                File.Delete(path);
                RefreshSketchList();
            }
        }


        private void SuspendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isSuspended)
            {
                TokenSource.Cancel();
                StatusText.Text = "Server suspended.";
                StatusText.Foreground = Brushes.Red;
                SuspendButton.Content = "Resume";
                isSuspended = true;
            }
            else
            {
                TokenSource = new CancellationTokenSource();
                StatusText.Text = "Server running...";
                StatusText.Foreground = Brushes.Green;
                SuspendButton.Content = "Suspend";
                isSuspended = false;
            }
        }
    }
}
