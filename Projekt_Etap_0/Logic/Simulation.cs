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

        private QuadTree _quadTree;

        private Vector2[] _nextVelocities;
        private Vector2[] _nextPositions;

        private Barrier _calculationBarrier;
        private Barrier _updateBarrier;

        private System.Diagnostics.Stopwatch _frameStopwatch = new System.Diagnostics.Stopwatch();

        private bool _isRunning = false;
        private readonly IBallRepository _ballRepository;

        private readonly int _diameter;

        public Simulation(IBallRepository ballRepository)
        {
            _ballRepository = ballRepository;
            _diameter = _ballRepository.DefaultDiameter;
        }
        public void GenerateBall(int ballCount)
        {
            int diameter = _diameter;
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

                IDataBall dataBall = _ballRepository.CreateBall(new Vector2(randX, randY));

                BallAdapter adapter = new BallAdapter(dataBall);
                Balls.Add(adapter);
            }
        }

        public bool checkIfValidPosition(Vector2 position)
        {
            foreach (IBall ball in Balls)
            {
                if (Vector2.Distance(ball.Position, position) < _diameter)
                {
                    return false;
                }
            }
            return true;
        }

        public void StartWith(int count)
        {
            if (_isRunning) { return; }

            Balls.Clear();
            for (int i = 0; i < count; i++)
            {
                GenerateBall(count);
            }
            Start();
        }

        public void Start()
        {
            if (_isRunning) return;
            _isRunning = true;
            Random random = new Random();
            int count = Balls.Count;

            _quadTree = new QuadTree(Balls);
            _nextPositions = new Vector2[count];
            _nextVelocities = new Vector2[count];
            _calculationBarrier = new Barrier(count);
            _updateBarrier = new Barrier(count, (barrier) =>
            {
                int elapsed = (int)_frameStopwatch.ElapsedMilliseconds;
                if (elapsed < 16)
                {
                    Thread.Sleep(16 - elapsed);
                }

                _quadTree = new QuadTree(Balls);
                _frameStopwatch.Restart();
            });

            _frameStopwatch.Start();

            for (int i = 0; i < count; i++)
            {
                int localI = i;
                IBall ball = Balls[localI];
                ball.Velocity = new Vector2(nextFloat(-3,3), nextFloat(-3, 3));
                Thread thread = new Thread(() => RunSimulationLoop(ball, localI));
                thread.Start();
            }
        }       

        public void Stop()
        {
            _isRunning = false;
        }

        private void RunSimulationLoop(IBall ball, int index)
        {
            List<IBall> nearbyBalls = new List<IBall>();

            while (_isRunning)
            {
                nearbyBalls.Clear();
                calculateCollision(ball, index, nearbyBalls);
            }
        }

        private void calculateCollision(IBall currentBall, int index, List<IBall> nearbyBalls)
        {
            Vector2 deltaVel = Vector2.Zero;
            float searchRadius = _diameter * 2;
            Rect2D searchRange = new Rect2D(
                currentBall.Position.X - searchRadius,
                currentBall.Position.Y - searchRadius,
                searchRadius * 2,
                searchRadius * 2);

            _quadTree.Query(searchRange, nearbyBalls);

            foreach (IBall otherBall in nearbyBalls)
            {
                if (currentBall == otherBall) continue; // Pomijamy sprawdzenie z samym sobą

                float radius = _diameter / 2;
                Vector2 Center1 = Vector2.Add(currentBall.Position, new Vector2(radius, radius));
                Vector2 Center2 = Vector2.Add(otherBall.Position, new Vector2(radius, radius));

                float distance = Vector2.Distance(Center1, Center2);

                if (distance <= _diameter && distance > 0)
                {
                    Vector2 normal = Vector2.Normalize(Center1 - Center2);
                    Vector2 dV = currentBall.Velocity - otherBall.Velocity;

                    // Warunek zapobiegający "sklejaniu się" kul po zderzeniu
                    if (Vector2.Dot(dV, normal) < 0)
                    {
                        //Vector2 collisionResponse = Vector2.Dot(dV, normal) * normal;
                        //currentBall.Velocity -= collisionResponse;
                        //otherBall.Velocity += collisionResponse;
                        
                        deltaVel -= Vector2.Dot(dV, normal) * normal;
                    }
                }
            }
            Vector2 tempPosition = Vector2.Add(currentBall.Position, currentBall.Velocity);
            Vector2 tempVelocity = Vector2.Add(currentBall.Velocity, deltaVel);

            if (tempPosition.X < 0 || tempPosition.X > 560 - _diameter)
            {
                tempVelocity = new Vector2(-currentBall.Velocity.X, currentBall.Velocity.Y);
            }

            if (tempPosition.Y < 0 || tempPosition.Y > 280 - _diameter)
            {
                tempVelocity = new Vector2(currentBall.Velocity.X, -currentBall.Velocity.Y);
            }

            _nextVelocities[index] = tempVelocity;
            _nextPositions[index] = tempPosition;

            _calculationBarrier.SignalAndWait();

            currentBall.Velocity = _nextVelocities[index];
            currentBall.Position = _nextPositions[index];

            _updateBarrier.SignalAndWait();
        }


            //using (System.IO.StreamWriter writer = new System.IO.StreamWriter("frame_generation_time.txt", append: false))
            //{
            //    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            //    QuadTree tree = new QuadTree(Balls);
            //    Vector2[] velocities = new Vector2[Balls.Count];
            //    Parallel.For(0, Balls.Count, i =>
            //    {
            //        IBall ball1 = Balls[i];

            //        // 2. Szukamy kul tylko w najbliższym otoczeniu (rozmiar średnicy wokół kuli)
            //        float searchRadius = _diameter * 2;
            //        Rect2D searchRange = new Rect2D(
            //            ball1.Position.X - searchRadius,
            //            ball1.Position.Y - searchRadius,
            //            searchRadius * 2,
            //            searchRadius * 2);

            //        List<IBall> nearbyBalls = new List<IBall>();
            //        tree.Query(searchRange, nearbyBalls);

            //        // 3. Sprawdzamy kolizje tylko z wyselekcjonowanymi pobliskimi kulami
            //        Vector2 delV = Vector2.Zero;
            //        foreach (IBall ball2 in nearbyBalls)
            //        {
            //            if (ball1 == ball2) continue; // Pomijamy sprawdzenie z samym sobą

            //            float radius = _diameter / 2;
            //            Vector2 Center1 = Vector2.Add(ball1.Position, new Vector2(radius, radius));
            //            Vector2 Center2 = Vector2.Add(ball2.Position, new Vector2(radius, radius));

            //            float distance = Vector2.Distance(Center1, Center2);

            //            if (distance <= _diameter && distance > 0)
            //            {
            //                Vector2 normal = Vector2.Normalize(Center1 - Center2);
            //                Vector2 dV = ball1.Velocity - ball2.Velocity;

            //                // Warunek zapobiegający "sklejaniu się" kul po zderzeniu
            //                if (Vector2.Dot(dV, normal) < 0)
            //                {
            //                    //Vector2 collisionResponse = Vector2.Dot(dV, normal) * normal;
            //                    //ball1.Velocity -= collisionResponse;
            //                    //ball2.Velocity += collisionResponse;

            //                    delV -= Vector2.Dot(dV, normal) * normal;
            //                }
            //            }
            //        }
            //        velocities[i] = delV;
            //    });

            //    for (int i = 0; i < Balls.Count; i++)
            //    {
            //        Balls[i].Velocity += velocities[i];
            //    }


            //    Parallel.ForEach(Balls, ball =>
            //    {
            //        moveIfLegal(ball);
            //    });

            //    stopwatch.Stop();
            //    await writer.WriteLineAsync(stopwatch.Elapsed.TotalMilliseconds.ToString());
            //    // Istniały klatki, które generowały się dłużej niż 16ms, więc dodajemy opóźnienie tylko wtedy, gdy generacja klatki była szybsza niż 16ms
            //    await Task.Delay(Math.Max(0, 16 - (int)stopwatch.Elapsed.TotalMilliseconds)); 
            //}

        //public void moveIfLegal(IBall ball)
        //{
        //    Vector2 tempPostition = Vector2.Add(ball.Position, ball.Velocity);

        //    if (tempPostition.X < 0 || tempPostition.X > 560 - _diameter)
        //    {
        //        ball.Velocity = new Vector2(-ball.Velocity.X, ball.Velocity.Y);
        //    }

        //    if (tempPostition.Y < 0 || tempPostition.Y > 280 - _diameter)
        //    {
        //        ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);
        //    }

        //    if (Math.Abs(ball.Velocity.X) > ball.MaxVelocity)
        //    {
        //        ball.Velocity = new Vector2(Math.Sign(ball.Velocity.X) * ball.MaxVelocity, ball.Velocity.Y);
        //    }

        //    if (Math.Abs(ball.Velocity.Y) > ball.MaxVelocity)
        //    {
        //        ball.Velocity = new Vector2(ball.Velocity.X, Math.Sign(ball.Velocity.Y) * ball.MaxVelocity);
        //    }

        //    ball.Position = Vector2.Add(ball.Position, ball.Velocity);
        //}

        public float nextFloat(int from, int to)
        {
            Random random = new Random();
            float sample = (float)random.NextDouble();

            return sample * ((float)to - (float)from) + from;
        }
    }

    public class BallAdapter : IBall
    {
        private readonly IDataBall _dataBall;

        public event PropertyChangedEventHandler PropertyChanged;

        public float MaxVelocity => _dataBall.MaxVelocity;

        public Vector2 Position 
        { 
            get { return _dataBall.Position; }
            set { _dataBall.Position = value; }
        }

        public Vector2 Velocity 
        { 
            get => _dataBall.Velocity; 
            set => _dataBall.Velocity = value; 
        }
        public double X => _dataBall.Position.X;
        public double Y => _dataBall.Position.Y;

        public double Diameter => _dataBall.Diameter;
        public BallAdapter(IDataBall dataBall)
        {
            _dataBall = dataBall;
            _dataBall.PropertyChanged += OnBallPropertyChanged;
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
    public struct Rect2D
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public Rect2D(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Contains(Vector2 point)
        {
            return point.X >= X && point.X <= X + Width &&
                   point.Y >= Y && point.Y <= Y + Height;
        }

        public bool Intersects(Rect2D range)
        {
            return !(range.X > X + Width ||
                     range.X + range.Width < X ||
                     range.Y > Y + Height ||
                     range.Y + range.Height < Y);
        }
    }

    public class QuadTree
    {
        private readonly int _capacity;
        private readonly Rect2D _boundary;
        private readonly List<IBall> _balls;
        private bool _divided;

        private QuadTree _northWest;
        private QuadTree _northEast;
        private QuadTree _southWest;
        private QuadTree _southEast;

        public QuadTree(Rect2D boundary, int capacity)
        {
            _boundary = boundary;
            _capacity = capacity;
            _balls = new List<IBall>();
            _divided = false;
        }

        // Konstruktor na podstawie widoku symulacji (560x280)
        public QuadTree(ObservableCollection<IBall> balls)
            : this(new Rect2D(0, 0, 560, 280), 4)
        {
            foreach (var ball in balls)
            {
                Insert(ball);
            }
        }

        public bool Insert(IBall ball)
        {
            if (!_boundary.Contains(ball.Position))
            {
                return false;
            }

            if (_balls.Count < _capacity)
            {
                _balls.Add(ball);
                return true;
            }

            if (!_divided)
            {
                Subdivide();
            }

            if (_northWest.Insert(ball)) return true;
            if (_northEast.Insert(ball)) return true;
            if (_southWest.Insert(ball)) return true;
            if (_southEast.Insert(ball)) return true;

            return false;
        }

        private void Subdivide()
        {
            float x = _boundary.X;
            float y = _boundary.Y;
            float w = _boundary.Width / 2;
            float h = _boundary.Height / 2;

            _northWest = new QuadTree(new Rect2D(x, y, w, h), _capacity);
            _northEast = new QuadTree(new Rect2D(x + w, y, w, h), _capacity);
            _southWest = new QuadTree(new Rect2D(x, y + h, w, h), _capacity);
            _southEast = new QuadTree(new Rect2D(x + w, y + h, w, h), _capacity);

            _divided = true;
        }

        public void Query(Rect2D range, List<IBall> found)
        {
            if (!_boundary.Intersects(range))
            {
                return;
            }

            foreach (IBall ball in _balls)
            {
                if (range.Contains(ball.Position))
                {
                    found.Add(ball);
                }
            }

            if (_divided)
            {
                _northWest.Query(range, found);
                _northEast.Query(range, found);
                _southWest.Query(range, found);
                _southEast.Query(range, found);
            }
        }
    }
}

