using System.Windows;

namespace FirstApp
{
    public static class InputDialog
    {
        public static string Show(string prompt)
        {
            Window window = new Window
            {
                Title = prompt,
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            var stack = new System.Windows.Controls.StackPanel { Margin = new Thickness(10) };
            var textBox = new System.Windows.Controls.TextBox();
            var okButton = new System.Windows.Controls.Button
            {
                Content = "OK",
                IsDefault = true,
                Width = 60,
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            string result = null;
            okButton.Click += (s, e) =>
            {
                result = textBox.Text;
                window.Close();
            };

            stack.Children.Add(textBox);
            stack.Children.Add(okButton);
            window.Content = stack;
            window.ShowDialog();
            return result;
        }
    }
}

