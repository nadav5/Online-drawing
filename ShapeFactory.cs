using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp
{
    public static class ShapeFactory
    {
        public static Shape CreateShape(string type)
        {
            return type switch
            {
                "Circle" => new Circle(0),
                "Rectangle" => new Rectangle(0, 0),
                "Line" => new Line(0, 0, 0, 0),
                _ => throw new NotSupportedException($"Unknown shape type: {type}")
            };
        }
    }
}