using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;

namespace Data
{
    public class Ball : INotifyPropertyChanged
    {
        private readonly int _radius = 3;

        public event PropertyChangedEventHandler PropertyChanged;

        private Vector2 _position { get; set; }

        public Ball(Vector2 position)
        {
            if (position.X < _radius || position.Y < _radius || position.X > (530 - _radius) || position.Y > (280 - _radius))
                throw new ArgumentException("Position cannot be negative");
            _position = position;
        }

        public Vector2 Position 
        {
            get { return _position; } 
            set 
            {
                if (value.X < _radius || value.Y < _radius || value.X > (530 - _radius) || value.Y > (280 - _radius))
                {
                    throw new ArgumentException("Position cannot be negative");
                }
                _position = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Position"));
            }
        }
    }
}
