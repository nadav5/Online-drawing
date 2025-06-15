using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FirstApp
{
    public class Rectangle : Shape
    {
        public double WidthRect { get; set; }
        public double HeightRect { get; set; }

        public Rectangle(double width, double height) : base("Rectangle")
        {
            WidthRect = width;
            HeightRect = height;
        }

        public override UIElement CreateVisual()
        {
            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
            {
                Width = WidthRect,
                Height = HeightRect,
                Stroke = Brushes.DarkRed,
                Fill = Brushes.LightCoral
            };

            Canvas.SetLeft(rect, X);
            Canvas.SetTop(rect, Y);

            AttachDragEvents(rect);

            return rect;
        }
    }
}
