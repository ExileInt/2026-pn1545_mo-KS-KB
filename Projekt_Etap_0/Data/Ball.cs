using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Ball : INotifyPropertyChanged, IDataBall
    {
        private int _diameter = Diameter;

        public event PropertyChangedEventHandler PropertyChanged;

        private Vector2 _position { get; set; }

        private Vector2 _velocity { get; set; }

        public const int Diameter = 14;

        public Ball(Vector2 position)
        {
            if (position.X < 0 || position.Y < 0 || position.X > (560 - _diameter) || position.Y > (280 - _diameter))
                throw new ArgumentException("Position out of bounds");
            _position = position;
            _velocity = new Vector2(0, 0);
        }

        public Vector2 Position 
        {
            get { return _position; } 
            set 
            {
                if (value.X < 0 || value.Y < 0 || value.X > (560 - _diameter) || value.Y > (280 - _diameter))
                {
                    throw new ArgumentException("Position out of bounds");
                }
                _position = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Position"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y"));
            }
        }

        public Vector2 Velocity 
        { 
            get { return _velocity; } 
            set 
            {
                _velocity = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Velocity"));
            }
        }

        int IDataBall.Diameter => _diameter;
    }

    public static class BallFactory
    {
        public static Ball CreateBall(Vector2 position)
        {     
             return new Ball(position);
        }
    }
}
