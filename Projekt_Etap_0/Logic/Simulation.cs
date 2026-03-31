using Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using System.Threading;



namespace Logic
{
    public class Simulation : ISimulation
    {
        public ObservableCollection<IBall> Balls { get; } = new ObservableCollection<IBall>();

        private bool _isRunning = false;
        public void GenerateBall(int ballCount)
        {
            int diameter = Ball.Diameter;
            Random random = new Random();
            float randX = 0.0f;
            float randY = 0.0f;
            int width = (560 - 5 * diameter) / diameter; // 1x to padding i w najgorszym wypadku można tylko osadzić 1/4 kul 
            int height = (280 - 5 * diameter) / diameter;
            
            if ( ballCount > width * height ) // Bardzo nie efektywna metoda !!!
            {
                throw new Exception("Impossible to place this amount of balls! ");
            }
            else
            {
                do
                {
                    randX = (float)random.Next(0, 560 - diameter); //ciekawostka - pozycja elipsy to nie jej centrum tylko lewy górny róg, więc tylko od z górnego przedziału się odejmuje
                    randY = (float)random.Next(0, 280 - diameter);
                } while (!checkIfValidPosition(new Vector2(randX, randY)));

                Ball ball = new Ball(new Vector2(randX, randY));

                BallAdapter adapter = new BallAdapter(ball);
                Balls.Add(adapter);
            }
        }

        public bool checkIfValidPosition(Vector2 position)
        {
            int diameter = Ball.Diameter;
            foreach (IBall ball in Balls)
            {
                if (Vector2.Distance(ball.Position, position) < diameter)
                {
                    return false;
                }
            }
            return true;
        }

        public async void Start()
        {
            if (_isRunning) return;
            _isRunning = true;
            Random random = new Random();

            foreach (IBall ball in Balls)
            {
                ball.Velocity = new Vector2(nextFloat(-3,3), nextFloat(-3, 3));
            }

            await RunSimulationLoop();

        }

        public void Stop()
        {
            _isRunning = false;
        }
        private async Task RunSimulationLoop()
        {
            while (_isRunning)
            {
                //ball.Velocity = ballInitialVelocity - Vector2.Dot(collisionDirection, normal) * normal;
                //ball2.Velocity = ball2InitialVelocity + Vector2.Dot(collisionDirection, normal) * normal;

                for (int i = 0; i < Balls.Count; i++)
                {
                    for (int j = i + 1; j < Balls.Count; j++)
                    {
                        IBall ball1 = Balls[i];
                        IBall ball2 = Balls[j];
                        Vector2 Position1 = ball1.Position;
                        Vector2 Position2 = ball2.Position;
                        Vector2 Velocity1 = ball1.Velocity;
                        Vector2 Velocity2 = ball2.Velocity;
                        float radius = Ball.Diameter / 2;
                        Vector2 Center1 = Vector2.Add(Position1, new Vector2(radius, radius));
                        Vector2 Center2 = Vector2.Add(Position2, new Vector2(radius, radius));

                        float distance = Vector2.Distance(Center1, Center2);

                        if (distance <= Ball.Diameter)
                        {
                            Vector2 normal = Vector2.Normalize(Center1 - Center2);
                            Vector2 dV = ball1.Velocity - ball2.Velocity;

                            if (Vector2.Dot(dV, normal) < 0)
                            {
                                Vector2 collisionResponse = Vector2.Dot(dV, normal) * normal; // skalar daje długość, normalna daje kierunek i zwrot
                                ball1.Velocity -= collisionResponse;
                                ball2.Velocity += collisionResponse;
                            }
                        }

                    }

                }

                foreach(IBall ball in Balls)
                {
                    moveIfLegal(ball);
                }

                await Task.Delay(16);
            }
        }

        public void moveIfLegal(IBall ball)
        {
            Vector2 tempPostition = Vector2.Add(ball.Position, ball.Velocity);

            if (tempPostition.X < 0 || tempPostition.X > 546)
            {
                ball.Velocity = new Vector2(-ball.Velocity.X, ball.Velocity.Y);
            }

            if (tempPostition.Y < 0 || tempPostition.Y > 266)
            {
                ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);
            }

            ball.Position = Vector2.Add(ball.Position, ball.Velocity);
        }

        public float nextFloat(int from, int to)
        {
            Random random = new Random();
            float sample = (float)random.NextDouble();

            return sample * ((float)to - (float)from) + from;
        }
    }

    public class BallAdapter : IBall
    {
        private readonly Ball _ball;

        public event PropertyChangedEventHandler PropertyChanged;

        public Vector2 Position 
        { 
            get { return _ball.Position; }
            set { _ball.Position = value; }
        }

        public Vector2 Velocity 
        { 
            get => _ball.Velocity; 
            set => _ball.Velocity = value; 
        }
        public double X => _ball.Position.X;
        public double Y => _ball.Position.Y;

        public double Diameter => Ball.Diameter;
        public BallAdapter(Ball ball)
        {
            _ball = ball;
            _ball.PropertyChanged += OnBallPropertyChanged;
        }

        private void OnBallPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Position")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Position"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y"));

            }
        }
    }

    
}

