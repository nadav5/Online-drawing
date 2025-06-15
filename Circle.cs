using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FirstApp
{
    public class Circle : Shape
    {
        public double Radius { get; set; }

        public Circle(double radius) : base("Circle")
        {
            Radius = radius;
        }

        public override UIElement CreateVisual()
        {
            Ellipse ellipse = new Ellipse
            {
                Width = Radius * 2,
                Height = Radius * 2,
                Stroke = Brushes.Black,
                Fill = Brushes.LightBlue
            };

            Canvas.SetLeft(ellipse, X);
            Canvas.SetTop(ellipse, Y);

            AttachDragEvents(ellipse); 
            return ellipse;
        }
    }
}
