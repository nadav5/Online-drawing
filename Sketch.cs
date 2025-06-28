using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FirstApp
{
    public class Sketch
    {
        private readonly List<Shape> shapes = new List<Shape>();

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

        public IReadOnlyList<Shape> GetShapes()
        {
            return shapes.AsReadOnly();
        }

        public void AddShapes(IEnumerable<Shape> newShapes)
        {
            shapes.AddRange(newShapes);
        }

        public void DrawLast(Canvas canvas)
        {
            if (shapes.Count > 0)
            {
                var last = shapes[^1];
                UIElement visual = last.CreateVisual();
                canvas.Children.Add(visual);
            }
        }


    }
}
