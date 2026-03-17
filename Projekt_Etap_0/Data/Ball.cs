using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Data
{
    public class Ball
    {
        private double _radius { get; set; }
        private Vector2 _position { get; set; }

        public Ball(double radius, Vector2 position)
        {
            _radius = radius;
            _position = position;
        }

        public double Radius { get { return _radius; } }

        public Vector2 Position { get { return _position; } }
    }
}
