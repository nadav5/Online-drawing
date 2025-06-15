using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FirstApp
{
    public class Sketch
    {
        private List<Shape> shapes = new List<Shape>();

        public void AddShape(Shape shape)
        {
            shapes.Add(shape);
        }

        public void DrawAll(Canvas canvas)
        {
            canvas.Children.Clear(); 
            foreach (var shape in shapes)
            {
                UIElement visual = shape.CreateVisual();
                canvas.Children.Add(visual);
            }
        }

        public void Clear()
        {
            shapes.Clear();
        }

        public List<Shape> GetShapes()
        {
            return shapes;
        }
    }
}
