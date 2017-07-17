using System;

namespace Routing
{
    public class Node
    {
        public float X { get; set; }
        public float Y { get; set; }
        
        public double DistanceTo(Node outputPort)
        {
            var dx = X - outputPort.X;
            var dy = Y - outputPort.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

    public class Line
    {
        public Node Pt1 { get; }
        public Node Pt2 { get; }

        public Line(Node pt1, Node pt2)
        {
            Pt1 = pt1;
            Pt2 = pt2;
        }
    }
}