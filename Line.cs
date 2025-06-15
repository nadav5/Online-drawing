using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FirstApp
{
    public class Line : Shape
    {
        public double EndX { get; set; }

        public double EndY { get; set; }



        public Line(double startX, double startY, double endX, double endY) : base("Line")
        {
            X = startX;
            Y = startY;
            EndX = endX;
            EndY = endY;

        }

        public override UIElement CreateVisual()
        {
            System.Windows.Shapes.Line line = new System.Windows.Shapes.Line
            {
                X1 = X,
                Y1 = Y,

                X2 = EndX,
                Y2 = EndY,
                Stroke = Brushes.Green,
                StrokeThickness = 4
            };

            AttachDragEvents(line); 

            return line;
        }
    }
}
