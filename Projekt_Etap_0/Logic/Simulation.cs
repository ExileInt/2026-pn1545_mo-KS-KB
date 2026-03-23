using Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;



namespace Logic
{
    public class Simulation : ISimulation
    {
        public ObservableCollection<IBall> Balls { get; } = new ObservableCollection<IBall>();


        public void Start()
        {
            Random random = new Random();
            float randX = (float)random.Next(3, 527);
            float randY = (float)random.Next(3, 277);
            Ball ball = new Ball(new System.Numerics.Vector2(randX, randY));

            BallAdapter adapter = new BallAdapter(ball);
            Balls.Add(adapter);
        }

    }

    public class BallAdapter : IBall
    {
        private readonly Ball _ball;

        public event PropertyChangedEventHandler PropertyChanged;

        public double X => _ball.Position.X;
        public double Y => _ball.Position.Y;
        public double Diameter => 3;
        public BallAdapter(Ball ball)
        {
            _ball = ball;
            _ball.PropertyChanged += OnBallPropertyChanged;
        }
        private void OnBallPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Position")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y"));
            }
        }
    }
}
