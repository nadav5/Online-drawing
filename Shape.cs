using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace FirstApp
{
    public abstract class Shape
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; } = DateTime.Now;
        public double X { get; set; }
        public double Y { get; set; }

        protected Shape(string name)
        {
            Name = name;
        }

        public abstract UIElement CreateVisual();

        protected void AttachDragEvents(UIElement element)
        {
            Point lastPosition = new Point();
            bool isDragging = false;

            element.MouseLeftButtonDown += (s, e) =>
            {
                var parent = VisualTreeHelper.GetParent(element) as UIElement;
                if (parent == null) return;

                lastPosition = e.GetPosition(parent);
                isDragging = true;
                element.CaptureMouse();
            };

            element.MouseMove += (s, e) =>
            {
                if (!isDragging) return;

                var parent = VisualTreeHelper.GetParent(element) as UIElement;
                if (parent == null) return;

                Point currentPosition = e.GetPosition(parent);
                double dx = currentPosition.X - lastPosition.X;
                double dy = currentPosition.Y - lastPosition.Y;

                if (element is System.Windows.Shapes.Line line)

                {
                    line.X1 += dx;
                    line.Y1 += dy;
                    line.X2 += dx;
                    line.Y2 += dy;

                    X = line.X1;
                    Y = line.Y1;

                    if (this is Line shapeLine)
                    {
                        shapeLine.EndX = line.X2;
                        shapeLine.EndY = line.Y2;
                    }
                }
                else if (element is FrameworkElement fe)
                {
                    X += dx;
                    Y += dy;
                    Canvas.SetLeft(fe, X);
                    Canvas.SetTop(fe, Y);
                }

                lastPosition = currentPosition;
            };

            element.MouseLeftButtonUp += (s, e) =>
            {
                isDragging = false;
                element.ReleaseMouseCapture();
            };
        }
    }
}
