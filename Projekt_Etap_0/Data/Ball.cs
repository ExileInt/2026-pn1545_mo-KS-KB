using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;

namespace Data
{
    public class Ball : INotifyPropertyChanged
    {
        private readonly int _radius = 7;

        public event PropertyChangedEventHandler PropertyChanged;

        private Vector2 _position { get; set; }

        public Ball(Vector2 position)
        {
            if (position.X < 0 || position.Y < 0 || position.X > (560 - _radius * 2) || position.Y > (280 - _radius * 2))
                throw new ArgumentException("Position out of bounds");
            _position = position;
        }

        public Vector2 Position 
        {
            get { return _position; } 
            set 
            {
                if (value.X < 0 || value.Y < 0 || value.X > (560 - _radius * 2) || value.Y > (280 - _radius * 2))
                {
                    throw new ArgumentException("Position out of bounds");
                }
                _position = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Position"));
            }
        }
    }
}
